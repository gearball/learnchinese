﻿using UnityEngine;

namespace LearnChinese {

  [CreateAssetMenu(fileName = "Configuration", menuName = "LearnChinese/Configuration")]
  public class Configuration : ScriptableObject {

    static Configuration instance;

    static Configuration Instance {
      get {
        if (instance == null) {
          instance = Resources.Load<Configuration> ("Configuration");
        }
        return instance;
      }
    }

    [Header("Audio Setting")]
    [Tooltip("Played when two card is paired.")]
    [SerializeField] AudioClip pairedSFX;

    /// <summary>
    /// Audio clip when two card is paired.
    /// </summary>
    public static AudioClip PairedSFX {
      get {
        return Instance.pairedSFX;
      }
    }

    [Header("Animation Setting")]
    [Tooltip("How far the card move towards camera when flipped.")]
    [SerializeField] float flipForwardDistance = 1f;
    [Tooltip("How big the card scaled when flipped.")]
    [SerializeField] float flipScale = 1.5f;
    [Tooltip("How long flipping animation take.")]
    [SerializeField] float flipDuration = .5f;
    [Tooltip("How long scale animation take.")]
    [SerializeField] float scaleDuration = .5f;

    /// <summary>
    /// How far the card move towards camera when flipped. 
    /// </summary>
    public static float FlipForwardDistance {
      get {
        return Instance.flipForwardDistance;
      }
    }

    /// <summary>
    /// How big the card scaled when flipped. 
    /// </summary>
    public static float FlipScale {
      get {
        return Instance.flipScale;
      }
    }

    /// <summary>
    /// How long flipping animation take. 
    /// </summary>
    public static float FlipDuration {
      get {
        return Instance.flipDuration;
      }
    }

    /// <summary>
    /// How long flipping animation take. 
    /// </summary>
    public static float ScaleDuration {
      get {
        return Instance.scaleDuration;
      }
    }

    [Header("Card Lists")]
    [Tooltip("List of card data")]
    [SerializeField] CardData[] cardsData;

    /// <summary>
    /// List of card data.
    /// </summary>
    /// <value>The cards data.</value>
    public static CardData[] CardsData {
      get {
        return Instance.cardsData;
      }
    }
   
  }

}

