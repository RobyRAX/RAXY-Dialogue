using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
//using RAXY.Core;
using RAXY.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>//, IBridgeable//ISepObject, IBridgeable
    {
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;

        public void RaiseDialogueStarted() => OnDialogueStarted?.Invoke();
        public void RaiseDialogueEnded() => OnDialogueEnded?.Invoke();
        protected override void Awake()
        {
            base.Awake();

            InitDialogueControllerDict();
        }

        [SerializeField]
        List<DialogueControllerBase> dialogueControllers;
        public Dictionary<DialogueType, DialogueControllerBase> DialogueControllerDict;
        void InitDialogueControllerDict()
        {
            DialogueControllerDict = new();
            foreach (var dialogueCont in dialogueControllers)
            {
                DialogueControllerDict.Add(dialogueCont.DialogueType, dialogueCont);
                dialogueCont.gameObject.SetActive(false);
            }
        }

        [Button]
        public void PlayDialogueSO(DialogueSO dialogueSO)
        {
            if (dialogueSO == null)
                return;

            if (DialogueControllerDict.TryGetValue(dialogueSO.dialogueType, out DialogueControllerBase dialogueCont))
            {
                dialogueCont.gameObject.SetActive(true);
                dialogueCont.PlayDialogueSO(dialogueSO);
            }
        }

        [Button]
        public void PrepareDialogueSO(DialogueSO dialogueSO)
        {
            if (dialogueSO == null)
                return;

            if (DialogueControllerDict.TryGetValue(dialogueSO.dialogueType, out DialogueControllerBase dialogueCont))
            {
                dialogueCont.gameObject.SetActive(true);
                dialogueCont.PrepareDialogueSO(dialogueSO);
            }
        }

        public void PlayDialogueSet(int index)
        {
            foreach (var dialogueCont in DialogueControllerDict.Values)
            {
                if (dialogueCont.currentDialogueSO != null)
                    dialogueCont.PlayDialogueSet(index);
            }
        }

        public void PlayDialogueSet(string id)
        {
            foreach (var dialogueCont in DialogueControllerDict.Values)
            {
                if (dialogueCont.currentDialogueSO != null)
                    dialogueCont.PlayDialogueSet(id);
            }
        }

        public void HideDialogue(bool instant = true)
        {
            foreach (var dialogueCont in dialogueControllers)
            {
                if (dialogueCont is DialogueController_MainBase mainDialogue)
                {
                    if (instant)
                        mainDialogue.HideDialogueBar_Instant();
                    else
                        mainDialogue.HideDialogueBar();
                }
            }
        }

        public void ResetNonTimelineDialogueForReplay()
        {
            if (!DialogueControllerDict.TryGetValue(DialogueType.NonTimelineDialogue, out DialogueControllerBase cont))
                return;

            if (cont is DialogueController_MainNonTimeline nonTimeline)
                nonTimeline.ResetForReplay();
        }
    }

    public enum DialogueType
    {
        TimelineDialogue, NonTimelineDialogue
    }
}