using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Utils {

  /// <summary>
  /// Singleton for single class usage.
  /// </summary>
  public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    static T instance;

    protected static T Instance {
      get {
        if (!IsReady) {
          Debug.Log (string.Format ("[{0}] is not ready", typeof (T).ToString ()));
          return null;
        }
        return instance;
      }
    }

    /// <summary>
    /// Determine if the singleton of this class is ready to be used. Make sure this is set to true to use it.
    /// </summary>
    /// <value><c>true</c> if is ready; otherwise, <c>false</c>.</value>
    public static bool IsReady {
      get;
      protected set;
    }

    public static bool IsEnabled {
      get;
      protected set;
    }

    void Awake () {
      IsEnabled = true;
      OnAwake ();
    }

    void Start () {
      OnStart ();
    }

    void Update () {
      OnUpdate ();
    }

    void OnDestroy () {
      OnDestroyed ();
    }

    void OnEnable () {
      OnEnabled ();
    }

    void OnDisable () {
      OnDisabled ();
    }

    protected virtual void OnAwake () {
      T[] ts = FindObjectsOfType<T> ();
      if (ts.Length == 0) {
        Debug.Log (string.Format ("[{0}] instance not found!", typeof (T).ToString ()));
      } else if (ts.Length > 1) {
        Debug.Log (string.Format ("[{0}] instance is not singleton!", typeof (T).ToString ()));
      } else {
        instance = (T) FindObjectOfType<T> ();
      }
    }

    protected virtual void OnStart () {

    }

    protected virtual void OnUpdate () {

    }

    protected virtual void OnDestroyed () {
      IsReady = false;
    }

    protected virtual void OnEnabled () {
      IsEnabled = true;
    }

    protected virtual void OnDisabled () {
      IsEnabled = false;
    }

  }

}