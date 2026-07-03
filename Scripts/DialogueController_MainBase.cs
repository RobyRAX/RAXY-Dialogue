using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RAXY.Event;
using RAXY.Utility;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RAXY.Dialogue
{
    public abstract class DialogueController_MainBase : DialogueControllerBase
    {
        [TitleGroup("Tween Setting")]
        [SerializeField] protected float portraitOpacityTweenTime = 0.2f;

        [TitleGroup("Tween Setting")]
        [SerializeField] protected float playDialogueFadeInTime = 0.33f;

        [TitleGroup("Events")]
        [SerializeField] protected StringEventSO selectOptionEvent;

        [TitleGroup("Prefab")]
        [SerializeField] protected OptionController optionUIPrefab;

        [TitleGroup("UI")]
        [SerializeField] protected Button nextBtn;

        [TitleGroup("UI")]
        [FormerlySerializedAs("speakerNameTxt")]
        [SerializeField] protected TextMeshProUGUI speakerNameTmp;

        [TitleGroup("UI")]
        [SerializeField] protected CanvasGroup dialogueParentCG;

        [TitleGroup("UI")]
        [SerializeField] protected CanvasGroup dialogueBarCG;

        [TitleGroup("UI")]
        [SerializeField] protected Transform portraitParent;

        [TitleGroup("UI")]
        [SerializeField] protected Transform optionParent;

        [TitleGroup("Runtime")]
        [ShowInInspector]
        public Dictionary<string, DialoguePortrait> PortraitDict;

        protected virtual void Start()
        {
            nextBtn.onClick.RemoveListener(NextDialogueData);
            nextBtn.onClick.AddListener(NextDialogueData);
        }

        public override void PrepareDialogueSO(DialogueSO dialogueSO)
        {
            base.PrepareDialogueSO(dialogueSO);

            SpawnDialoguePortraits();
            ApplyPortraitData(currentDialogueSO.portraitsOnStart);
        }

        protected void SpawnDialoguePortraits()
        {
            // Clear dictionary
            PortraitDict = new();

            if (currentDialogueSO == null || currentDialogueSO.actors == null)
                return;

            if (portraitParent == null)
                return;

            // --- Destroy previous portraits safely ---
            var toDestroy = new List<GameObject>();
            foreach (Transform child in portraitParent)
                toDestroy.Add(child.gameObject);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (var go in toDestroy)
                    Object.DestroyImmediate(go);
            }
            else
#endif
            {
                foreach (var go in toDestroy)
                    Object.Destroy(go);
            }

            int index = 0;
            // --- Spawn new portraits ---
            foreach (var actorSO in currentDialogueSO.actors)
            {
                if (actorSO == null)
                    continue;

                if (actorSO.dialoguePortraitProvider == null || actorSO.dialoguePortraitProvider.Asset == null)
                    continue;

                GameObject newPortraitObj;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    // Editor mode — keep prefab connection
                    newPortraitObj = PrefabUtility.InstantiatePrefab(
                                        actorSO.dialoguePortraitProvider.Asset, 
                                        portraitParent) as GameObject;
                }
                else
#endif
                {
                    // Runtime — normal instantiate
                    newPortraitObj = Instantiate(actorSO.dialoguePortraitProvider.Asset, portraitParent);
                }

                if (newPortraitObj == null)
                    continue;

                var portraitComp = newPortraitObj.GetComponent<DialoguePortrait>();
                if (portraitComp == null)
                    continue;

                portraitComp.PortraitRuntimeIndex = index;
                PortraitDict.Add(actorSO.actorId, portraitComp);
                index++;
            }
        }

        void Process_DialogueData_OnStart(DialogueData dialogueData)
        {
            ApplyPortraitData(dialogueData.portraitsOnStart);
            Process_DialogueActions(dialogueData.actionsOnStart);
        }

        protected override void ShowCurrentDialogueDataToUI()
        {
            if (CurrentDialogueDataSet == null)
                return;

            var currentDialogue = CurrentDialogueDataSet.CurrentDialogueData;

            BlockNext(currentDialogue.blockNextDuration);
            Process_DialogueData_OnStart(currentDialogue);

            speakerNameTmp.text = currentDialogue.GetSpeakerName(currentDialogueSO.actors);
            dialogueTyper.StartTyping(currentDialogue.dialogue);
        }

        public void BlockNext(float duration)
        {
            if (blockNextCoroutine != null)
                StopCoroutine(blockNextCoroutine);

            blockNextCoroutine = StartCoroutine(BlockNextCo(duration));
        }

        private Coroutine blockNextCoroutine;
        private IEnumerator BlockNextCo(float duration)
        {
            isBlockingNext = true;
            yield return new WaitForSeconds(duration);
            isBlockingNext = false;
            blockNextCoroutine = null; // clear reference when done
        }

        protected void StopBlockNextCoroutine()
        {
            isBlockingNext = false;
            if (blockNextCoroutine == null)
                return;

            StopCoroutine(blockNextCoroutine);
            blockNextCoroutine = null;
        }

        [HorizontalGroup("Debug Function/OP")]
        [Button]
        public void NextDialogueData()
        {
            if (isBlockingNext)
                return;

            if (CurrentDialogueDataSet == null)
                return;

            if (IsTyping)
            {
                dialogueTyper.ShowAllInstant();
                return;
            }

            if (CurrentDialogueDataSet.IsFinish)
            {
                Process_DialogueDataSet_OnComplete(CurrentDialogueDataSet);
                return;
            }

            CurrentDialogueDataSet.NextIndex();
            ShowCurrentDialogueDataToUI();
        }

        [TitleGroup("Debug Function")]
        [HorizontalGroup("Debug Function/ToggleBar")]
        [Button]
        public void ShowDialogueBar() => ShowDialogueBar(0.2f, null);
        public void ShowDialogueBar(Action OnComplete) => ShowDialogueBar(0.2f, OnComplete);
        public void ShowDialogueBar(float inDuration, Action OnComplete)
        {
            dialogueBarCG.DOFade(1, inDuration).OnComplete(() => OnComplete?.Invoke());
        }

        public override void PlayDialogueSO(DialogueSO dialogueSO)
        {
            gameObject.SetActive(true);

            PrepareDialogueSO(dialogueSO);
            ResetUI();
            HideDialogueBar_Instant();

            foreach (var dialogueDataSet in DialogueDataSets)
            {
                dialogueDataSet.Reset();
            }

            DialogueManager.Instance?.RaiseDialogueStarted();
        }

        public override void CompleteDialogueSO()
        {
            dialogueParentCG.DOFade(0, 0.2f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                DialogueManager.Instance?.RaiseDialogueEnded();
            });
        }

        public override void PlayDialogueSet(int dialogueSetIndex)
        {
            ShowDialogueBar(() =>
            {
                CurrentDialogueDataSet = DialogueDataSets[dialogueSetIndex];
                CurrentDialogueDataSet.Reset();
                ShowCurrentDialogueDataToUI();
            });
        }

        public override void PlayDialogueSet(string dialogueSetId)
        {
            ShowDialogueBar(() =>
            {
                CurrentDialogueDataSet = DialogueDataSets.Find(x => x.setId == dialogueSetId);
                if (CurrentDialogueDataSet == null)
                    return;
                CurrentDialogueDataSet.Reset();
                ShowCurrentDialogueDataToUI();
            });
        }

        [HorizontalGroup("Debug Function/ToggleBar")]
        [Button]
        public void HideDialogueBar() => HideDialogueBar(0.2f, null);
        public void HideDialogueBar(Action OnComplete) => HideDialogueBar(0.2f, OnComplete);
        public void HideDialogueBar(float inDuration, Action OnComplete)
        {
            dialogueBarCG.DOFade(0, inDuration).OnComplete(() =>
            {
                ResetUI();
                OnComplete?.Invoke();
            });
        }
        public void HideDialogueBar_Instant() => HideDialogueBar(0, null);

        void Process_DialogueData_OnComplete(DialogueData dialogueData)
        {
            Process_DialogueActions(dialogueData.actionsOnComplete);
        }

        void Process_DialogueDataSet_OnComplete(DialogueDataSet dialogueDataSet)
        {
            if (dialogueDataSet.CompleteActionTriggered)
                return;

            ApplyPortraitData(dialogueDataSet.portraitsOnComplete);
            Process_DialogueActions(dialogueDataSet.actionsOnComplete);

            dialogueDataSet.CompleteActionTriggered = true;
        }

        protected void Process_DialogueActions(List<DialogueActionEntry> entries)
        {
            foreach (var action in entries)
            {
                Process_SingleDialogueAction(action);
            }
        }

        protected virtual void Process_SingleDialogueAction(DialogueActionEntry actionEntry)
        {
            switch (actionEntry.actionType)
            {
                case DialogueActionType.HideDialogueBar:
                    var hideDialogueParam = actionEntry.hideDialogue_Param;
                    if (hideDialogueParam.instant)
                        HideDialogueBar_Instant();
                    else
                        HideDialogueBar();
                    break;
                case DialogueActionType.RaiseEvent:
                    var events = actionEntry.raiseEvent_Param.events;
                    PrimitiveEventRaiser.RaiseList(events);
                    break;
                case DialogueActionType.CompleteDialogue:
                    CompleteDialogueSO();
                    break;
                case DialogueActionType.ShowOptions:
                    string optionSetId = actionEntry.showOptions_Param.optionSetId;
                    ShowOptions(optionSetId);
                    break;
                case DialogueActionType.PlayDialogueSet:
                    string dialogueSetId = actionEntry.playDialogueSet_Param.dialogueSetId;
                    PlayDialogueSet(dialogueSetId);
                    break;
            }
        }

        protected abstract void ApplyPortraitData(List<PortraitState> states);

        [TitleGroup("Debug Function")]
        [Button]
        public void ResetUI()
        {
            speakerNameTmp.text = "";
            dialogueTyper.StartTyping("");
        }

        [HorizontalGroup("Debug Function/Options")]
        [Button]
        public void ShowOptions(string optionSetId)
        {
            nextBtn.gameObject.SetActive(false);
            OptionDataSet selectedSet = currentDialogueSO.optionDataSets.Find(x => x.setId == optionSetId);
            CustomUtility.DestroyAllChildren(optionParent);

            foreach (var optionData in selectedSet.options)
            {
                OptionController newOption = Instantiate(optionUIPrefab, optionParent);
                newOption.Setup(optionData, this);
            }

            selectOptionEvent.Unsubscribe(SelectOptionRaisenHandler);
            selectOptionEvent.Subscribe(SelectOptionRaisenHandler);
        }

        void SelectOptionRaisenHandler(string optionId)
        {
            CloseOptions();
        }

        [HorizontalGroup("Debug Function/Options")]
        [Button]
        public void CloseOptions()
        {
            nextBtn.gameObject.SetActive(true);
            CustomUtility.DestroyAllChildren(optionParent);

            selectOptionEvent.Unsubscribe(SelectOptionRaisenHandler);
        }

        public void Process_OptionData(OptionData optionData)
        {
            selectOptionEvent.Raise(optionData.optionId);
            Process_DialogueActions(optionData.actionsOnSelect);
        }
    }
}
