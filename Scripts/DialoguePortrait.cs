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
        public int PortraitRuntimeIndex { get; set; }

        [TitleGroup("UI")]
        public RectTransform pivot;
        [TitleGroup("UI")]
        public CanvasGroup mainCG;
        [TitleGroup("UI")]
        public CanvasGroup charColorCG;
        [TitleGroup("UI")]
        public List<Image> swappableImgs;

        [TitleGroup("Swappable")]
        [TableList]
        [SerializeField]
        List<SwappableEntry> swappables;
        public Dictionary<string, SwappableEntry> SwappableDict;

#if UNITY_EDITOR
        public List<SwappableEntry> Swappables => swappables;
#endif

        void InitFaceDict()
        {
            SwappableDict = new();

            foreach (var face in swappables)
            {
                SwappableDict.Add(face.swappableId, face);
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
        public void SetSwappableSprite(string swappableId)
        {
            if (SwappableDict != null && SwappableDict.TryGetValue(swappableId, out SwappableEntry swappable))
            {
                foreach (var img in swappableImgs)
                {
                    img.sprite = swappable.swappableSprite;
                }
            }
            else
            {
                var swappableAlt = swappables.Find(x => x.swappableId == swappableId);
                if (swappableAlt != null)
                {
                    foreach (var img in swappableImgs)
                    {
                        img.sprite = swappableAlt.swappableSprite;
                    }
                }
            }
        }

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
    public class SwappableEntry
    {
        public string swappableId;
        public Sprite swappableSprite;
    }

#if UNITY_EDITOR
    [Serializable]
    public class DialoguePortrait_EditorData
    {
        public string portraitId;
        public List<string> swappableIds;

        public DialoguePortrait_EditorData() { }
    }
#endif
}

