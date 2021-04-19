using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace com.gb.statemachine_toolkit.editor
{
    [CustomEditor(typeof(StateBehaviour))]
    [System.Serializable]
    public class StateBehaviourEditor : Editor
    {
        StateBehaviour stateBehaviour;

        #region SERIALIZED_PROPERTIES

        SerializedProperty m_type;

        SerializedProperty m_textTag;
        SerializedProperty m_textToSet;

        SerializedProperty m_dialogueType;
        SerializedProperty m_dialogueId;
        SerializedProperty m_dialogueAnswerIntParam;

        SerializedProperty m_interactionType;
        SerializedProperty m_enableInteraction;
        SerializedProperty m_enableInput;
        SerializedProperty m_enableLook;
        SerializedProperty m_enableMovement;
        SerializedProperty m_interactiveObjectsTag;

        SerializedProperty m_cutsceneType;
        SerializedProperty m_animatedObjectTag;
        SerializedProperty m_animatedObjectTrigger;
        SerializedProperty m_animatedObjectBool;
        SerializedProperty m_animatedObjectBoolValue;
        SerializedProperty m_timelineAsset;
        SerializedProperty m_videoPlayerTag;
        SerializedProperty m_videoUrl;
        SerializedProperty m_videoClip;
        SerializedProperty m_playVideo;

        SerializedProperty m_objectToActivateTag;
        SerializedProperty m_activateObject;

        SerializedProperty m_asyncLoad;
        SerializedProperty m_sceneToLoad;
        SerializedProperty m_autoLoad;

        SerializedProperty m_increaseStage;
        SerializedProperty m_increaseCustomInt;
        SerializedProperty m_customInt;
        SerializedProperty m_transitions;
        SerializedProperty m_timeToWait;

        #endregion SERIALIZED_PROPERTIES

        #region GUI_CONTENTS

        GUIContent m_type_content;

        GUIContent m_textTag_content;
        GUIContent m_textToSet_content;

        GUIContent m_dialogueType_content;
        GUIContent m_dialogueId_content;
        GUIContent m_dialogueAnswerIntParam_content;

        GUIContent m_interactionType_content;
        GUIContent m_enableInteraction_content;
        GUIContent m_enableInput_content;
        GUIContent m_enableLook_content;
        GUIContent m_enableMovement_content;
        GUIContent m_interactiveObjectTag_content;

        GUIContent m_cutsceneType_content;
        GUIContent m_animatedObjectTag_content;
        GUIContent m_animatedObjectTrigger_content;
        GUIContent m_animatedObjectBool_content;
        GUIContent m_animatedObjectBoolValue_content;
        GUIContent m_timelineAsset_content;
        GUIContent m_videoPlayerTag_content;
        GUIContent m_videoUrl_content;
        GUIContent m_videoClip_content;
        GUIContent m_playVideo_content;

        GUIContent m_objectToActivateTag_content;
        GUIContent m_activateObject_content;

        GUIContent m_asyncLoad_content;
        GUIContent m_sceneToLoad_content;
        GUIContent m_autoLoad_content;

        GUIContent m_increaseStage_content;
        GUIContent m_increaseCustomInt_content;
        GUIContent m_customInt_content;

        //GUIContent m_transitions_content;
        //GUIContent m_transitionType_content;
        //GUIContent m_transitionTargetType_content;
        GUIContent addButton_content = new GUIContent("ADD TRANSITION", "Adds a transition to be executed after the Time To Wait has passed.");
        GUIContent removeButton_content = new GUIContent("-", "Remove this transition");

        private static GUILayoutOption addTrnsBtnWidth = GUILayout.Width(120f);
        private static GUILayoutOption removeTrnsBtnWidth = GUILayout.Width(20f);

        GUIContent m_timeToWaitOnEnd_content;

        #endregion GUI_CONTENTS

        #region BOOLS
        bool showTextHelp = false;
        bool showDialogueHelp = false;
        bool showInteractionHelp = false;
        bool showCutsceneHelp = false;
        bool showTimelineHelp = false;
        bool showObjectHelp = false;
        bool showAdvancedTransitions = false;
        bool showAdvancedTransitionsHelp = false;
        #endregion

        GUIStyle helpStyle;

        private void OnEnable()
        {
            if (target == null) return;

            stateBehaviour = (StateBehaviour)target;

            m_type = this.serializedObject.FindProperty("type");

            m_textTag = this.serializedObject.FindProperty("textTag");
            m_textToSet = this.serializedObject.FindProperty("textToSet");

            m_dialogueType = this.serializedObject.FindProperty("dialogueType");
            m_dialogueId = this.serializedObject.FindProperty("dialogueID");
            m_dialogueAnswerIntParam = this.serializedObject.FindProperty("answerIntParameter");

            m_interactionType = this.serializedObject.FindProperty("interactionType");
            m_interactiveObjectsTag = this.serializedObject.FindProperty("interactiveObjectsTag");
            m_enableInteraction = this.serializedObject.FindProperty("enableInteraction");
            m_enableInput = this.serializedObject.FindProperty("enableInput");
            m_enableLook = this.serializedObject.FindProperty("enableLook");
            m_enableMovement = this.serializedObject.FindProperty("enableMovement");

            m_cutsceneType = this.serializedObject.FindProperty("cutsceneType");
            m_animatedObjectTag = this.serializedObject.FindProperty("animatedObjectTag");
            m_animatedObjectTrigger = this.serializedObject.FindProperty("animatedObjectTrigger");
            m_animatedObjectBool = this.serializedObject.FindProperty("animatedObjectBool");
            m_animatedObjectBoolValue = this.serializedObject.FindProperty("animatedObjectBoolValue");
            m_timelineAsset = this.serializedObject.FindProperty("timelineAsset");
            m_videoPlayerTag = this.serializedObject.FindProperty("videoPlayerTag");
            m_videoUrl = this.serializedObject.FindProperty("videoUrl");
            m_videoClip = this.serializedObject.FindProperty("videoClip");
            m_playVideo = this.serializedObject.FindProperty("playVideo");

            m_objectToActivateTag = this.serializedObject.FindProperty("objectToActivateTag");
            m_activateObject = this.serializedObject.FindProperty("activateObject");

            m_asyncLoad = this.serializedObject.FindProperty("asyncLoad");
            m_sceneToLoad = this.serializedObject.FindProperty("sceneToLoad");
            m_autoLoad = this.serializedObject.FindProperty("autoLoad");

            m_increaseStage = this.serializedObject.FindProperty("increaseStage");
            m_increaseCustomInt = this.serializedObject.FindProperty("increaseCustomInt");
            m_customInt = this.serializedObject.FindProperty("customInt");
            m_transitions = this.serializedObject.FindProperty("transitions");
            m_timeToWait = this.serializedObject.FindProperty("timeToWait");

            m_type_content = GetGUIContent<StateBehaviour>(m_type, true);

            m_textTag_content = GetGUIContent<StateBehaviour>(m_textTag, true);
            m_textToSet_content = GetGUIContent<StateBehaviour>(m_textToSet, true);

            m_dialogueType_content = GetGUIContent<StateBehaviour>(m_dialogueType, true);
            m_dialogueId_content = GetGUIContent<StateBehaviour>(m_dialogueId, true);
            m_dialogueAnswerIntParam_content = GetGUIContent<StateBehaviour>(m_dialogueAnswerIntParam, true);

            m_interactionType_content = GetGUIContent<StateBehaviour>(m_interactionType, true);
            m_interactiveObjectTag_content = GetGUIContent<StateBehaviour>(m_interactiveObjectsTag, true);
            m_enableInteraction_content = GetGUIContent<StateBehaviour>(m_enableInteraction, true);
            m_enableInput_content = GetGUIContent<StateBehaviour>(m_enableInput, true);
            m_enableLook_content = GetGUIContent<StateBehaviour>(m_enableLook, true);
            m_enableMovement_content = GetGUIContent<StateBehaviour>(m_enableMovement, true);

            m_cutsceneType_content = GetGUIContent<StateBehaviour>(m_cutsceneType, true);
            m_animatedObjectTag_content = GetGUIContent<StateBehaviour>(m_animatedObjectTag, true);
            m_animatedObjectTrigger_content = GetGUIContent<StateBehaviour>(m_animatedObjectTrigger, true);
            m_animatedObjectBool_content = GetGUIContent<StateBehaviour>(m_animatedObjectBool, true);
            m_animatedObjectBoolValue_content = GetGUIContent<StateBehaviour>(m_animatedObjectBoolValue, true);
            m_timelineAsset_content = GetGUIContent<StateBehaviour>(m_timelineAsset, true);
            m_videoPlayerTag_content = GetGUIContent<StateBehaviour>(m_videoPlayerTag, true);
            m_videoUrl_content = GetGUIContent<StateBehaviour>(m_videoUrl, true);
            m_videoClip_content = GetGUIContent<StateBehaviour>(m_videoClip, true);
            m_playVideo_content = GetGUIContent<StateBehaviour>(m_playVideo, true);

            m_objectToActivateTag_content = GetGUIContent<StateBehaviour>(m_objectToActivateTag, true);
            m_activateObject_content = GetGUIContent<StateBehaviour>(m_activateObject, true);

            m_asyncLoad_content = GetGUIContent<StateBehaviour>(m_asyncLoad, true);
            m_sceneToLoad_content = GetGUIContent<StateBehaviour>(m_sceneToLoad, true);
            m_autoLoad_content = GetGUIContent<StateBehaviour>(m_autoLoad, true);

            m_increaseStage_content = GetGUIContent<StateBehaviour>(m_increaseStage, true);
            m_increaseCustomInt_content = GetGUIContent<StateBehaviour>(m_increaseCustomInt, true);
            m_customInt_content = GetGUIContent<StateBehaviour>(m_customInt, true);
            //m_transitions_content = GetGUIContent<StateBehaviour>(m_transitions, true);

            m_timeToWaitOnEnd_content = GetGUIContent<StateBehaviour>(m_timeToWait, true);
        }

        public override void OnInspectorGUI()
        {
            if (helpStyle == null)
            {
                helpStyle = GUI.skin.label;
                helpStyle.richText = true;
                helpStyle.wordWrap = true;
            }

            this.serializedObject.Update();

            m_type.intValue = (int)(StateBehaviour.StateType)EditorGUILayout.EnumPopup(m_type_content, (StateBehaviour.StateType)m_type.intValue);

            switch (m_type.enumValueIndex)
            {
                // TEXT
                case 0:
                    showTextHelp = EditorGUILayout.Foldout(showTextHelp, "TEXT HELP");
                    if (showTextHelp) DrawHelp(stateBehaviour.textHelpMsg, helpStyle);
                    m_textTag.stringValue = EditorGUILayout.TextField(m_textTag_content, m_textTag.stringValue);
                    m_textToSet.stringValue = EditorGUILayout.TextField(m_textToSet_content, m_textToSet.stringValue);
                    break;

                // DIALOGUE
                case 1:
                    showDialogueHelp = EditorGUILayout.Foldout(showDialogueHelp, "DIALOGUE HELP");
                    if (showDialogueHelp) DrawHelp(stateBehaviour.dialogueHelpMsg, helpStyle);
                    EditorGUILayout.Space();
                    m_dialogueType.intValue = (int)(StateBehaviour.DialogueType)EditorGUILayout.EnumPopup(m_dialogueType_content, (StateBehaviour.DialogueType)m_dialogueType.intValue);
                    m_dialogueId.stringValue = EditorGUILayout.TextField(m_dialogueId_content, m_dialogueId.stringValue);

                    if (m_dialogueType.enumValueIndex == 1)
                        m_dialogueAnswerIntParam.stringValue = EditorGUILayout.TextField(m_dialogueAnswerIntParam_content, m_dialogueAnswerIntParam.stringValue);
                    break;

                // INTERACTION
                case 2:

                    showInteractionHelp = EditorGUILayout.Foldout(showInteractionHelp, "INTERACTION HELP");
                    if (showInteractionHelp) DrawHelp(stateBehaviour.interactionHelp, helpStyle);
                    EditorGUILayout.Space();

                    m_interactionType.intValue = (int)(StateBehaviour.InteractionType)EditorGUILayout.EnumPopup(m_interactionType_content, (StateBehaviour.InteractionType)m_interactionType.intValue);
                    m_interactiveObjectsTag.stringValue = EditorGUILayout.TextField(m_interactiveObjectTag_content, m_interactiveObjectsTag.stringValue);

                    switch (m_interactionType.enumValueIndex)
                    {
                        // ALL
                        case 0:
                            m_enableInteraction.boolValue = EditorGUILayout.Toggle(m_enableInteraction_content, m_enableInteraction.boolValue);
                            break;
                        // INPUT
                        case 1:
                            m_enableInput.boolValue = EditorGUILayout.Toggle(m_enableInput_content, m_enableInput.boolValue);
                            break;
                        // LOOK
                        case 2:
                            m_enableLook.boolValue = EditorGUILayout.Toggle(m_enableLook_content, m_enableLook.boolValue);
                            break;
                        // MOVEMENT
                        case 3:
                            m_enableMovement.boolValue = EditorGUILayout.Toggle(m_enableMovement_content, m_enableMovement.boolValue);
                            break;
                        // MIXED
                        case 4:
                            m_enableInput.boolValue = EditorGUILayout.Toggle(m_enableInput_content, m_enableInput.boolValue);
                            m_enableLook.boolValue = EditorGUILayout.Toggle(m_enableLook_content, m_enableLook.boolValue);
                            m_enableMovement.boolValue = EditorGUILayout.Toggle(m_enableMovement_content, m_enableMovement.boolValue);
                            break;
                        default: break;
                    }

                    break;

                // CUTSCENE
                case 3:
                    showCutsceneHelp = EditorGUILayout.Foldout(showCutsceneHelp, "CUTSCENE HELP");
                    if (showCutsceneHelp) DrawHelp(stateBehaviour.cutsceneHelpMsg, helpStyle);
                    EditorGUILayout.Space();
                    m_cutsceneType.intValue = (int)(StateBehaviour.CutsceneType)EditorGUILayout.EnumPopup(m_cutsceneType_content, (StateBehaviour.CutsceneType)m_cutsceneType.intValue);

                    switch (m_cutsceneType.enumValueIndex)
                    {
                        case 0:
                            m_animatedObjectTag.stringValue = EditorGUILayout.TextField(m_animatedObjectTag_content, m_animatedObjectTag.stringValue);
                            m_animatedObjectTrigger.stringValue = EditorGUILayout.TextField(m_animatedObjectTrigger_content, m_animatedObjectTrigger.stringValue); ;
                            break;
                        case 1:
                            m_animatedObjectTag.stringValue = EditorGUILayout.TextField(m_animatedObjectTag_content, m_animatedObjectTag.stringValue);
                            m_animatedObjectBool.stringValue = EditorGUILayout.TextField(m_animatedObjectBool_content, m_animatedObjectBool.stringValue);
                            m_animatedObjectBoolValue.boolValue = EditorGUILayout.Toggle(m_animatedObjectBoolValue_content, m_animatedObjectBoolValue.boolValue);
                            break;
                        case 2:
                            showTimelineHelp = EditorGUILayout.Foldout(showTimelineHelp, "TIMELINE HELP");
                            if (showTimelineHelp) DrawHelp(stateBehaviour.timelineHelpMsg, helpStyle);
                            m_animatedObjectTag.stringValue = EditorGUILayout.TextField(m_animatedObjectTag_content, m_animatedObjectTag.stringValue);
                            EditorGUILayout.PropertyField(m_timelineAsset, m_timelineAsset_content);
                            break;
                        case 3:
                            m_videoPlayerTag.stringValue = EditorGUILayout.TextField(m_videoPlayerTag_content, m_videoPlayerTag.stringValue);
                            m_videoUrl.stringValue = EditorGUILayout.TextField(m_videoUrl_content, m_videoUrl.stringValue);
                            EditorGUILayout.PropertyField(m_videoClip, m_videoClip_content);
                            m_playVideo.boolValue = EditorGUILayout.Toggle(m_playVideo_content, m_playVideo.boolValue);
                            break;

                        default: break;
                    }
                    break;

                // ACTIVATE/DEACTIVATE OBJECT
                case 4:
                    showObjectHelp = EditorGUILayout.Foldout(showObjectHelp, "OBJECT HELP");
                    if (showObjectHelp) DrawHelp(stateBehaviour.objectHelp, helpStyle);
                    EditorGUILayout.Space();
                    m_objectToActivateTag.stringValue = EditorGUILayout.TextField(m_objectToActivateTag_content, m_objectToActivateTag.stringValue);
                    m_activateObject.boolValue = EditorGUILayout.Toggle(m_activateObject_content, m_activateObject.boolValue);
                    break;

                // SCENE_CHANGE
                case 5:
                    m_sceneToLoad.stringValue = EditorGUILayout.TextField(m_sceneToLoad_content, m_sceneToLoad.stringValue);
                    m_asyncLoad.boolValue = EditorGUILayout.Toggle(m_asyncLoad_content, m_asyncLoad.boolValue);
                    if (m_asyncLoad.boolValue)
                        m_autoLoad.boolValue = EditorGUILayout.Toggle(m_autoLoad_content, m_autoLoad.boolValue);
                    break;

                default: break;
            }

            // if the StateType is SCENE_CHANGE, the transition settings don't make sense, 
            // since the StateManager is scene based and its settings are not carried over to the new scene.
            if (m_type.intValue == 5)
            {
                this.serializedObject.ApplyModifiedProperties();
                return;
            }

            // TRANSITION SETTINGS
            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("TRANSITION SETTINGS", EditorStyles.boldLabel);
                m_increaseStage.boolValue = EditorGUILayout.Toggle(m_increaseStage_content, m_increaseStage.boolValue);
                m_increaseCustomInt.boolValue = EditorGUILayout.Toggle(m_increaseCustomInt_content, m_increaseCustomInt.boolValue);
                if (m_increaseCustomInt.boolValue)
                {
                    EditorGUI.indentLevel++;
                    m_customInt.stringValue = EditorGUILayout.TextField(m_customInt_content, m_customInt.stringValue);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
                showAdvancedTransitions = EditorGUILayout.Foldout(showAdvancedTransitions, "ADVANCED TRANSITIONS");
                if (showAdvancedTransitions)
                {
                    EditorGUI.indentLevel++;
                    showAdvancedTransitionsHelp = EditorGUILayout.Foldout(showAdvancedTransitionsHelp, "HELP");
                    if (showAdvancedTransitionsHelp) DrawHelp(stateBehaviour.transitionsHelp, helpStyle);
                    EditorGUI.indentLevel--;
                    var width = EditorGUIUtility.currentViewWidth;

                    if (m_transitions != null)
                    {
                        if (GUILayout.Button(addButton_content, EditorStyles.miniButton, addTrnsBtnWidth))
                        {
                            // both of these methods copy the last inserted element values
                            //m_transitions.InsertArrayElementAtIndex(m_transitions.arraySize);
                            //m_transitions.arraySize++;

                            // instead of copying values from the last element, add a new element with default values
                            Undo.RecordObject(stateBehaviour, "Added Transition");
                            var list = stateBehaviour.transitions;
                            list.Add(new StateBehaviour.Transition());
                            stateBehaviour.transitions = list;
                        }

                        for (var i = 0; i < m_transitions.arraySize; i++)
                        {
                            DrawTransition(i, width - 38);
                        }
                    }
                }
                EditorGUILayout.Space();
                m_timeToWait.floatValue = EditorGUILayout.FloatField(m_timeToWaitOnEnd_content, m_timeToWait.floatValue);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        #region TRANSITIONS

        private void DrawTransition(int index, float width)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(width));

            var t = m_transitions.GetArrayElementAtIndex(index);
            var t_type = t.FindPropertyRelative("transitionType");
            var t_target_type = t.FindPropertyRelative("transitionTargetType");

            var target_options = t_target_type.enumDisplayNames;
            // check the transition type, INCREASE and DECREASE TransitionTypes only make sense with TargetTypes STAGE and INT
            // so remove them from the shown enum
            if (t_type.intValue == 0 || t_type.intValue == 1)
            {
                var optionsList = new List<string>(target_options);
                optionsList.RemoveAt(optionsList.Count - 1);
                optionsList.RemoveAt(optionsList.Count - 1);
                target_options = optionsList.ToArray();
            }

            EditorGUI.BeginChangeCheck();
            t_type.intValue = (int)(StateBehaviour.TransitionType)EditorGUILayout.EnumPopup((StateBehaviour.TransitionType)t_type.intValue, GUILayout.Width(width / 4));
            if (EditorGUI.EndChangeCheck())
            {
                // reset target type value when the transition type changes
                t_target_type.intValue = 0;
            }
            // show only the filtered options in the Popup
            t_target_type.intValue = EditorGUILayout.Popup((int)t_target_type.intValue, target_options, GUILayout.Width(width / 4));
            // instead of showing all of them
            // t_target_type.intValue = (int)(StateBehaviour.TransitionTargetType)EditorGUILayout.EnumPopup((StateBehaviour.TransitionTargetType)t_target_type.intValue, GUILayout.Width(width / 4));

            var t_stageValue = t.FindPropertyRelative("stageValue");
            var t_intName = t.FindPropertyRelative("intName");
            var t_intValue = t.FindPropertyRelative("intValue");
            var t_triggerName = t.FindPropertyRelative("triggerName");

            var t_boolName = t.FindPropertyRelative("boolName");
            var t_boolValue = t.FindPropertyRelative("boolValue");
            var rightWidth = width / 2 - 20;

            switch (t_target_type.intValue)
            {
                // STAGE
                case 0:
                    if (t_type.intValue == 0 || t_type.intValue == 1)
                        EditorGUILayout.LabelField(string.Empty, GUILayout.Width(rightWidth));
                    else
                    {
                        EditorGUILayout.LabelField("VALUE: ", GUILayout.Width(rightWidth / 4 * 3 - 3));
                        t_stageValue.intValue = EditorGUILayout.IntField(t_stageValue.intValue, GUILayout.ExpandWidth(false), GUILayout.Width(rightWidth / 4));//GUILayout.Width(width / 4));
                    }
                    break;
                // INT
                case 1:
                    if (t_type.intValue == 0 || t_type.intValue == 1)
                    {
                        EditorGUILayout.LabelField("NAME: ", GUILayout.Width(rightWidth / 2 - 3));
                        t_intName.stringValue = EditorGUILayout.TextField(t_intName.stringValue, GUILayout.ExpandWidth(false), GUILayout.Width(rightWidth / 2));
                    }
                    else
                    {
                        EditorGUILayout.LabelField("NAME: ", GUILayout.Width(rightWidth / 4 - 3));
                        t_intName.stringValue = EditorGUILayout.TextField(t_intName.stringValue, GUILayout.ExpandWidth(false), GUILayout.Width(rightWidth / 4 - 3));
                        EditorGUILayout.LabelField("VALUE: ", GUILayout.Width(rightWidth / 4 - 3));
                        t_intValue.intValue = EditorGUILayout.IntField(t_intValue.intValue, GUILayout.ExpandWidth(false), GUILayout.Width(rightWidth / 4));//GUILayout.Width(width / 4));
                    }
                    break;
                // BOOL
                case 2:
                    EditorGUILayout.LabelField("NAME: ", GUILayout.Width(rightWidth / 4 - 3));
                    t_boolName.stringValue = EditorGUILayout.TextField(t_boolName.stringValue, GUILayout.ExpandWidth(false), GUILayout.Width(rightWidth / 4 - 3));
                    EditorGUILayout.LabelField("VALUE: ", GUILayout.Width(rightWidth / 4 - 3));
                    t_boolValue.boolValue = EditorGUILayout.Toggle(t_boolValue.boolValue, GUILayout.ExpandWidth(false), GUILayout.Width(rightWidth / 4));//GUILayout.Width(width / 4));
                    break;
                // TRIGGER
                case 3:
                    EditorGUILayout.LabelField("NAME: ", GUILayout.Width(rightWidth / 2 - 3));
                    t_triggerName.stringValue = EditorGUILayout.TextField(t_triggerName.stringValue, GUILayout.ExpandWidth(false), GUILayout.Width(rightWidth / 2));
                    break;
            }
            if (GUILayout.Button(removeButton_content, removeTrnsBtnWidth, GUILayout.ExpandWidth(false)))
                m_transitions.DeleteArrayElementAtIndex(index);

            EditorGUILayout.EndHorizontal();
        }
        #endregion TRANSITIONS

        private void DrawHelp(string helpString, GUIStyle helpStyle)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                EditorGUILayout.LabelField(helpString, helpStyle);
        }

        #region UTILITIES

        private static GUIContent GetGUIContent<Type>(SerializedProperty serializedProperty, bool inherit)
        {
            GUIContent content = new GUIContent("", "");

            if (serializedProperty == null)
            {
                return content;
            }

            FieldInfo field = typeof(Type).GetField(serializedProperty.name);
            if (null == field)
            {
                return content;
            }

            var tooltip = GetFieldTooltip(field, inherit);

            content = new GUIContent(serializedProperty.displayName, tooltip);
            return content;
        }

        private static string GetFieldTooltip(FieldInfo field, bool inherit)
        {
            TooltipAttribute[] attributes
                 = field.GetCustomAttributes(typeof(TooltipAttribute), inherit)
                 as TooltipAttribute[];

            string ret = "";
            if (attributes.Length > 0)
                ret = attributes[0].tooltip;

            return ret;
        }

        #endregion UTILITIES
    }
}
