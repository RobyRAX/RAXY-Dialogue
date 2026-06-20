using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace RAXY.Dialogue
{
    [CreateAssetMenu(menuName = "RAXY/Dialogue/Dialogue SO")]
    public class DialogueSO : ScriptableObject
    {
#if UNITY_EDITOR
        [PropertyOrder(-2)]
        [ShowIf("@!IsUsingTimeline")]
        [Button]
        public void SwitchToTimeline()
        {
            StaticIsUsingTimeline = true;
            dialogueType = DialogueType.TimelineDialogue;

            RefreshAll();
        }

        [PropertyOrder(-2)]
        [ShowIf("@IsUsingTimeline")]
        [Button]
        public void SwitchToNonTimeline()
        {
            StaticIsUsingTimeline = false;
            dialogueType = DialogueType.NonTimelineDialogue;

            RefreshAll();
        }

        public void RefreshAll()
        {
            foreach (var portraitData in portraitsOnStart)
            {
                if (IsUsingTimeline)
                    portraitData.SwitchToTimeline();
                else
                    portraitData.SwitchToNonTimeline();
            }

            foreach (var dialogueDataSet in dialogueDataSets)
            {
                foreach (var dialogueData in dialogueDataSet.dialogueDatas)
                {
                    foreach (var portraitData in dialogueData.portraitsOnStart)
                    {
                        if (IsUsingTimeline)
                            portraitData.SwitchToTimeline();
                        else
                            portraitData.SwitchToNonTimeline();
                    }
                }

                foreach (var portraitData in dialogueDataSet.portraitsOnComplete)
                {
                    if (IsUsingTimeline)
                        portraitData.SwitchToTimeline();
                    else
                        portraitData.SwitchToNonTimeline();
                }
            }

            RefreshPortraitEditorData();
            GetMarkers();
        } 

        public static bool StaticIsUsingTimeline { get; set; }
#endif
        [ReadOnly]
        public bool IsUsingTimeline => dialogueType == DialogueType.TimelineDialogue;

        [ReadOnly]
        [HideLabel]
        [EnumToggleButtons]
        public DialogueType dialogueType;

        [TitleGroup("Timeline")]
        [ShowIf("@IsUsingTimeline")]
        public TimelineAsset timeline;

        [TitleGroup("Timeline")]
        [TableList]
        [ShowIf("@IsUsingTimeline")]
        public List<MarkerTimeId> markers;

        public double GetMarkerTime(string markerId)
        {
            return markers.Find(x => x.id == markerId).Time;
        }

        public double GetMarkerTime(string markerId, int frameOffset)
        {
            var marker = markers.Find(x => x.id == markerId);
            if (marker == null || timeline == null)
                return 0;

            double fps = timeline.editorSettings.frameRate;
            double frameDuration = 1.0 / fps;

            return marker.Time + (frameOffset * frameDuration);
        }

        public static List<string> MarkerIds { get; set; }

        [TitleGroup("Timeline")]
        [Button]
        [ShowIf("@IsUsingTimeline")]
        void GetMarkers()
        {
            markers.Clear();

            if (timeline == null)
            {
                Debug.LogWarning("Timeline not assigned.");
                return;
            }

            // Collect all IdMarkers across all tracks
            foreach (var output in timeline.outputs)
            {
                if (output.sourceObject is not TrackAsset track)
                    continue;

                foreach (var marker in track.GetMarkers())
                {
                    if (marker is IdMarker idMarker)
                        markers.Add(new MarkerTimeId(idMarker.id, idMarker));
                }
            }

            // Add start and end markers once
            markers.Add(new MarkerTimeId(MarkerTimeId.START_MARKER, 0));
            markers.Add(new MarkerTimeId(MarkerTimeId.END_MARKER, timeline));

            // Sort once after collecting everything
            markers = markers.OrderBy(m => m.Time).ToList();

            // Build MarkerIds in the same order
            MarkerIds = new List<string>();
            foreach (var marker in markers)
                MarkerIds.Add(marker.id);
        }

        [TitleGroup("Actors")]
        [ListDrawerSettings(ShowIndexLabels = true)]
        [OnValueChanged("RefreshPortraitEditorData")]
        public List<DialogueActorSO> actors;

        [TitleGroup("On Start")]
        [ValueDropdown("MarkerIds")]
        [ShowIf("@IsUsingTimeline")]
        public string firstStopMarker;

        [TitleGroup("On Start")]
        [ShowIf("@!IsUsingTimeline")]
        public int firstDialogueDataSet;

        [TitleGroup("On Start")]
        public List<PortraitState> portraitsOnStart;

        [TitleGroup("Dialogue")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "setId")]
        public List<DialogueDataSet> dialogueDataSets;

        [TitleGroup("Option")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "setId")]
        public List<OptionDataSet> optionDataSets;

#if UNITY_EDITOR
        [TitleGroup("Actors")]
        [Button]
        void RefreshPortraitEditorData()
        {
            portraitDatas_Editor = new();
            if (actors == null)
                return;

            foreach (var actor in actors)
            {
                if (actor == null)
                    continue;
                portraitDatas_Editor.Add(actor.GetPortraitEditorData());
            }

            PortraitState.SetPortraitData(portraitDatas_Editor);
            Debug.Log("Refreshed");
        }

        [TitleGroup("Debug")]
        [SerializeField]
        public List<DialoguePortrait_EditorData> portraitDatas_Editor;
#endif
    }

    [Serializable]
    public class MarkerTimeId
    {
        [TableColumnWidth(75, false)]
        public string id;

        [SerializeField, HideInInspector]
        private Marker marker;

        [SerializeField, HideInInspector]
        private double time;

        [SerializeField, HideInInspector]
        private TimelineAsset timeline;

        [SerializeField, HideInInspector]
        private GetTimeType getTimeType;

        [ShowInInspector, ReadOnly]
        [SuffixLabel("seconds")]
        public double Time
        {
            get
            {
                switch (getTimeType)
                {
                    case GetTimeType.FlatTime:
                        return time;
                    case GetTimeType.Marker:
                        if (marker == null)
                            return 0;
                        return marker.time;
                    case GetTimeType.TimelineEnd:
                        if (timeline == null)
                            return 0;
                        return timeline.duration;
                    default:
                        return 0;
                }
            }
        }

        private enum GetTimeType
        {
            FlatTime,
            Marker,
            TimelineEnd
        }

        public MarkerTimeId() { }

        public MarkerTimeId(string id, double time)
        {
            this.id = id ?? "unnamed";
            this.time = time;
            getTimeType = GetTimeType.FlatTime;
        }

        public MarkerTimeId(string id, Marker marker)
        {
            this.id = id ?? "unnamed";
            this.marker = marker;
            getTimeType = GetTimeType.Marker;
        }

        public MarkerTimeId(string id, TimelineAsset timeline)
        {
            this.id = id ?? "unnamed";
            this.timeline = timeline;
            getTimeType = GetTimeType.TimelineEnd;
        }

        public const string START_MARKER = "start";
        public const string END_MARKER = "end";
    }
}