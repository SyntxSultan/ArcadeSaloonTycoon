using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI availableSkillPointsText;
   [SerializeField] private SkillButton[] skillButtons;
   [SerializeField] private Button closeButton;

   private void Start()
   {
      SkillManager.Instance.OnSkillPointsChanged += OnSkillPointsChanged;
      closeButton.onClick.AddListener(OnCloseSkillUI);
   }

   private void OnCloseSkillUI()
   {
      GetComponent<SkillTooltip>().HideTooltip();
      ScreenManager.Instance.CloseSkillsUI();
   }

   private void OnSkillPointsChanged(int skillPoints)
   {
      availableSkillPointsText.text = skillPoints.ToString();
   }

   public void RefreshAllButtons()
   {
      foreach (var button in skillButtons)
      {
         // Force update state
         button.GetType().GetMethod("UpdateButtonState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(button, null);
      }
   }
}
