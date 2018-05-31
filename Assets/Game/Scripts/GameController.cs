using UnityEngine;
using Utils;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace LearnChinese {

  /// <summary>
  /// Controller for the game logic.
  /// </summary>
  public class GameController : Singleton<GameController> {

    [Header("References")]
    [SerializeField] Card cardPrefab;

    [Header("Properties")]
    [Tooltip("Determines table size.")]
    [SerializeField] Vector2 gridSize = new Vector2 (3, 4);
    [Tooltip("Card spacing on the grid.")]
    [SerializeField] Vector2 cardSpacing = new Vector2 (3, 4);
    [Tooltip("Time a flipped card shown before being flipped back if not match.")]
    [SerializeField] float displayTime = 1f;
    [Tooltip("Time for a card's pronunciation sound.")]
    [SerializeField] float audioTime = 1f;
    [Tooltip("Time for a card's paired animation.")]
    [SerializeField] float pairedTime = 1f;
    [Tooltip("Final scale size for paired card.")]
    [SerializeField] float pairedScale = 1.1f;

    Card[] cards;
    /// <summary>
    /// List of cards being flipped.
    /// </summary>
    List<Card> flippedCards = new List<Card> ();
    /// <summary>
    /// The number of cards in the middle of flipping.
    /// </summary>
    int numCardFlipping = 0;
    /// <summary>
    /// Indicates whether a card's pronunciation is being played.
    /// </summary>
    bool isPlayingAudio;
    /// <summary>
    /// The total card paired.
    /// </summary>
    int totalPaired = 0;

    /// <summary>
    /// Gets a value indicating whether this instance is in the middle of a play session.
    /// </summary>
    public static bool IsPlaying;

    protected override void OnStart () {
      IsReady = true;
    }

    protected override void OnUpdate () {
      CheckInput ();
    }

    public static void StartSession () {
      IsPlaying = true;
      Instance.totalPaired = 0;
      Instance.Generate ();
    }

    /// <summary>
    /// Checks user input (clicking).
    /// </summary>
    void CheckInput () {
      if (!IsPlaying)
        return;
      if (Input.GetMouseButtonDown (0)) {
        RaycastHit hit;
        if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
          Vector3 origin = Camera.main.transform.position;
          if (!isPlayingAudio && numCardFlipping < 2 && hit.collider.tag == "Card") {
            numCardFlipping++;
            isPlayingAudio = true;
            Card card = hit.collider.GetComponent<Card> ();
            card.Flip (OnCardFlipped);
          }
        }
      }
    }

    /// <summary>
    /// Generates card table.
    /// </summary>
    void Generate () {
      if (cards != null) {
        cards = new Card[0];
      }
      int cardCount = Mathf.RoundToInt (gridSize.x * gridSize.y);
      cards = new Card[cardCount];
      KeyValuePair<int,bool>[] mode = new KeyValuePair<int, bool>[cardCount];
      for (int i = 0; i < mode.Length; i++) {
        mode [i] = new KeyValuePair<int, bool> (Mathf.FloorToInt (i / 2), i % 2 == 0);
      }
      mode = mode.OrderBy (x => Random.Range (0f, 1f)).ToArray ();

      int k = 0;
      Vector2 gridCenter = new Vector2 (gridSize.x / 2f, gridSize.y / 2f);
      for (int i = 0; i < gridSize.x; i++) {
        for (int j = 0; j < gridSize.y; j++) {
          Card card = Instantiate<Card> (cardPrefab, new Vector3 ((gridCenter.x - i - .5f) * cardSpacing.x, (gridCenter.y - j - .5f) * cardSpacing.y), Quaternion.identity);
          card.SetDisplay (Configuration.CardsData [mode [k].Key], mode [k].Value);
          cards [k] = card;
          k++;
        }
      }
    }

    /// <summary>
    /// Called after a card has finished flipping.
    /// </summary>
    /// <param name="card">The flipped card.</param>
    void OnCardFlipped (Card flippedCard) {
      flippedCards.Add (flippedCard);
      StartCoroutine (DelayedChecking ());
    }

    /// <summary>
    /// Delay card checking, wait for audio to be finished first.
    /// </summary>
    IEnumerator DelayedChecking () {
      yield return new WaitForSeconds (audioTime);
      isPlayingAudio = false;
      if (flippedCards.Count == 2) {
        StartCoroutine (DelayedFlipBack ());
      }
    }

    /// <summary>
    /// Delay cards being flipped back, wait for card to be displayed for short while.
    /// </summary>
    IEnumerator DelayedFlipBack () {
      yield return new WaitForSeconds (displayTime);
      if (flippedCards[0].CardType == flippedCards[1].CardType) {
        AnimatePaired (flippedCards [0].gameObject);
        AnimatePaired (flippedCards [1].gameObject);
        totalPaired++;
        UIController.SetPaired (totalPaired);
      } else {
        flippedCards [0].Flip ();
        flippedCards [1].Flip ();
      }
      flippedCards.Clear ();
      numCardFlipping = 0;

      if (totalPaired == cards.Length / 2) {
        UIController.ShowUI (UI.Result);
      }
    }

    void AnimatePaired (GameObject card) {
      Vector3 topCamera = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 1f));
      topCamera.z = 0;
      Sequence scale = DOTween.Sequence ();
      scale.Append (card.transform
        .DOScale (Vector3.one * pairedScale, pairedTime)
        .SetEase (Ease.InOutBack)
        .OnComplete (() => {
            card.GetComponent<Card> ().SetPinyin (false);
          //TODO
          //Add visual effects
          }));
      scale.AppendInterval (displayTime);
      scale.Append (card.transform
        .DOScale (Vector3.zero, pairedTime)
        .SetEase (Ease.InBack));
      Sequence move = DOTween.Sequence ();
      move.Append (card.transform
        .DOLocalMove (Vector3.zero, pairedTime)
        .SetEase (Ease.InBack));
      move.AppendInterval (displayTime);
      move.Append (card.transform
        .DOLocalMove (topCamera, pairedTime)
        .SetEase (Ease.InBack));
      move.OnComplete (() => {
          Destroy (card);
        });
      scale.Play ();
      move.Play ();
    }

  }

}

