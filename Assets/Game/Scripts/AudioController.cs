using UnityEngine;
using Utils;

namespace LearnChinese {

  [RequireComponent(typeof(AudioSource))]
  public class AudioController : Singleton<AudioController> {

    AudioSource speaker;

    protected override void OnAwake () {
      base.OnAwake ();
      speaker = GetComponent<AudioSource> ();
    }

    protected override void OnStart () {
      IsReady = true;
    }

    /// <summary>
    /// Plays an audio clip once.
    /// </summary>
    /// <param name="clip">AudioClip.</param>
    public static void PlayOnce (AudioClip clip) {
      Instance.speaker.PlayOneShot (clip);
    }


  }
}

