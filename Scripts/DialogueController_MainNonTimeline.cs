using System.Collections.Generic;
using DG.Tweening;
using RAXY.Event;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RAXY.Dialogue
{
    public class DialogueController_MainNonTimeline : DialogueController_MainBase
    {
        [TitleGroup("Tween Setting")]
        [SerializeField] float portraitPositionTweenTime = 0.2f;

        [TitleGroup("Tween Setting")]
        [SerializeField] float playDialogueDelay = 0.3f;

        [TitleGroup("Portrait Position")]
        [SerializeField] Transform leftPos;

        [TitleGroup("Portrait Position")]
        [SerializeField] Transform centerPos;

        [TitleGroup("Portrait Position")]
        [SerializeField] Transform rightPos;

        public override DialogueType DialogueType => DialogueType.NonTimelineDialogue;

        protected override void PrepareDialogueSO(DialogueSO dialogueSO)
        {
            base.PrepareDialogueSO(dialogueSO);

            SetPortraitsPosition(currentDialogueSO.portraitsOnStart);
        }

        void SetPortraitsPosition(List<PortraitState> states)
        {
            // Null or empty check
            if (states == null || states.Count == 0)
            {
                Debug.LogWarning("SetPortraitsPosition() called with null or empty states");
                return;
            }

            foreach (var portrait in PortraitDict?.Values)
            {
                if (portrait == null)
                {
                    continue;
                }

                // find state
                var selectedPortraitState = states.Find(
                    x => x != null && x.actorIndex == portrait.PortraitRuntimeIndex
                );

                if (selectedPortraitState == null || !selectedPortraitState.setPosition)
                {
                    continue;
                }

                // switch expression (correct syntax)
                Transform selectedPos = selectedPortraitState.portraitPosition switch
                {
                    DialoguePortraitPosition.Left => leftPos,
                    DialoguePortraitPosition.Center => centerPos,
                    DialoguePortraitPosition.Right => rightPos,
                    _ => centerPos
                };

                if (selectedPos != null)
                {
                    portrait.SetPosition(selectedPos, 
                                            selectedPortraitState.useTween_Position, 
                                            portraitPositionTweenTime);
                }
            }
        }

        public override void PlayDialogueSO(DialogueSO dialogueSO)
        {
            dialogueParentCG.alpha = 0;
            isBlockingNext = true;

            base.PlayDialogueSO(dialogueSO);

            dialogueParentCG.DOFade(1, playDialogueFadeInTime).OnComplete(() =>
            {
                DOVirtual.DelayedCall(playDialogueDelay, () =>
                {
                    PlayDialogueSet(currentDialogueSO.firstDialogueDataSet);
                });
            });
        }

        protected override void ApplyPortraitData(List<PortraitState> states)
        {
            foreach (var portrait in PortraitDict.Values)
            {
                var selectedPortState = states.Find(x => x.actorIndex == portrait.PortraitRuntimeIndex);
                if (selectedPortState == null)
                    continue;

                if (selectedPortState.setSwappable)
                    portrait.SetSwappableSprite(selectedPortState.swappableId);

                if (selectedPortState.setOpacity)
                {
                    portrait.SetOpacity(selectedPortState.opacity, 
                                        selectedPortState.setRootOpacity, 
                                        selectedPortState.useTween_Opacity,
                                        portraitOpacityTweenTime);
                }
            }

            SetPortraitsPosition(states);
        }
    }
}
