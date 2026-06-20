using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Dialogue
{
    [Serializable]
    public class PortraitState
    {
        [HorizontalGroup("Actor")]
        public int actorIndex;

        [HorizontalGroup("PortraitOptions_1")]

        // --- Opacity section ---
        [BoxGroup("PortraitOptions_1/Opacity")]
        [ToggleLeft]
        public bool setOpacity;

        [BoxGroup("PortraitOptions_1/Opacity")]
        [ShowIf("@setOpacity && !_isUsingTimeline")]
        [ToggleLeft]
        public bool setRootOpacity;

        [BoxGroup("PortraitOptions_1/Opacity")]
        [ShowIf("@setOpacity")]
        [ToggleLeft]
        [LabelText("Use Tween")]
        public bool useTween_Opacity;

        [BoxGroup("PortraitOptions_1/Opacity")]
        [ShowIf("@setOpacity")]
        [HideLabel]
        [PropertyRange(0, 1)]
        public float opacity;

        // --- Face section ---
        [BoxGroup("PortraitOptions_1/Face")]
        [ToggleLeft]
        public bool setFace;

        [BoxGroup("PortraitOptions_1/Face")]
        [ShowIf("@setFace")]
        [ValueDropdown("SelectedFaceIds")]
        public string faceId;

        // --- Portrait position section
        [BoxGroup("PortraitOptions_1/Position")]
        [ToggleLeft]
        [ShowIf("@!_isUsingTimeline")]
        public bool setPosition;

        [BoxGroup("PortraitOptions_1/Position")]
        [ToggleLeft]
        [ShowIf("@!_isUsingTimeline")]
        [LabelText("Use Tween")]
        public bool useTween_Position;

        [BoxGroup("PortraitOptions_1/Position")]
        [ShowIf("@setPosition && !_isUsingTimeline")]
        [EnumToggleButtons]
        [HideLabel]
        public DialoguePortraitPosition portraitPosition;

        public PortraitState()
        {
#if UNITY_EDITOR
            if (DialogueSO.StaticIsUsingTimeline)
                SwitchToTimeline();
            else
                SwitchToNonTimeline();
#endif
        }

#if UNITY_EDITOR
        bool _isUsingTimeline;

        public void SwitchToTimeline()
        {
            _isUsingTimeline = true;
        }

        public void SwitchToNonTimeline()
        {
            _isUsingTimeline = false;
        }

        [HorizontalGroup("Actor")]
        [HideLabel]
        [ShowInInspector]
        [DisplayAsString]
        public string PortraitId
        {
            get
            {
                if (_cachedPortraitDatas == null)
                    return null;

                if (actorIndex < 0 || actorIndex >= _cachedPortraitDatas.Count)
                    return null;

                var data = _cachedPortraitDatas[actorIndex];
                return data?.portraitId;
            }
        }

        static List<DialoguePortrait_EditorData> _cachedPortraitDatas;
        static List<string> _cachedPortraitIds;
        List<string> SelectedFaceIds
        {
            get
            {
                var newList = new List<string>();

                var selectedData = _cachedPortraitDatas?.Find(x => x.portraitId == PortraitId);
                if (selectedData == null)
                    return newList;

                foreach (var face in selectedData.faceIds)
                {
                    newList.Add(face);
                }

                return newList;
            }
        }

        public static void SetPortraitData(List<DialoguePortrait_EditorData> data)
        {
            _cachedPortraitDatas = data;
            _cachedPortraitIds = new();
            foreach (var portData in _cachedPortraitDatas)
            {
                _cachedPortraitIds.Add(portData.portraitId);
            }
        }
#endif
    }

    public enum DialoguePortraitPosition
    {
        Left, Center, Right
    }
}
