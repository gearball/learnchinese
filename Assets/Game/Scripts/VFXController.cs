using UnityEngine;
using Utils;

namespace LearnChinese {

  public class VFXController : Singleton<VFXController> {

    [Header("References")]
    [SerializeField] ParticleSystem pairVFX;

    protected override void OnStart () {
      IsReady = true;
    }

    public static void PlayVFX (VFX vfx) {
      if (vfx == VFX.Paired) {
        Instance.pairVFX.Play ();
      }
    }

  }

}

