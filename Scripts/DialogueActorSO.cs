using System;
using System.Collections.Generic;
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
        public LocalizationCacher ActorNameLoc;

        public bool useAddressable = true;

        [HideIf("@useAddressable")]
        public GameObject dialoguePortrait;

        [ShowIf("@useAddressable")]
        public AssetReferenceGameObject dialoguePortraitRef;

#if UNITY_EDITOR
        public DialoguePortrait_EditorData GetPortraitEditorData()
        {
            var editorData = new DialoguePortrait_EditorData();
            editorData.portraitId = actorId;
            editorData.faceIds = new();
            var faces = dialoguePortrait.GetComponent<DialoguePortrait>().Faces;
            foreach (var face in faces)
            {
                editorData.faceIds.Add(face.faceId);
            }

            return editorData;
        }
#endif
    }
}