using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueSO dialogueSO;

        [Button]
        public void StartDialogue()
        {
            DialogueManager.Instance.PlayDialogueSO(dialogueSO);
        }
    }
}
