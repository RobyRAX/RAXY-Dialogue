using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace RAXY.Dialogue
{
    [Serializable]
    public class StartDialogueMarker : Marker, INotification
    {
        public PropertyName id => new PropertyName();

        public enum InputMethod
        {
            Index, Id
        }

        public InputMethod inputMethod;
        public string dialogueSetId;
        public int dialogueSetIndex;
    }
}

