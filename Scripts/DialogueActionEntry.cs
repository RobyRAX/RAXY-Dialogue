using System;
using System.Collections.Generic;
using RAXY.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace RAXY.Dialogue
{
    [Serializable]
    public class DialogueActionEntry
    {
        public DialogueActionType actionType;

        [ShowIf("@actionType == DialogueActionType.HideDialogueBar")]
        [HideLabel]
        [FormerlySerializedAs("hideDialgue_Param")]
        public HideDialogueBar_ActionParam hideDialogue_Param;

        [ShowIf("@actionType == DialogueActionType.PlayTimeline")]
        [HideLabel]
        public PlayTimeline_ActionParam playTimeline_Param;

        [ShowIf("@actionType == DialogueActionType.RaiseEvent")]
        [HideLabel]
        public RaiseEvent_ActionParam raiseEvent_Param;

        [ShowIf("@actionType == DialogueActionType.ShowOptions")]
        [HideLabel]
        public ShowOptions_ActionParam showOptions_Param;

        [ShowIf("@actionType == DialogueActionType.PlayDialogueSet")]
        [HideLabel]
        public PlayDialogueSet_ActionParam playDialogueSet_Param;
    }

    [Serializable]
    public class HideDialogueBar_ActionParam
    {
        public bool instant;
    }

    [Serializable]
    public class PlayTimeline_ActionParam
    {
        List<string> Ids => DialogueSO.MarkerIds;
        [ValueDropdown("Ids")]
        public string startFrameMarker;
        [ValueDropdown("Ids")]
        public string endFrameMarker;
    }

    [Serializable]
    public class RaiseEvent_ActionParam
    {
        public List<PrimitiveEventRaiser> events;
    }

    [Serializable]
    public class ShowOptions_ActionParam
    {
        public string optionSetId;
    }

    [Serializable]
    public class PlayDialogueSet_ActionParam
    {
        public string dialogueSetId;
    }

    public enum DialogueActionType
    {
        None,
        HideDialogueBar,
        PlayTimeline,
        RaiseEvent,
        CompleteDialogue,
        ShowOptions,
        PlayDialogueSet
    }
}
