using Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

namespace LearnChinese {
  
  public class UIController : Singleton<UIController> {

    [Header("References")]
    [SerializeField] Canvas canvas;
    [SerializeField] Image[] imgBackground;

    [Header("UI Title")]
    [SerializeField] GameObject conTitle;
    [SerializeField] TextMeshProUGUI txtTapStart;
    [SerializeField] TextMeshProUGUI txtTitleStart;

    [Header("UI Gameplay")]
    [SerializeField] GameObject conGameplay;
    [SerializeField] TextMeshProUGUI txtMatches;

    [Header("UI Result")]
    [SerializeField] GameObject conResult;
    [SerializeField] TextMeshProUGUI txtTapOver;
    [SerializeField] TextMeshProUGUI txtTitleOver;

    [Header("Properties")]
    [Tooltip("Transition animation time duration")]
    [SerializeField] float transitionTime = 1f;
    [Tooltip("Speed of moving background")]
    [SerializeField] float bgMoveTime = 1f;
    [Tooltip("Fading time duration")]
    [SerializeField] float fadeTime = 1f;
    [Tooltip("UpDown animation time duration")]
    [SerializeField] float upDownTime = 1f;
    [Tooltip("UpDown animation distance")]
    [SerializeField] float upDownDistance = 1f;
    [Tooltip("Scale animation value")]
    [SerializeField] float scaleValue = 1f;
    [Tooltip("Scale animation time duration")]
    [SerializeField] float scaleTime = .5f;

    /// <summary>
    /// Current active UI.
    /// </summary>
    GameObject conActive;

    /// <summary>
    /// Indicates whether the UI is in the middle of a transition.
    /// </summary>
    public static bool IsTransitioning {
      get;
      private set;
    }

    protected override void OnStart () {
      AnimateBackground ();
      AnimateTitle ();
      AnimateResult ();
      InitUI ();
      IsReady = true;
    }

    /// <summary>
    /// Animates the background so it does not look bleak.
    /// </summary>
    void AnimateBackground () {
      for (int i = 0; i < imgBackground.Length; i++) {
        imgBackground [i].GetComponent<RectTransform> ()
          .DOBlendableLocalMoveBy (Vector3.right * imgBackground [i].rectTransform.sizeDelta.x, bgMoveTime)
          .SetLoops (-1, LoopType.Restart)
          .SetEase (Ease.Linear);
      }
    }

    /// <summary>
    /// Animates UI Title components.
    /// </summary>
    void AnimateTitle () {
      StartCoroutine (TweenFadingText (txtTapStart));
      txtTitleStart.GetComponent<RectTransform> ()
        .DOBlendableLocalMoveBy (Vector3.up * upDownDistance, upDownTime)
        .SetLoops (-1, LoopType.Yoyo)
        .SetEase (Ease.Linear);
    }

    /// <summary>
    /// Animates UI Result components.
    /// </summary>
    void AnimateResult () {
      StartCoroutine (TweenFadingText (txtTapOver));
      txtTitleOver.GetComponent<RectTransform> ()
        .DOBlendableLocalMoveBy (Vector3.up * upDownDistance, upDownTime)
        .SetLoops (-1, LoopType.Yoyo)
        .SetEase (Ease.Linear);
    }

    /// <summary>
    /// Adds a looping fade tween on a text.
    /// </summary>
    /// <returns>The fading text.</returns>
    /// <param name="txtUI">TextMeshPro uGUI</param>
    IEnumerator TweenFadingText (TextMeshProUGUI txtUI) {
      float alpha = 0;
      float time = 0;
      bool isFadeOut = false;
      while (true) {
        yield return null;
        if (isFadeOut) {
          alpha = 1f - time / fadeTime;
        } else {
          alpha = time / fadeTime;
        }
        Color color = txtUI.color;
        color.a = alpha;
        txtUI.color = color;
        time += Time.deltaTime;
        if (time >= fadeTime) {
          isFadeOut = !isFadeOut;
          time = 0;
        }
      }
    }

    /// <summary>
    /// Sets number of paired card on UI.
    /// </summary>
    /// <param name="paired">Number of card paired.</param>
    public static void SetPaired (int paired) {
      Instance.setPaired (paired);
    }

    void setPaired (int paired, bool instance = false) {
      txtMatches.SetText (string.Format ("Paired: {0}", paired));
      int id = txtMatches.GetInstanceID ();
      DOTween.Kill (id);
      DOTween.Restart (id);
      if (instance) {
        txtMatches.transform
          .DOScale (scaleValue, scaleTime)
          .SetEase (Ease.Linear)
          .SetLoops (2, LoopType.Yoyo)
          .SetId (id);
      }
    }

    /// <summary>
    /// Starts playing session.
    /// </summary>
    public void StartSession () {
      ShowUI (UI.Gameplay, GameController.StartSession);
    }

    /// <summary>
    /// Return to title.
    /// </summary>
    public void EndSession () {
      ShowUI (UI.Title);
    }

    /// <summary>
    /// Inits all UI position and state.
    /// </summary>
    void InitUI () {
      conGameplay.SetActive (true);
      conTitle.SetActive (true);
      conResult.SetActive (true);
      conActive = conTitle;
      Rect rectGameplay = conGameplay.GetComponent<RectTransform> ().rect;
      Vector2 sizeGameplay = new Vector2 (rectGameplay.x, rectGameplay.y);
      conGameplay.GetComponent<RectTransform> ().localPosition = new Vector3 (conGameplay.GetComponent<RectTransform> ().localPosition.x, sizeGameplay.y * 2);
      Rect rectResult = conResult.GetComponent<RectTransform> ().rect;
      Vector2 sizeResult = new Vector3 (rectResult.x, rectResult.y);
      conResult.GetComponent<RectTransform> ().localPosition = new Vector3 (conResult.GetComponent<RectTransform> ().localPosition.x, sizeResult.y * 2);
    }

    /// <summary>
    /// Shows the UI needed.
    /// </summary>
    /// <param name="ui">Designated UI.</param>
    /// <param name="onComplete">On complete callback.</param>
    public static void ShowUI (UI ui, UnityAction onComplete = null) {
      Instance.showUI (ui, onComplete);
    }

    void showUI (UI ui, UnityAction onComplete) {
      if (IsTransitioning)
        return;
      IsTransitioning = true;
      GameObject conNew = conActive;
      Rect rectActive = conActive.GetComponent<RectTransform> ().rect;
      Vector2 sizeActive = new Vector2 (rectActive.width, rectActive.height);
      Vector2 sizeNew = new Vector2 (0, 0);
      if (ui == UI.Gameplay) {
        if (conActive == conGameplay)
          return;
        SetPaired (0);
        conNew = conGameplay;
      } else if (ui == UI.Title) {
        if (conActive == conTitle)
          return;
        conNew = conTitle;
      } else if (ui == UI.Result) {
        if (conActive == conResult)
          return;
        conNew = conResult;
      }
      Rect rectNew = conNew.GetComponent<RectTransform> ().rect;
      sizeNew = new Vector2 (rectNew.width, rectNew.height);
      GameObject colActiveOld = conActive;
      conActive.GetComponent<RectTransform> ()
        .DOBlendableLocalMoveBy (Vector3.up * sizeActive.y, transitionTime)
        .ChangeStartValue (Vector3.zero)
        .SetEase (Ease.InBack)
        .OnComplete (() => {
          colActiveOld.GetComponent<RectTransform> ().localPosition = new Vector3 (colActiveOld.GetComponent<RectTransform> ().localPosition.x, sizeActive.y * -1);
        });
      conNew.GetComponent<RectTransform> ()
        .DOBlendableLocalMoveBy (Vector3.up * sizeNew.y, transitionTime)
        .ChangeStartValue (Vector3.zero)
        .SetEase (Ease.InBack)
        .OnComplete (() => {
          IsTransitioning = false;
          if (onComplete != null) {
            onComplete ();
          }
        });
      conActive = conNew;
    }

  }
}

