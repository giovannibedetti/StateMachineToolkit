using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace com.gb.statemachine_toolkit
{
    public class StateBehaviour : StateMachineBehaviour
    {
        StateManager stateManager;

        #region ENUMS
        /// <summary>
        /// The various types a State can assume
        /// </summary>
        public enum StateType { TEXT, DIALOGUE, INTERACTION, CUTSCENE, OBJECT, SCENE_CHANGE }
        public StateType type;

        /// <summary>
        /// The types a Dialogue State can assume
        /// </summary>
        public enum DialogueType { SEQUENCE, QUESTION }
        public DialogueType dialogueType;

        /// <summary>
        /// The types a Cutscene State can assume
        /// </summary>
        public enum CutsceneType { TRIGGER, BOOL, TIMELINE, VIDEO }
        public CutsceneType cutsceneType;

        public enum InteractionType { ALL, INPUT, LOOK, MOVEMENT, MIXED }
        [Tooltip("The type of the Interaction to enable/disable. " +
            "\n\"ALL\" will enable/disable all the interaction scripts, " +
            "\n\"INPUT\" will enable/disable the script that handles clicks on InteractiveObjects, " +
            "\n\"MOVEMENT\" will enable/disable the script that handles the player movement, " +
            "\n\"LOOK\" will enable/disable the script that handles the camera rotation driven by mouse, " +
            "\n\"MIXED\" allows the user to choose which script to enable/disable. ")]
        public InteractionType interactionType;

        public enum TransitionType { INCREASE, DECREASE, SET }
        public enum TransitionTargetType { STAGE, INT, BOOL, TRIGGER }

        #endregion ENUMS

        #region TEXT
        [Tooltip("The tag of the GameObject with a Text or TextMeshProUGUI component that will be updated with the Text To Set content.")]
        public string textTag;
        [Tooltip("The text to set")]
        public string textToSet;

        #endregion TEXT

        #region DIALOGUE
        [Tooltip("The ID of the dialogue texts to be loaded from copy.txt")]
        public string dialogueID;
        [Tooltip("The Int parameter name of the StateManager animator that will be used to save the choosen answer. " +
            "This value will be set to a value from 0 to (number of questions - 1) when the user chooses an answer from the shown list, but only after waiting for \"Time To Wait\".")]
        public string answerIntParameter;
        #endregion DIALOGUE

        #region INTERACTION
        [Tooltip("If the interaction should be enabled or disabled. " +
            "If this is selected all the interactions will be enabled, and disabled otherwise.")]
        public bool enableInteraction = true;
        [Tooltip("If the input should be enabled or disabled, so that the player is able or not to click on the InteractiveObjects present in the scene.")]
        public bool enableInput = true;
        [Tooltip("If the movement should be enabled or disabled.")]
        public bool enableMovement = true;
        [Tooltip("If the mouse look should be enabled or disabled")]
        public bool enableLook = true;
        [Tooltip("The tag of the InteractiveObjects that will be enabled in this state")]
        public string interactiveObjectsTag;
        #endregion INTERACTION

        #region CUTSCENE
        [Tooltip("The tag of the GameObject to animate")]
        public string animatedObjectTag;
        [Tooltip("The name of the trigger parameter to set on the animator of the GameObject to animate")]
        public string animatedObjectTrigger;
        [Tooltip("The name of the bool parameter to set on the animator")]
        public string animatedObjectBool;
        [Tooltip("The bool value of the above bool parameter to set ")]
        public bool animatedObjectBoolValue;
        [Tooltip("The timeline to be played by the PlayableDirector found in the GameObject with the specified \"Animated Object Tag\"")]
        public TimelineAsset timelineAsset;
        [Tooltip("The tag of the gameObject containing the VideoPlayer component. If not specified, the VideoPlayer component will be searched in the StateManager GameObject.")]
        public string videoPlayerTag;
        [Tooltip("The videoPlayer renderTexture. If not specified, the renderTexture assigned to the found VideoPlayer will be used. Be sure to check the Render Mode of the VideoPlayer component.")]
        public RenderTexture videoRenderTexture;
        [Tooltip("The URL of the video to be played. If not specified, will try to use the VideoClip assigned to the found VideoPlayer as a Source.")]
        public string videoUrl;
        [Tooltip("The VideoClip to be played. If both this and the videoUrl are specified, the videoPlayer will try to play the VideoClip.")]
        public VideoClip videoClip;
        [Tooltip("If the video should be played or stopped.")]
        public bool playVideo;

        #endregion CUTSCENE

        #region OBJECT
        [Tooltip("The Tag of the GameObject to activate/deactivate")]
        public string objectToActivateTag;
        [Tooltip("If the GameObject with the above Tag should be activated or deactivated")]
        public bool activateObject;
        #endregion OBJECT

        #region SCENE_CHANGE
        [Tooltip("If the scene should be loaded asynchronously or not")]
        public bool asyncLoad;
        [Tooltip("The scene to be loaded")]
        public string sceneToLoad;
        [Tooltip("If selected, the user has to click a button to activate a scene, otherwise the new scene is auto loaded as soon as it's ready.")]
        public bool autoLoad;
        #endregion SCENE_CHANGE
        
        #region TRANSITION

        [Tooltip("If selected it increases by one the \"Stage\" parameter in the StateManager animator, " +
            "after waiting for the time specified in \"Time To Wait\"")]
        public bool increaseStage = true;
        [Tooltip("If selected it increases by one the specified integer parameter in the StateManager animator, " +
            "after waiting for the time specified in \"Time To Wait\"")]
        public bool increaseCustomInt = false;
        [Tooltip("The custom Int parameter that is increased at the end of this state, if \"Increase Custom Int\" is true")]
        public string customInt;
        // TODO [Tooltip("If the wait (and the carried actions) should be interrupted before completing them when this state is exited ")]
        // TODO public bool clearActionsOnInterruption = true;

        public List<Transition> transitions = new List<Transition>();

        [Tooltip("The time in seconds to wait before completing the state." +
            "\nIf \"Increase Stage\" is selected, it will increase the Stage parameter in StateManager animator." +
            "\nIf \"Increase Custom Int\" is selected, it will increase a custom integer parameter." +
            "\n\"Go To Next Stage\" and \"Set Custom Int\" can be used together too, but they will use the same waiting time." +
            "\nIn the case of a Dialogue Question, the \"Answer Int Parameter\" will be set after the wait." +
            "\nAlso the Advanced Transitions will be executed after the wait. ")]
        public float timeToWait = 0f;

        /// <summary>
        /// A list to hold the coroutines started by this StateBehaviour, to be able to stop them in case the state is interrupted/exited before completing them
        /// </summary>
        private List<Coroutine> _waitingRoutines = new List<Coroutine>();

        #endregion TRANSITION

        #region HELP_MSG

        public readonly string textHelpMsg = "The <b>TEXT STATE</b> is used to update the UI Text with the specified <i>Text Tag</i> with the provided <i>Text To Set</i>. " +
            "It works with Text and TextMeshPro components. If multiple gameObjects are found with the specified Tag, all of them are updated.";

        public readonly string dialogueHelpMsg = "The <b>DIALOGUE STATE</b> is used to display a UI dialogue with a sequence of texts to the user, or to display a question with several answers." +
            "\n<color=orange>It automatically disables all interactions (Player movement, mouseLook, and clicks) to be able to click in the UI.</color>" +
            "\n\nIt retrieves the texts at the specified <i>Dialogue ID</i> in the txt file specified in the Copy component. " +
            "\nTo be able to load the copy file, a Copy component must be present in the scene, and the <i>File Name</i> (with extension) must be specified. " +
            "\nThis file can reside inside the <i>Resources</i> folder, or can be placed inside the Build folder " +
            "(in this way it is possible to update all the texts without rebuilding the application)." +
            "\n\nWhen showing a <b>SEQUENCE</b> of texts, pressing the <i>Next Button</i> of the <i>Dialogue</i> component will switch to the next text of the sequence, and after the last text the dialogue will be closed." +
            "\n\nWhen showing a <b>QUESTION</b> with answers, clicking an answer will close the dialogue. " +
            "\nAfter the wait specified in <i>Time To Wait</i> it will set the specified <i>Answer Int Parameter</i> in the StateManager animator." +
            "\n\nWhen the dialogue is being closed, it can do specific actions:" +
            "\n- If <i>Increase Stage</i> is selected, it will increase by one the Stage in the State Manager animator, after waiting for <i>Time To Wait On End</i> seconds." +
            "\n- If <i>Increase Custom Int</i> is selected, it will increase by one the specified Integer parameter in the StateManager animator, after waiting for <i>Time To Wait On End</i> seconds." +
            "\n\n<b>TO SETUP A DIALOGUE</b>:\n- place the \"<b>Dialogue</b>\" prefab inside a Canvas in the scene." +
            "\n- Select the StateManager and assign the scene dialogue to it." +
            "\nIf no dialogue is found, nothing is displayed. " +
            "\nYou can customise the Dialogue as you want." +
            "\n\n<b>DIALOGUE TYPE: SEQUENCE</b>" +
            "\nIt will display all the texts found at the line with the specified <i>Dialogue ID</i> in <i>copy.txt</i>, one after the other, by pressing the next button." +
            "\n\n<b>DIALOGUE TYPE: QUESTION</b>" +
            "\nIt uses all the texts found at the line with the specified Dialogue ID in <i>Resources/Copy.txt</i>" +
            "\nIt needs at least two texts, one for the question, and one for the answer." +
            "\nThe first text found will be the question, all the other texts are the possible answers." +
            "\n\n<b>TO CREATE YOUR COPY:</b>" +
            "\n- Use the <i>NTA Copy Template</i> Google Sheet to create your copy file." +
            "\n- Add as many lines you need, with a unique identifier (ID) for each line. " +
            "\n- Download the file as .tsv (table separated values), and import it inside the \"Resources\" folder, or inside the Data folder of the application (at the same level of the Assets folder)." +
            "\n- Change the extension to \".txt\"" +
            "\nIf no copy file is found, or no ID is found, or there are no texts at the specified ID, the Dialogue won't be displayed" +
            "\nIf an ID is not unique its texts will be discarded." +
            "\nFor each line in the copy file, the first cell is the ID of a group of texts, the next cells are the texts to be displayed";

        public readonly string cutsceneHelpMsg = "The <b>CUTSCENE STATE</b> is used to manage animator parameters (so that it can play animations), play a timeline or a video." +
            "\nAlthough a Cutscene is usually a non interactive sequence in video games, <color=orange>the <b>CUTSCENE STATE</b> DOESN'T DISABLE the player interaction when entered</color>, " +
            "so this must be done in a previous state if needed." +
            "\nThis is to give more freedom to the user, and have the possibility to animate something also when the player could interact with it." +
            "\n\nIt tries to find the object to animate with the TAG specified in <i>Animated Object Tag</i> and then acts based on the selected <i>CUTSCENE TYPE</i>:" +
            "\n\n- Selecting <b>TRIGGER</b>, it will set the specified trigger in the StateManager animator." +
            "\n\n- Selecting <b>BOOL</b>, it will set the specified bool and its value in the StateManager animator." +
            "\n\n- Selecting <b>TIMELINE</b>, it will play the specified <i>Timeline Asset</i> in the PlayableDirector component found on the <i>Animated Object Tag</i>" +
            "\n\n- Selecting <b>VIDEO</b>, it will load and play the specified video from an URL or from a VideoClip, using the VideoPlayer found at <i>Video Player Tag</i>" +
            "\n\n<b><color=cyan>PRO TIP</color></b>: the <b>TRIGGER</b> and <b>BOOL</b> types can be used to change the flow of a StateMachine!";

        public readonly string timelineHelpMsg = "<b>HOW TO CREATE A TIMELINE</b>:" +
            "\n- The timeline must be prepared beforehand to be able to use it: select a GameObject and add the <i>Playable Director</i> component to it." +
            "\n- Set a Tag for the GameObject to be able to retrieve it later." +
            "\n- Open the <i>Timeline</i> tab from <i>Window/Sequencing</i>. The timeline supports several <i>Track Types</i>. " +
            "We will focus on three of them: <b><i>Activation</i></b>, <b><i>Animation</i></b>, <b><i>Audio</i></b>." +
            "\n\nThe <b><i>Activation</i></b> track is used to Activate/Deactivate a GameObject. " +
            "\n- To create an Activation track, drag the GameObject you want to activate/deactivate in the left section of the <i>Timeline</i> tab, and select Activation in the Popup that appears; " +
            "or press the \"<b>+</b>\" in the top left of the Timeline tab and select Activation Track: in this way you'll have to specify the GameObject you want to activate/deactivate." +
            "\nWhen the cursor is in the \"Active\" zone, the GameObject is active, and it is disabled when the cursor is out of it." +
            "\nYou can choose any GameObject present in the scene, the GameObject holding the Playable Director can be animated too." +
            "\n\nThe <b><i>Animation</i></b> track is used to Animate a GameObject. " +
            "\n- Drag the object you want to animate in the left section of the <i>Timeline</i> tab, and select <i>Add Animation Track</i>, " +
            "or press the \"<b>+</b>\" in the top left of the Timeline tab and select <i>Add Animation Track</i>: in this way you'll have to specify the GameObject you want to animate." +
            "If no animator component is present in the choosen GameObject, it will be added automatically." +
            "\n- Press the Red Record Button in the added Animation Track, and set the parameters you want to animate on the selected GameObject at specific frames. " +
            "\nBy pressing the \"Show Curves View\" button beside the Record button, you can edit the curves and fine tune your animation." +
            "\nYou can also drag AnimationClips inside the TimeLine, and cross fade between them dragging one above another." +
            "\n\nThe <b><i>Audio</i></b> track is used to set and play AudioClips of an AudioSource present in the scene." +
            "\n- Drag the AudioSource you want to use in the left section of the <i>Timeline</i> tab, and select <i>Add Audio Track</i>, " +
            "or press the \"<b>+</b>\" in the top left of the Timeline tab and select <i>Add Audio Track</i>: in this way you'll have to specify the GameObject with the AudioSource." +
            "\n- Add as many AudioClips you want in the Timeline, at the keyframe where you want them to be played. " +
            "By default they are faded in/out: if you don't want this, select the AudioClip in the Timeline, set the Blend Curves to Manual and edit them to your likings." +
            "\nYou can select the \"None\" Blend Curve Type from the options at the bottom of the Inspector" +
            "\nIf you drag an AudioClip above another, it will crossfade between the two." +
            "\n\nRefer to the TimeLine docs for more information.";

        public readonly string interactionHelp = "The <b>INTERACTION STATE</b> is used to activate/deactivate the <b><i>Input</i></b>, <b><i>Movement</i></b> and <b><i>MouseLook</i></b> of the Player." +
            "\n\nBased on the chosen <b><i>Interaction Type</i></b>, they can be controlled together or selectively:" +
            "\n\nIf <b>ALL</b> is selected, all the interactions are activated/deactivated based on the <b>Enable Interaction</b> toggle. " +
            "\n\nIf <b>INPUT</b> is selected, just the Input is activated/deactivated." +
            "\n\nIf <b>MOVEMENT</b> is selected, just the Movement is activated/deactivated." +
            "\n\nIf <b>LOOK</b> is selected, just the MouseLook is activated/deactivated." +
            "\n\nIf <b>MIXED</b> is selected, they can be selectively activated/deactivated." +
            "\n\nIf the <b><i>Interactive Objects Tag</i></b> is specified, it will enable/disable the collider of all the GameObjects found with that Tag. " +
            "\nFor the interaction to work, each GameObject needs an InteractiveObject component, and to be on the same Layer specified in the InputController component.";

        public readonly string objectHelp = "The <b>OBJECT STATE</b> is used to manage the activation/deactivation of a GameObject present in the Scene." +
            "\n\nSet a tag on the target GameObject and write the tag in the <i>Object To Activate Tag</i>. " +
            "\nIf <i>Activate Object</i> is selected, the GameObject will be activated, and deactivated otherwise.";

        public readonly string sceneHelp = "The <b>SCENE_CHANGE STATE</b> is used to load a new scene. Just set the name of the scene to load in <i>Scene To Load</i> field." +
            "\nNo transition settings are available since the StateManager is scene based, and the changes to parameter aren't carried over into the new scene.";
        public readonly string transitionsHelp = "The <b>Advanced Transitions</b> can be used to Increase, Decrease or Set the specified STAGE, INT, BOOL or TRIGGER parameters, so that the the flow of the StateMachine can be modified.";
        #endregion HELP_MSG

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log($"{Time.time} Entering state {stateInfo.fullPathHash} {name}");
            stateManager = animator.GetComponent<StateManager>();
            if (!stateManager) return;

            Action nextAction = null;
            if (increaseStage || increaseCustomInt)
            {
                nextAction = () => _waitingRoutines.Add(Utilities.WaitThenAct(stateManager, timeToWait, () =>
                {
                    if (increaseStage) stateManager.NextStage();
                    if (increaseCustomInt) stateManager.IncreaseInt(customInt);
                }));
            }

            Action transitionAction = null;

            foreach (var t in transitions)
            {
                transitionAction += () =>
                {
                    switch (t.transitionTargetType)
                    {
                        case TransitionTargetType.STAGE:
                            switch (t.transitionType)
                            {
                                case TransitionType.INCREASE:
                                    stateManager.NextStage();
                                    break;
                                case TransitionType.DECREASE:
                                    stateManager.DecreaseInt("Stage");
                                    break;
                                case TransitionType.SET:
                                    stateManager.SetInt("Stage", t.intValue);
                                    break;
                            }
                            break;
                        case TransitionTargetType.INT:
                            switch (t.transitionType)
                            {
                                case TransitionType.INCREASE:
                                    stateManager.IncreaseInt(t.intName);
                                    break;
                                case TransitionType.DECREASE:
                                    stateManager.DecreaseInt(t.intName);
                                    break;
                                case TransitionType.SET:
                                    stateManager.SetInt(t.intName, t.intValue);
                                    break;
                            }
                            break;
                        case TransitionTargetType.BOOL:
                            switch (t.transitionType)
                            {
                                case TransitionType.INCREASE:
                                case TransitionType.DECREASE:
                                    break;
                                case TransitionType.SET:

                                    stateManager.SetBool(t.boolName, t.boolValue);
                                    break;
                            }
                            break;
                        case TransitionTargetType.TRIGGER:
                            switch (t.transitionType)
                            {
                                case TransitionType.INCREASE:
                                case TransitionType.DECREASE:
                                    break;
                                case TransitionType.SET:
                                    stateManager.SetTrigger(t.triggerName);
                                    break;
                            }
                            break;
                    }
                };
            }

            switch (type)
            {
                case StateType.TEXT:
                    stateManager.SetText(textTag, textToSet);
                    break;

                case StateType.DIALOGUE:
                    // The dialogue automatically disables interaction (to be able to click on the canvas)
                    stateManager.EnableInteraction(false);

                    switch (dialogueType)
                    {
                        case DialogueType.SEQUENCE:
                            stateManager.OpenDialogue(dialogueID, nextAction);
                            break;
                        case DialogueType.QUESTION:
                            stateManager.OpenQuestion(dialogueID, answerIntParameter, timeToWait, nextAction);
                            break;
                        default:
                            break;
                    }
                    break;

                case StateType.INTERACTION:

                    switch (interactionType)
                    {
                        case InteractionType.ALL:
                            stateManager.EnableInteraction(enableInteraction);
                            break;
                        case InteractionType.INPUT:
                            stateManager.EnableInput(enableInput);
                            break;
                        case InteractionType.LOOK:
                            stateManager.EnableMouseLook(enableLook);
                            break;
                        case InteractionType.MOVEMENT:
                            stateManager.EnableMovement(enableMovement);
                            break;
                        case InteractionType.MIXED:
                            stateManager.EnableInput(enableInput);
                            stateManager.EnableMouseLook(enableLook);
                            stateManager.EnableMovement(enableMovement);
                            break;
                        default: break;
                    }
                    if (!string.IsNullOrWhiteSpace(interactiveObjectsTag))
                        stateManager.EnableInteractiveObjects(interactiveObjectsTag, true);
                    break;

                case StateType.CUTSCENE:
                    switch (cutsceneType)
                    {
                        case CutsceneType.TRIGGER:
                            stateManager.AnimateTrigger(animatedObjectTag, animatedObjectTrigger);
                            break;
                        case CutsceneType.BOOL:
                            stateManager.AnimateBool(animatedObjectTag, animatedObjectBool, animatedObjectBoolValue);
                            break;
                        case CutsceneType.TIMELINE:
                            stateManager.PlayTimeline(animatedObjectTag, timelineAsset);
                            break;
                        case CutsceneType.VIDEO:
                            if (playVideo)
                            {
                                if (videoClip)
                                    stateManager.PlayVideo(videoPlayerTag, videoClip);
                                else
                                    stateManager.PlayVideo(videoPlayerTag, videoUrl);
                            }
                            else
                            {
                                stateManager.StopVideo(videoPlayerTag);
                            }
                            break;
                    }
                    break;

                case StateType.OBJECT:
                    stateManager.ActivateObject(objectToActivateTag, activateObject);
                    break;

                case StateType.SCENE_CHANGE:
                    stateManager.LoadScene(sceneToLoad, asyncLoad, autoLoad);
                    break;
                default:
                    break;
            }

            // simple transition actions are called for every state except:
            // - dialogue, that calls the nextAction after the text has been read, or an answer has been choosen
            // - scene, since transitioning to the new scene could lead to missing references
            if (type != StateType.DIALOGUE && type != StateType.SCENE_CHANGE)
            {
                if (increaseStage || increaseCustomInt) nextAction();
            }

            // advanced transition actions
            if (transitions != null && transitions.Count > 0)
            {
                Debug.Log($"{Time.time} found {transitions.Count} transitions, started waiting {timeToWait} before calling their actions");
                _waitingRoutines.Add(Utilities.WaitThenAct(stateManager, timeToWait, transitionAction));
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // TODO if this is false, avoid resetting the wait routines
            // if (!clearActionsOnInterruption) return;

            // stop the existing waiting routines when the state is exited
            // if execution of this state wasn't interrupted, they should be already null
            // if execution was interrupted, they could be still active, so stop them
            if (stateManager != null && _waitingRoutines != null && _waitingRoutines.Count > 0)
            {
                foreach (var routine in _waitingRoutines)
                    if (routine != null) stateManager.StopCoroutine(routine);
                _waitingRoutines.Clear();
            }
        }

        [Serializable]
        public class Transition
        {
            [SerializeField] public TransitionType transitionType;
            [SerializeField] public TransitionTargetType transitionTargetType;
            [SerializeField] public int stageValue;
            [SerializeField] public string intName;
            [SerializeField] public int intValue;
            [SerializeField] public string boolName;
            [SerializeField] public bool boolValue;
            [SerializeField] public string triggerName;
        }
    }
}
