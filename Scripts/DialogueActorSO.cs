using System;
using System.Collections.Generic;
using RAXY.Core.Addressable;
using RAXY.Utility.Localization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RAXY.Dialogue
{
    [CreateAssetMenu(menuName = "RAXY/Dialogue/Dialogue Actor")]
    public class DialogueActorSO : ScriptableObject
    {
        public string actorId;

        [TitleGroup("Name")]
        public bool useLocalization;

        [TitleGroup("Name")]
        [ShowIf("@useLocalization")]
        public LocalizationCacher ActorNameLoc;

        [TitleGroup("Name")]
        [HideIf("@useLocalization")]
        [SerializeField]
        string actorName;

        [TitleGroup("Portrait")]
        [HideLabel]
        public AddressableAssetProviderGameObject dialoguePortraitProvider;

        public string ActorName
        {
            get
            {
                if (useLocalization)
                    return ActorNameLoc.CachedString;
                else
                    return actorName;
            }
        }

#if UNITY_EDITOR
        public DialoguePortrait_EditorData GetPortraitEditorData()
        {
            var editorData = new DialoguePortrait_EditorData();
            editorData.portraitId = actorId;
            editorData.swappableIds = new();

            if (dialoguePortraitProvider.Asset != null)
            {
                var swappables = dialoguePortraitProvider.Asset.GetComponent<DialoguePortrait>().Swappables;
                foreach (var face in swappables)
                {
                    editorData.swappableIds.Add(face.swappableId);
                }
            }

            return editorData;
        }
#endif
    }
}