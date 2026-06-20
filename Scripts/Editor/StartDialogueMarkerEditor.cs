#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace RAXY.Dialogue
{
    [CustomEditor(typeof(StartDialogueMarker))]
    public class StartDialogueMarkerEditor : Editor
    {
        SerializedProperty inputMethodProp;
        SerializedProperty dialogueSetIdProp;
        SerializedProperty dialogueSetIndexProp;

        void OnEnable()
        {
            inputMethodProp = serializedObject.FindProperty("inputMethod");
            dialogueSetIdProp = serializedObject.FindProperty("dialogueSetId");
            dialogueSetIndexProp = serializedObject.FindProperty("dialogueSetIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Start Dialogue Marker", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            // Draw enum
            EditorGUILayout.PropertyField(inputMethodProp);

            // Draw fields based on enum selection
            var method = (StartDialogueMarker.InputMethod)inputMethodProp.enumValueIndex;

            EditorGUI.indentLevel++;
            if (method == StartDialogueMarker.InputMethod.Id)
            {
                EditorGUILayout.PropertyField(dialogueSetIdProp, new GUIContent("Dialogue Set ID"));
            }
            else if (method == StartDialogueMarker.InputMethod.Index)
            {
                EditorGUILayout.PropertyField(dialogueSetIndexProp, new GUIContent("Dialogue Set Index"));
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox("Triggers a dialogue set when Timeline reaches this marker.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif