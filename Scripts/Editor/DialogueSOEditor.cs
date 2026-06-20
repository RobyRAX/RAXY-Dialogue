#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace RAXY.Dialogue
{
    [CustomEditor(typeof(DialogueSO))]
    public class DialogueSOEditor : OdinEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
            if(((DialogueSO)target).IsUsingTimeline)
                ((DialogueSO)target).SwitchToTimeline();
            else
                ((DialogueSO)target).SwitchToNonTimeline();
        }
    }
}
#endif
