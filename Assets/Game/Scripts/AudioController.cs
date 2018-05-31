using UnityEngine;
using Utils;
using System.Collections;

namespace LearnChinese {

  public class AudioController : Singleton<AudioController> {

    [SerializeField] AudioSource speakerSFX;
    [SerializeField] AudioSource speakerBGM;
    [SerializeField] AudioSource speakerArea;


    protected override void OnAwake () {
      base.OnAwake ();
    }

    protected override void OnStart () {
      IsReady = true;
    }

    /// <summary>
    /// Plays an audio clip once.
    /// </summary>
    /// <param name="clip">AudioClip.</param>
    public static void PlayOnce (AudioClip clip, float volume = 1) {
      Instance.speakerSFX.PlayOneShot (clip, volume);
    }

    /// <summary>
    /// Plays specific time frame on the audio clip.
    /// </summary>
    /// <param name="clip">AudioClip.</param>
    /// <param name="start">Start time.</param>
    /// <param name="end">End time.</param>
    public static void PlayArea (AudioClip clip, float start, float end) {
      Instance.StartCoroutine(Instance.playArea (clip, start, end));
    }

    IEnumerator playArea (AudioClip clip, float start, float end) {
      speakerArea.time = start;
      speakerArea.clip = clip;
      speakerArea.Play ();
      yield return new WaitForSeconds (end - start);
      speakerArea.Stop ();
    }


  }
}

