using System.Collections.Generic;
using RAXY.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Dialogue
{
    public abstract class DialogueControllerBase : MonoBehaviour
    {
        public virtual DialogueType DialogueType { get; }

        [TitleGroup("Runtime")]
        [HideLabel]
        public DialogueSO currentDialogueSO;

        [TitleGroup("UI")]
        [SerializeField] protected TextTyper dialogueTyper;

        public List<DialogueDataSet> DialogueDataSets => currentDialogueSO.dialogueDataSets;

        [TitleGroup("Status")]
        [ShowInInspector]
        [ReadOnly]
        [HideInEditorMode]
        public DialogueDataSet CurrentDialogueDataSet { get; set; }

        [TitleGroup("Status")]
        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        protected bool isBlockingNext;

        [TitleGroup("Status")]
        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        protected bool IsTyping => dialogueTyper != null && dialogueTyper.IsTyping;

        [TitleGroup("Debug Function")]
        [Button]
        protected virtual void PrepareDialogueSO(DialogueSO dialogueSO)
        {
            currentDialogueSO = dialogueSO;
        }

        [TitleGroup("Debug Function")]
        [Button]
        public void PlayDialogueSO()
        {
            PlayDialogueSO(currentDialogueSO);
        }

        [TitleGroup("Debug Function")]
        [Button]
        public abstract void PlayDialogueSO(DialogueSO dialogueSO);

        public abstract void PlayDialogueSet(int dialogueSetIndex);
        public abstract void PlayDialogueSet(string dialogueSetId);

        public abstract void CompleteDialogueSO();

        [TitleGroup("Debug Function")]
        [Button]
        protected abstract void ShowCurrentDialogueDataToUI();
    }
}