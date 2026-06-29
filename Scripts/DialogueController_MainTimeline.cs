using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;
using RAXY.Event;
using UnityEngine.Serialization;
using RAXY.Utility;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities.Editor;
#endif

namespace RAXY.Dialogue
{
    [RequireComponent(typeof(PlayableDirector))]
    public class DialogueController_MainTimeline : DialogueController_MainBase, INotificationReceiver
    {
        public override DialogueType DialogueType => DialogueType.TimelineDialogue;
        PlayableDirector _pd;

        protected override void Start()
        {
            base.Start();

            _pd = GetComponent<PlayableDirector>();
        }

        public override void PlayDialogueSO(DialogueSO dialogueSO)
        {
            base.PlayDialogueSO(dialogueSO);
            dialogueParentCG.alpha = 1;

            var markerTime = dialogueSO.GetMarkerTime(dialogueSO.firstStopMarker);
            PlayTimelineSegment(0, markerTime);
            isBlockingNext = true;
        }

        protected override void Process_SingleDialogueAction(DialogueActionEntry actionEntry)
        {
            base.Process_SingleDialogueAction(actionEntry);

            switch (actionEntry.actionType)
            {
                case DialogueActionType.PlayTimeline:
                    var playTimelineParam = actionEntry.playTimeline_Param;
                    Process_PlayTimeline(playTimelineParam.startFrameMarker, playTimelineParam.endFrameMarker);
                    break;
            }
        }

        void BindPortraitToTimeline()
        {
            if (currentDialogueSO.IsUsingTimeline == false)
                return;

            if (_pd == null || currentDialogueSO == null || currentDialogueSO.timeline == null)
            {
                Debug.LogWarning("PlayableDirector or TimelineAsset not set!");
                return;
            }

            var timeline = currentDialogueSO.timeline;
            _pd.playableAsset = timeline; // assign timeline

            foreach (var output in timeline.outputs)
            {
                if (output.sourceObject is DialogueMarkerTrack track)
                {
                    _pd.SetGenericBinding(track, this);
                }
            }

            // --- bind each portrait ---
            int index = 0;
            foreach (var portrait in PortraitDict)
            {
                string id = $"Actor {index}";
                foreach (var output in timeline.outputs)
                {
                    // Try to match track name
                    var track = output.sourceObject as TrackAsset;
                    if (track == null)
                        continue;

                    if (track.name.Contains(id))
                    {
                        // Usually track output target type is Animator for AnimationTrack
                        var anim = portrait.Value.GetComponent<Animator>();
                        if (anim != null)
                        {
                            _pd.SetGenericBinding(track, anim);
                            Debug.Log($"Bound {track.name} to {portrait.Value.name}");
                        }
                        else
                        {
                            Debug.LogWarning($"Portrait '{portrait.Value.name}' has no Animator to bind!");
                        }
                    }

                    if (track is DialogueMarkerTrack)
                    {
                        _pd.SetGenericBinding(track, this);
                    }
                }
                index++;
            }

            // Rebuild graph to apply bindings
            _pd.RebuildGraph();
            _pd.Evaluate();

            Debug.Log($"Bound {PortraitDict.Count} portraits to timeline tracks.");
        }

        protected override void PrepareDialogueSO(DialogueSO dialogueSO)
        {
            base.PrepareDialogueSO(dialogueSO);

            _pd = GetComponent<PlayableDirector>();
            BindPortraitToTimeline();
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
                                        false, 
                                        selectedPortState.useTween_Opacity,
                                        portraitOpacityTweenTime);
                }
            }
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is StartDialogueMarker marker)
            {
                if (marker.inputMethod == StartDialogueMarker.InputMethod.Index)
                    PlayDialogueSet(marker.dialogueSetIndex);
                else
                    PlayDialogueSet(marker.dialogueSetId);
            }
            else if (notification is PauseTimelineMarker)
            {
                _pd.Pause();
            }
        }

        void Process_PlayTimeline(string startMarker, string endMarker)
        {
            double startTime = currentDialogueSO.GetMarkerTime(startMarker);
            double endTime = currentDialogueSO.GetMarkerTime(endMarker);
            PlayTimelineSegment(startTime, endTime);
        }
        public void PlayTimelineSegment(double startTime, double endTime)
        {
            StartCoroutine(PlayTimelineSegmentCo(startTime, endTime));
        }
        IEnumerator PlayTimelineSegmentCo(double startTime, double endTime)
        {
            _pd.time = startTime;
            _pd.Evaluate();
            _pd.Play();

            while (_pd.time < endTime)
                yield return null;

            _pd.Pause();
        }
    }
}