using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace RAXY.Dialogue
{
    [Serializable]
    public class DialogueData
    {
        string Label
        {
            get
            {
                if (string.IsNullOrEmpty(speakerName) && string.IsNullOrEmpty(dialogue))
                    return "(Empty)";

                // Limit preview length (for Odin labels, 40–60 chars is good)
                const int maxPreviewLength = 40;

                string preview = dialogue;
                if (!string.IsNullOrEmpty(dialogue))
                {
                    preview = dialogue.Length > maxPreviewLength
                        ? dialogue.Substring(0, maxPreviewLength) + "..."
                        : dialogue;
                }

                if (!string.IsNullOrEmpty(speakerName))
                    return $"{speakerName}: {preview}";
                else
                    return preview;
            }
        }

        public string speakerName;
        public string dialogue;

        [TitleGroup("Dialogue Setting")]
        [SuffixLabel("seconds")]
        [LabelText("Block Next")]
        public float blockNextDuration = 0.33f;

        [TitleGroup("Dialogue Setting")]
        [SuffixLabel("seconds")]
        [LabelText("Auto Next")]
        public float autoNextDuration = 2f;

        [TitleGroup("On Start")]
        public List<PortraitState> portraitsOnStart;

        [TitleGroup("On Start")]
        public List<DialogueActionEntry> actionsOnStart;

        [TitleGroup("On Complete")]
        public List<DialogueActionEntry> actionsOnComplete;
    }

    [Serializable]
    public class DialogueDataSet
    {
        public string setId;

        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Label")]
        public List<DialogueData> dialogueDatas;

#if UNITY_EDITOR
        [FoldoutGroup("Debugging")]
        [ValueDropdown("MarkerIds")]
        [Tooltip("For debugging purpose")]
        [HideInEditorMode]
        public string markerToStart;

        [FoldoutGroup("Debugging")]
        [ValueDropdown("MarkerIds")]
        [Tooltip("For debugging purpose")]
        [HideInEditorMode]
        public string markerToStop;
#endif

        static List<string> MarkerIds => DialogueSO.MarkerIds; 

        [TitleGroup("On Complete")]
        public List<PortraitState> portraitsOnComplete;

        [TitleGroup("On Complete")]
        public List<DialogueActionEntry> actionsOnComplete;      

        [TitleGroup("Status")]
        [FoldoutGroup("Status/Status")]
        [ReadOnly]
        [HideInEditorMode]
        public int currentDialogueIndex;

        [FoldoutGroup("Status/Status")]
        [HideInEditorMode]
        public DialogueData CurrentDialogueData
        {
            get
            {
                return dialogueDatas[currentDialogueIndex];
            }
        }

        [FoldoutGroup("Status/Status")]
        [ShowInInspector]
        [HideInEditorMode]
        public bool IsFinish => currentDialogueIndex >= dialogueDatas.Count - 1;

        [FoldoutGroup("Status/Status")]
        [ShowInInspector]
        [ReadOnly]
        [HideInEditorMode]
        public bool StartActionTriggered { get; set; }

        [FoldoutGroup("Status/Status")]
        [ShowInInspector]
        [ReadOnly]
        [HideInEditorMode]
        public bool CompleteActionTriggered { get; set; }

        public void Reset()
        {
            currentDialogueIndex = 0;
            CompleteActionTriggered = false;
            StartActionTriggered = false;
        }

        public void NextIndex()
        {
            currentDialogueIndex++;
        }
    }
}