using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.gb.statemachine_toolkit
{
    public class SceneLoader : MonoBehaviour
    {
        [Tooltip("If the scene should be loaded asynchronously or not")]
        public bool asyncLoad;
        [Tooltip("The Canvas prefab that will be instantiated when loading a new Scene. ")]
        public GameObject loadCanvasPrefab;
        [Tooltip("If the progress bar should be shown when loading")]
        public bool showProgress;
        [Tooltip("If the loading of the new scene should happen automatically, without the user pressing a button to confirm.")]
        public bool autoActivateScene = true;

        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<SceneLoader>();
                    if (_instance)
                    {
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        public void Load(string sceneToLoad)
        {
            if (string.IsNullOrWhiteSpace(sceneToLoad))
            {
                Debug.LogError("Cannot load a new scene, Scene To Load is empty!");
                return;
            }
            if (asyncLoad)
                StartCoroutine(Loading(sceneToLoad));
            else
                SceneManager.LoadScene(sceneToLoad);
        }

        public void AllowSceneChange()
        {
            Debug.Log("SceneChange allowed by user");
            this._allowSceneActivation = true;
        }

        public void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;

        }

        private static SceneLoader _instance;
        private bool _allowSceneActivation;

        IEnumerator Loading(string sceneToLoad)
        {
            LoadCanvas lc = null;

            Debug.Log($"Loading scene {sceneToLoad}");

            if (loadCanvasPrefab)
            {
                var canvas = Instantiate(loadCanvasPrefab, this.transform);
                lc = canvas.GetComponent<LoadCanvas>();
                if (lc)
                {
                    if (lc.progressImage) lc.progressImage.SetActive(true);
                    if (lc.introText) lc.introText.SetActive(true);
                    if (lc.progressImage) lc.progressImage.SetActive(true);
                    if (lc.progressFillImage) lc.progressFillImage.gameObject.SetActive(showProgress);
                    if (lc.startButton)
                    {
                        if (!autoActivateScene)
                        {
                            lc.startButton.onClick.AddListener(AllowSceneChange);
                            lc.startButton.gameObject.SetActive(true);
                        }
                        else
                            lc.startButton.gameObject.SetActive(false);
                    }
                    if (lc.fullscreenButton)
                    {
                        if (!autoActivateScene)
                        {
                            lc.fullscreenButton.onClick.AddListener(()=> { lc.fullscreenButton.gameObject.SetActive(false); Screen.fullScreen = true; });
                            lc.fullscreenButton.gameObject.SetActive(true);
                        }
                        else
                            lc.fullscreenButton.gameObject.SetActive(false);
                    }
                }
            }

            var loadOp = SceneManager.LoadSceneAsync(sceneToLoad);
            if (loadOp == null)
            {
                Debug.LogError($"SceneLoader: Cannot load {sceneToLoad}, check the scene name and be sure to add it to the build settings.");
                yield break;
            }
            loadOp.allowSceneActivation = this._allowSceneActivation = autoActivateScene;

            while (!loadOp.isDone)
            {
                yield return null;

                if (lc && lc.progressFillImage) lc.progressFillImage.fillAmount = loadOp.progress;

                if (loadOp.progress >= 0.9f)
                {
                    //Debug.Log($"Scene {sceneToLoad} loaded!");
                    // Scene has been loaded here, we can get rid of the loading bar
                    if (lc && lc.progressImage) lc.progressImage.SetActive(false);

                    if (this._allowSceneActivation)
                    {
                        loadOp.allowSceneActivation = true;
                    }
                }
            }
            // reset the flag to be able to load another scene later
            _allowSceneActivation = !autoActivateScene;

            // finally hide the canvas
            // TODO trigger an out animation before destroying?
            if (lc) Destroy(lc.gameObject);
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                if (this != _instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
