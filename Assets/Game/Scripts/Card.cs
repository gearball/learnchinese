using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace LearnChinese {

  /// <summary>
  /// Contains various animation method and callbacks on the card.
  /// </summary>
  public class Card : MonoBehaviour {

    public class CardEvent : UnityEvent<Card> {}

    public CardEvent OnFlipped = new CardEvent ();

    [Header("References")]
    [SerializeField] SpriteRenderer sprObject;
    [SerializeField] SpriteRenderer sprText;

    CardData cardData;

    /// <summary>
    /// Gets the type of the card.
    /// </summary>
    /// <value>The type of the card.</value>
    public string CardType {
      get {
        if (cardData != null) {
          return cardData.name;
        }
        return "";
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is opened.
    /// </summary>
    /// <value><c>true</c> if this instance is opened; otherwise, <c>false</c>.</value>
    public bool IsOpened {
      get;
      private set;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is in the middle of an animation.
    /// </summary>
    /// <value><c>true</c> if this instance is in the middle of an animation; otherwise, <c>false</c>.</value>
    public bool IsAnimating {
      get;
      private set;
    }

    /// <summary>
    /// Flips open / close a card.
    /// </summary>
    /// <param name="isOpen">If set to <c>true</c> open the card.</param>
    public void Flip (UnityAction<Card> onFlipped = null) {
      if (IsAnimating)
        return;
      OnFlipped.RemoveAllListeners ();
      if (onFlipped != null) {
        OnFlipped.AddListener (onFlipped);
      }
      IsAnimating = true;
      transform
        .DOBlendableLocalMoveBy (Vector3.forward * Configuration.FlipForwardDistance * -1f, Configuration.FlipDuration / 2)
        .SetLoops (2, LoopType.Yoyo)
        .SetEase (Ease.OutCubic);
      transform
        .DOBlendableScaleBy (Vector3.one * Configuration.FlipScale, Configuration.FlipDuration / 2)
        .SetLoops (2, LoopType.Yoyo)
        .SetEase (Ease.OutCubic);
      transform
        .DOBlendableLocalRotateBy (Vector3.up * (!IsOpened ? 180f : -180f), Configuration.FlipDuration, RotateMode.LocalAxisAdd)
        .SetEase (Ease.Linear)
        .OnComplete (() => {
          IsOpened = !IsOpened;
          if (IsOpened) {
            AudioController.PlayOnce (cardData.clip);
          }
          IsAnimating = false;
          OnFlipped.Invoke (this);
        });
    }

    /// <summary>
    /// Sets card display.
    /// </summary>
    /// <param name="data">CardData to be applied.</param>
    /// <param name="isPinyin">If set to <c>true</c> is pinyin.</param>
    public void SetDisplay (CardData data, bool isPinyin) {
      cardData = data;
      sprObject.sprite = data.sprObject;
      sprText.sprite = isPinyin ? data.sprPinyin : data.sprChinese;
    }

    /// <summary>
    /// Sets the text mode, pinyin or chinese.
    /// </summary>
    /// <param name="isPinyin">If set to <c>true</c> is pinyin.</param>
    public void SetPinyin (bool isPinyin) {
      sprText.sprite = isPinyin ? cardData.sprPinyin : cardData.sprChinese;
    }

    /// <summary>
    /// Play spawn animation.
    /// </summary>
    public void Spawn (float delay = 0f) {
      transform
        .DOScale (Vector3.one, Configuration.ScaleDuration)
        .ChangeStartValue (Vector3.zero)
        .SetDelay (delay)
        .SetEase (Ease.OutBack);
    }
  }

}
