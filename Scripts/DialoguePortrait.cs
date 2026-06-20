using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace RAXY.Dialogue
{
    public class DialoguePortrait : MonoBehaviour
    {
        public string portraitId;

        public int PortraitRuntimeIndex { get; set; }

        [TitleGroup("UI")]
        public RectTransform rootRT;
        [TitleGroup("UI")]
        public CanvasGroup mainCG;
        [TitleGroup("UI")]
        public CanvasGroup charColorCG;
        [TitleGroup("UI")]
        public Image charFaceImg;

        [TitleGroup("Face")]
        [TableList]
        [SerializeField]
        List<FaceEntry> faces;
        public Dictionary<string, FaceEntry> FaceDict;

#if UNITY_EDITOR
        public List<FaceEntry> Faces => faces;
#endif

        void InitFaceDict()
        {
            FaceDict = new();

            foreach (var face in faces)
            {
                FaceDict.Add(face.faceId, face);
            }
        }

        Animator _anim;

        void Start()
        {
            _anim = GetComponent<Animator>();
            if (_anim != null)
            {
                _anim.runtimeAnimatorController = null;
            }

            InitFaceDict();
        }

        [TitleGroup("Debug Function")]
        [Button]
        public void SetFaceSprite(string faceId)
        {
            if (FaceDict != null && FaceDict.TryGetValue(faceId, out FaceEntry face))
            {
                charFaceImg.sprite = face.faceSprite;
            }
            else
            {
                var faceAlt = faces.Find(x => x.faceId == faceId);
                if (faceAlt != null)
                {
                    charFaceImg.sprite = faceAlt.faceSprite;
                }
            }
        }

        // [HorizontalGroup("Debug Function/Op")]
        // [Button]
        // public void Dim()
        // {
        //     charColorCG.alpha = 0.33f;
        // }

        // [HorizontalGroup("Debug Function/Op")]
        // [Button]
        // public void Highlight()
        // {
        //     charColorCG.alpha = 1;
        // }

        /// <summary>
        ///
        /// </summary>
        /// <param name="opacity"></param>
        /// <param name="setRootOpacity">if this set to true, the charColorCG will be set to 1.
        /// if false, the rootCG will be set to 1 instead.</param>
        [Button]
        public void SetOpacity(float opacity,
                                bool setRootOpacity = false,
                                bool useTween = false,
                                float duration = 0.25f,
                                Ease ease = Ease.OutQuad)
        {
            if (mainCG == null || charColorCG == null)
            {
                Debug.LogWarning("SetOpacity() called but CanvasGroups are null.");
                return;
            }

            // Kill previous tweens to avoid stacking fade tweens
            mainCG.DOKill();
            charColorCG.DOKill();

            if (!useTween)
            {
                if (setRootOpacity)
                {
                    mainCG.alpha = opacity;
                    charColorCG.alpha = 1f;
                }
                else
                {
                    mainCG.alpha = 1f;
                    charColorCG.alpha = opacity;
                }

                return;
            }

            // Tween logic
            if (setRootOpacity)
            {
                mainCG.DOFade(opacity, duration).SetEase(ease);
                charColorCG.DOFade(1f, duration).SetEase(ease);
            }
            else
            {
                mainCG.DOFade(1f, duration).SetEase(ease);
                charColorCG.DOFade(opacity, duration).SetEase(ease);
            }
        }

        [Button]
        public void SetPosition(Transform target, bool useTween = false, float duration = 0.2f, Ease ease = Ease.OutQuad)
        {
            if (target == null)
            {
                Debug.LogWarning("SetPosition() called with null target.");
                return;
            }

            // Kill previous tweens on this transform to avoid stacking
            transform.DOKill();

            if (!useTween)
            {
                transform.position = target.position;
                return;
            }

            // Tween to target position
            transform.DOMove(target.position, duration)
                     .SetEase(ease);
        }
    }

    [Serializable]
    public class FaceEntry
    {
        public string faceId;
        public Sprite faceSprite;
    }

#if UNITY_EDITOR
    [Serializable]
    public class DialoguePortrait_EditorData
    {
        public string portraitId;
        public List<string> faceIds;

        public DialoguePortrait_EditorData() { }
        public DialoguePortrait_EditorData(DialoguePortrait portrait)
        {
            portraitId = portrait.portraitId;
            faceIds = new();

            foreach (var face in portrait.Faces)
            {
                faceIds.Add(face.faceId);
            }
        }
    }
#endif
}

