using System;
using System.Collections.Generic;
using RAXY.Event;
using RAXY.Utility.Localization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Dialogue
{
    [Serializable]
    public class OptionDataSet
    {
        public string setId;

        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "optionText")]
        public List<OptionData> options;
    }

    [Serializable]
    public class OptionData
    {
        public string optionId;
        public string optionText;

        public List<DialogueActionEntry> actionsOnSelect; 
    }
}
