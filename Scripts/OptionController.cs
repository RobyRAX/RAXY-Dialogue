using RAXY.Event;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RAXY.Dialogue
{
    public class OptionController : MonoBehaviour
    {
        [TitleGroup("UI")]
        [SerializeField] TextMeshProUGUI optionTmp;
        [SerializeField] Button optionBtn;

        [TitleGroup("Runtime")]
        [ShowInInspector]
        public OptionData OptionData { get; set; }

        [TitleGroup("Runtime")]
        public DialogueController_MainBase DialogueController { get; set; }

        public void Setup(OptionData optionData, DialogueController_MainBase dialogueController)
        {
            OptionData = optionData;
            DialogueController = dialogueController;

            optionTmp.text = optionData.optionText;
            gameObject.name = optionData.optionId;

            optionBtn.onClick.RemoveListener(OnClick_OptionBtn);
            optionBtn.onClick.AddListener(OnClick_OptionBtn);
        }

        void OnClick_OptionBtn()
        {
            DialogueController.Process_OptionData(OptionData);
        }
    }
}
