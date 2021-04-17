using System.Collections;
using UnityEngine;

namespace com.gb.statemachine_toolkit
{
    public class StartManager : MonoBehaviour
    {
        public string sceneToLoadWhenReady = "MainScene";

        // Start is called before the first frame update
        IEnumerator Start()
        {
            while (Copy.Instance == null && !Copy.Instance.Initialised)
                yield return null;
            Debug.Log($"Ready to load scene {sceneToLoadWhenReady}");
            while (SceneLoader.Instance == null)
            {
                yield return null;
                Debug.Log("waiting for SceneLoader to be ready");
            }
            SceneLoader.Instance.Load(sceneToLoadWhenReady);
        }
    }
}
