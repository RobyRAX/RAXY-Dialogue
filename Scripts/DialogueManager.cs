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
        // #region ISepObject
        // public GameObject GetGameObject => gameObject;
        //public bool FirstInitDone { get; set; }
        // public int Order { get; set; }
        // public string SepGroup { get; set; }

        // public async UniTask Init()
        // {
        //     InitDialogueControllerDict();
        //     FirstInitDone = true;
        // }
        // #endregion

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
            if (DialogueControllerDict.TryGetValue(dialogueSO.dialogueType, out DialogueControllerBase dialogueCont))
            {
                dialogueCont.gameObject.SetActive(true);
                dialogueCont.PlayDialogueSO(dialogueSO);
            }
        }
    }

    public enum DialogueType
    {
        TimelineDialogue, NonTimelineDialogue
    }
}