using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

namespace com.gb.statemachine_toolkit
{
    [RequireComponent(typeof(Animator))]
    public class StateManager : MonoBehaviour
    {
        [Tooltip("The player GameObject. " +
            "\nTo be able to move, click on InteractiveObjects, and rotate its camera, it needs a PlayerController component.")]
        public GameObject player;
        [Tooltip("The main dialogue that will be used to display sequence of texts or questions.")]
        public Dialogue dialogue;
        [Tooltip("The state animator that drives the behaviour of this scene.")]
        private Animator stateAnimator;
        [Tooltip("If all the InteractiveObjects found in the scene have to be disabled when the Scene starts.")]
        public bool disableInteractiveObjectsOnStart = true;

        private PlayerController playerController;
        private InteractiveObject[] interactiveObjects;

        void Awake()
        {
            stateAnimator = this.GetComponent<Animator>();

            if (!player)
            {
                Debug.LogWarning("Player GameObject not set in StateManager");
                return;
            }

            playerController = player.GetComponent<PlayerController>();
            if (!playerController) Debug.LogWarning("PlayerController not found on Player!");
        }

        private void Start()
        {
            if (disableInteractiveObjectsOnStart)
            {
                interactiveObjects = FindObjectsOfType<InteractiveObject>();
                // start with all the interactiveObjects in scene disabled
                foreach (var io in interactiveObjects)
                {
                    io.Enable(false);
                }
            }
        }

        #region SET_PARAMETERS

        /// <summary>
        /// Increase by one the Int Parameter "Stage" of the StateManager animator.
        /// </summary>
        public void NextStage()
        {
            stateAnimator.SetInteger("Stage", stateAnimator.GetInteger("Stage") + 1);
            Debug.Log("Next Stage! " + stateAnimator.GetInteger("Stage"));
        }

        /// <summary>
        /// Increase by one the given parameter in the state animator
        /// </summary>
        /// <param name="parameter">the name of the parameter to increase</param>
        public void IncreaseInt(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                Debug.Log("Cannot increase Int Parameter, the specified one is empty.");
            }
            this.stateAnimator.SetInteger(parameter, this.stateAnimator.GetInteger(parameter) + 1);
        }

        /// <summary>
        /// Decrease by one the given parameter in the state animator
        /// </summary>
        /// <param name="parameter">the name of the parameter to decrease</param>
        public void DecreaseInt(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                Debug.Log("Cannot decrease Int Parameter, the specified one is empty.");
            }
            this.stateAnimator.SetInteger(parameter, this.stateAnimator.GetInteger(parameter) - 1);
        }

        /// <summary>
        /// Set an int parameter in the state animator
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public void SetInt(string parameter, int value)
        {
            if (!Utilities.HasParameter(parameter, this.stateAnimator))
            {
                Debug.LogError($"There is no parameter \"{parameter}\" in the animator of {this.name}");
                return;
            }
            var intParam = this.stateAnimator.GetInteger(parameter);
            this.stateAnimator.SetInteger(parameter, value);
            Debug.Log($"Int parameter: {parameter} new value: {intParam}");
        }

        /// <summary>
        /// Get an int parameter in the state animator
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int GetIntParameter(string parameter)
        {
            return this.stateAnimator.GetInteger(parameter);
        }

        /// <summary>
        /// Toggle a bool parameter in the state animator
        /// </summary>
        /// <param name="parameter"></param>
        public void ToggleBool(string parameter)
        {
            this.stateAnimator.SetBool(parameter, !this.stateAnimator.GetBool(parameter));
        }

        /// <summary>
        /// Set a bool parameter in the state animator
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public void SetBool(string parameter, bool value)
        {
            this.stateAnimator.SetBool(parameter, value);
        }

        /// <summary>
        /// Set a trigger in the state animator
        /// </summary>
        /// <param name="parameter"></param>
        public void SetTrigger(string parameter)
        {
            this.stateAnimator.SetTrigger(parameter);
        }

        #endregion SET_PARAMETERS

        #region TEXT

        public void SetText(string textID, string textToSet)
        {
            var gos = GameObject.FindGameObjectsWithTag(textID);
            foreach (var go in gos)
            {
                var text = go.GetComponent<Text>();
                if (text) text.text = textToSet;

                var tmp = go.GetComponent<TextMeshProUGUI>();
                if (tmp) tmp.SetText(textToSet);
            }
        }

        #endregion TEXT

        #region DIALOGUE

        public void OpenDialogue(string copyId, Action onLastText)
        {
            if (!dialogue)
            {
                Debug.LogWarning("Cannot Open Dialogue, no Dialogue component found. Please assign a Dialogue present in the current scene to the \"Dialogue\" inspector of the StateManager.");
                return;
            }
            if (string.IsNullOrWhiteSpace(copyId))
            {
                Debug.LogWarning("Cannot Open Dialogue, copyId is empty!");
                return;
            }
            var strings = Copy.Instance.GetCopy(copyId);
            if (strings == null)
            {
                Debug.LogError($"Cannot Open Dialogue, no texts found at copy ID {copyId}. Be sure to have a Copy component in the scene.");
                return;
            }
            Debug.Log($"Opening dialogue: {copyId}");
            dialogue.Show(strings, onLastText);
        }

        /// <summary>
        /// Opens a dialogue and goes to the next stage immediately after the last text has been read.
        /// </summary>
        /// <param name="copyId"></param>
        public void OpenDialogue(string copyId)
        {
            if (!dialogue)
            {
                Debug.LogWarning("Cannot Open Dialogue, no dialogue found. Please assign a Dialogue present in the current scene to the \"Dialogue\" inspector of the StateManager.");
                return;
            }
            if (string.IsNullOrWhiteSpace(copyId))
            {
                Debug.LogWarning("Cannot Open Dialogue, copyId is empty!");
                return;
            }
            var strings = Copy.Instance.GetCopy(copyId);
            if (strings == null)
            {
                Debug.LogError($"Cannot Open Dialogue, no texts found at copy ID {copyId}. Be sure to have a Copy component in the scene.");
                return;
            }
            Debug.Log($"Opening dialogue: {copyId}");
            dialogue.Show(strings, NextStage);
        }

        public void OpenQuestion(string copyId, string answerIntParam, float timeToWait, Action onAnswer)
        {
            if (!dialogue)
            {
                Debug.LogWarning("Cannot Open Question, No dialogue found. Please assign a Dialogue present in the current scene to the \"Dialogue\" inspector of the StateManager.");
                return;
            }
            //Debug.Log($"Opening question: {copyId}, answerIntParam: {answerIntParam}");
            var strings = Copy.Instance.GetCopy(copyId);
            if (strings == null)
            {
                Debug.LogError($"Cannot Open question, no copy found with copyId {copyId}");
                return;
            }

            dialogue.ShowQuestion(strings, timeToWait, (parameter) =>
            {
                this.SetInt(answerIntParam, parameter);
                onAnswer?.Invoke();
            });
        }

        #endregion DIALOGUE

        #region INTERACTION

        /// <summary>
        /// Enables full interaction
        /// Player can move, look around, and click.
        /// </summary>
        /// <param name="enable">if the interaction should be enabled or not</param>
        public void EnableInteraction(bool enable)
        {
            Debug.Log((enable ? "Enabling" : "Disabling") + " all interactions");
            EnableMovement(enable);
            EnableInput(enable);
            EnableMouseLook(enable);
        }

        /// <summary>
        /// Enables the script that controls the first person camera look through mouse movement or mobile joystick
        /// </summary>
        /// <param name="enable"></param>
        public void EnableMouseLook(bool enable)
        {
            Debug.Log((enable ? "Enabling" : "Disabling") + " Look");

            if (playerController)
            {
                playerController.lookActive = enable;
                playerController.LockView(enable);
            }
        }

        /// <summary>
        /// Enables the interaction with InteractiveObjects
        /// </summary>
        /// <param name="enable"></param>
        public void EnableInput(bool enable)
        {
            Debug.Log((enable ? "Enabling" : "Disabling") + " Input");
            if (playerController) playerController.interactionActive = enable;
        }

        /// <summary>
        /// Enables the player movement using the keyboard or mobile joystick
        /// </summary>
        /// <param name="enable"></param>
        public void EnableMovement(bool enable)
        {
            Debug.Log((enable ? "Enabling" : "Disabling") + " Movement");
            if (playerController) playerController.movementActive = enable;
        }

        /// <summary>
        /// Enables an InteractiveObject for interaction
        /// </summary>
        /// <param name="go">The gameObject where to look for the InteractiveObject component</param>
        /// <param name="enable">Enable or not interaction</param>
        public void EnableInteractiveObject(GameObject go, bool enable)
        {
            var io = go.GetComponent<InteractiveObject>();
            if (io == null)
            {
                Debug.LogError($"Cannot enable object with tag {tag}, no InteractiveObject component found on GameObject {go.name}");
                return;
            }
            io.Enable(enable);
        }

        /// <summary>
        /// Enables one or more InteractiveObject for interaction
        /// It just enables/disables the collider component found.
        /// </summary>
        /// <param name="tag">The tag of the InteractiveObjects to find</param>
        /// <param name="enable">Enable or not interaction of the found InteractiveObjects, disabling their parent colliders.</param>
        public void EnableInteractiveObjects(string tag, bool enable)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                Debug.LogError("Cannot enable Interactive Object(s), tag is empty!");
                return;
            }

            var gos = GameObject.FindGameObjectsWithTag(tag);
            if (gos == null)
            {
                Debug.LogError($"No object found with tag {tag}");
                return;
            }

            foreach (var go in gos)
                EnableInteractiveObject(go, enable);
        }

        #endregion INTERACTION

        #region CUTSCENE

        /// <summary>
        /// Sets a trigger and/or a bool in an animator found in the object with the given tag
        /// Warning: trigger and bool could be activated at the same time
        /// </summary>
        /// <param name="animatedObjectTag">The tag of the object with the animator component</param>
        /// <param name="animatedObjectTrigger">The name of the trigger to set</param>
        /// <param name="animatedObjectBool">The name of the bool to set</param>
        /// <param name="animatedObjectBoolValue">The bool value to set</param>
        public void AnimateObject(string animatedObjectTag, string animatedObjectTrigger, string animatedObjectBool, bool animatedObjectBoolValue)
        {
            Debug.Log($"StateManager.AnimateObject " +
                $"animatedObjectTag: {animatedObjectTag} " +
                $"animatedObjectTrigger {animatedObjectTrigger} " +
                $"animatedObjectBool {animatedObjectBool} " +
                $"animatedObjectBoolValue {animatedObjectBoolValue}");
            if (string.IsNullOrWhiteSpace(animatedObjectTag))
            {
                Debug.LogError($"Cannot animate object, tag is empty!");
                return;
            }
            var go = GameObject.FindGameObjectWithTag(animatedObjectTag);
            if (!go)
            {
                Debug.LogError($"Cannot animate object with tag {tag}, object not found");
                return;
            }
            var anim = go.GetComponent<Animator>();
            if (!anim)
            {
                Debug.LogError($"Cannot animate object with tag {tag}, animator component not found");
                return;
            }
            if (!string.IsNullOrWhiteSpace(animatedObjectTrigger))
                anim.SetTrigger(animatedObjectTrigger);
            if (!string.IsNullOrWhiteSpace(animatedObjectBool))
                anim.SetBool(animatedObjectBool, animatedObjectBoolValue);
        }

        public void AnimateTrigger(string animatedObjectTag, string animatedObjectTrigger)
        {
            var anim = FindAnimator(animatedObjectTag);
            if (!string.IsNullOrWhiteSpace(animatedObjectTrigger))
            {
                Debug.Log($"Setting trigger {animatedObjectTrigger} to object with tag {animatedObjectTag}");
                anim.SetTrigger(animatedObjectTrigger);
            }
            else
                Debug.LogError($"Cannot set trigger {animatedObjectTrigger} on the animator of object with tag {animatedObjectTag}");
        }

        public void AnimateBool(string animatedObjectTag, string animatedObjectBool, bool animatedObjectBoolValue)
        {
            var anim = FindAnimator(animatedObjectTag);
            if (!string.IsNullOrWhiteSpace(animatedObjectBool))
            {
                Debug.Log($"Setting bool {animatedObjectBool} with value {animatedObjectBoolValue} to object with tag {animatedObjectTag}");
                anim.SetBool(animatedObjectBool, animatedObjectBoolValue);
            }
            else
                Debug.LogError($"Cannot set bool {animatedObjectBool} on the animator of object with tag {animatedObjectTag}");
        }

        private Animator FindAnimator(string animatedObjectTag)
        {
            if (string.IsNullOrWhiteSpace(animatedObjectTag))
            {
                Debug.LogError($"Cannot animate object, tag is empty!");
                return null;
            }
            var go = GameObject.FindGameObjectWithTag(animatedObjectTag);
            if (!go)
            {
                Debug.LogError($"Cannot animate object with tag {tag}, object not found");
                return null;
            }
            var anim = go.GetComponent<Animator>();
            if (!anim)
            {
                Debug.LogError($"Cannot animate object with tag {tag}, animator component not found");
                return null;
            }
            return anim;
        }

        public void PlayTimeline(string animatedObjectTag, TimelineAsset timelineAsset)
        {
            if (string.IsNullOrWhiteSpace(animatedObjectTag))
            {
                Debug.LogError($"Tag cannot be empty, cannot play timelineAsset {timelineAsset.name}");
                return;
            }
            var go = GameObject.FindGameObjectWithTag(animatedObjectTag);
            if (!go) { Debug.LogError($"Animated object with tag {tag} not found, timeline cannot be played"); return; }
            var dir = go.GetComponent<PlayableDirector>();
            if (!dir) { Debug.LogError($"PlayableDirector not found on object {go.name} (tag {tag}), timeline cannot be played"); return; }
            dir.playableAsset = timelineAsset;
            dir.Play();
        }

        public void PlayVideo(string videoPlayerTag, string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                Debug.LogError($"Cannot play video on GameObject with tag {videoPlayerTag}, videoUrl is empty!");
                return;
            }
            var go = GameObject.FindGameObjectWithTag(videoPlayerTag);
            if (!go)
            {
                Debug.LogError($"Cannot find VideoPlayer with tag {videoPlayerTag}");
                return;
            }
            var vp = go.GetComponent<VideoPlayer>();
            if (!vp)
            {
                Debug.LogError($"Cannot Play Video with url {videoUrl}, VideoPlayer component not found on GameObject {go.name} with tag {videoPlayerTag}");
                return;
            }
            PlayVideo(vp, videoUrl);
        }

        public void PlayVideo(string videoPlayerTag, VideoClip clip)
        {
            if (clip == null)
            {
                Debug.LogError($"Cannot play video on GameObject with tag {videoPlayerTag}, please assign a Video Clip!");
                return;
            }
            var go = GameObject.FindGameObjectWithTag(videoPlayerTag);
            if (!go)
            {
                Debug.LogError($"Cannot find VideoPlayer with tag {videoPlayerTag}");
                return;
            }
            var vp = go.GetComponent<VideoPlayer>();
            if (!vp)
            {
                Debug.LogError($"Cannot Play VideoClip {clip.name}, VideoPlayer component not found on GameObject with tag {videoPlayerTag}");
                return;
            }
            PlayVideo(vp, clip);
        }

        public void PlayVideo(VideoPlayer videoPlayer, string videoUrl)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoUrl;
            videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
            videoPlayer.Prepare();
        }

        public void PlayVideo(VideoPlayer videoPlayer, VideoClip clip)
        {
            videoPlayer.clip = clip;
            videoPlayer.Play();
        }

        /// <summary>
        /// Plays the Video found at the specified videoUrl.
        /// Looks for the VideoPlayer component in the StateManager.
        /// </summary>
        /// <param name="videoUrl">The url of the video to play</param>
        public void PlayVideo(string videoUrl)
        {
            var videoPlayer = this.GetComponent<VideoPlayer>();
            if (!videoPlayer)
            {
                Debug.LogError($"Cannot play video {videoUrl}, VideoPlayer component not found in StateManager.");
            }
            PlayVideo(videoPlayer, videoUrl);
        }

        /// <summary>
        /// Plays the specified VideoClip.
        /// Looks for the VideoPlayer component in the StateManager.
        /// </summary>
        /// <param name="video">The VideoClip to play</param>
        public void PlayVideo(VideoClip video)
        {
            var videoPlayer = this.GetComponent<VideoPlayer>();
            if (!videoPlayer)
            {
                Debug.LogError($"Cannot play video {video.name}, VideoPlayer component not found in StateManager.");
            }
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = video;
            videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
        }

        private void VideoPlayer_prepareCompleted(VideoPlayer source)
        {
            source.prepareCompleted -= VideoPlayer_prepareCompleted;
            source.Play();
        }

        public void StopVideo(string videoPlayerTag)
        {
            var go = GameObject.FindGameObjectWithTag(videoPlayerTag);
            if (!go)
            {
                Debug.LogError($"Cannot find VideoPlayer with tag {videoPlayerTag}");
                return;
            }
            var vp = go.GetComponent<VideoPlayer>();
            if (!vp)
            {
                Debug.LogError($"Cannot Stop VideoPlayer  component not found on GameObject {go.name} with tag {videoPlayerTag}");
                return;
            }
            vp.Stop();
        }

        #endregion CUTSCENE

        #region OBJECT

        /// <summary>
        /// Activates/Deactivates one or more objects found in the scene through the specified tag
        /// </summary>
        /// <param name="objectToActivateTag">The tag of the objects to activate/deactivate</param>
        /// <param name="activateObject">if the found GameObjects have to be activated or deactivated</param>
        public void ActivateObject(string objectToActivateTag, bool activateObject)
        {
            var gos = FindAllObjectsWithTag(objectToActivateTag);//GameObject.FindGameObjectsWithTag(objectToActivateTag);
            if (gos == null || gos.Length == 0) { Debug.LogWarning($"No GameObject found with tag {objectToActivateTag}"); }
            foreach (var g in gos)
            {
                g.SetActive(activateObject);
            }
        }

        /// <summary>
        /// Finds all the GameObjects present in scene with the specified tag. Also disabled GameObjects are found.
        /// </summary>
        /// <param name="tag">The tag of the GameObjects to find.</param>
        /// <returns></returns>
        private GameObject[] FindAllObjectsWithTag(string tag)
        {
            List<GameObject> gos = new List<GameObject>();
            //Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
            var objs = GameObject.FindObjectsOfType<Transform>(true);

            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].hideFlags == HideFlags.None)
                {
                    if (objs[i].gameObject.CompareTag(tag))
                    {
                        gos.Add(objs[i].gameObject);
                        //Debug.Log($"Found a gameObject with tag {tag}");
                    }
                }
            }
            return gos.ToArray();
        }

        #endregion OBJECT

        #region SCENE_CHANGE

        public void LoadScene(string sceneName)
        {
            SceneLoader.Instance.Load(sceneName);
        }

        public void LoadScene(string sceneName, bool asyncLoad, bool autoLoad)
        {
            Debug.Log($"Loading {sceneName}, asyncLoad {asyncLoad}, autoLoad: {autoLoad}");
            SceneLoader.Instance.asyncLoad = asyncLoad;
            SceneLoader.Instance.autoActivateScene = autoLoad;
            SceneLoader.Instance.Load(sceneName);
        }

        #endregion SCENE_CHANGE
    }
}
