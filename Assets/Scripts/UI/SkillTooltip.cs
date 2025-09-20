using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTooltip : MonoBehaviour
{
    [SerializeField] private SkillUI skillUI;
    
    [Header("UI References")]
    [SerializeField] private RectTransform tooltipPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI moneyCostText;
    [SerializeField] private TextMeshProUGUI spCostText;
    
    //Pre purchase state
    [SerializeField] private GameObject prePurchaseState;
    [SerializeField] private Button claimButton;
    [SerializeField] private Color canClaimColor;
    [SerializeField] private Color cantClaimColor;
    
    //Post purchase state
    [SerializeField] private TextMeshProUGUI claimedText;

    private bool isTooltipOpened;
    private SkillSO cachedSkillSO;
    
    private void Start()
    {
        claimedText.gameObject.SetActive(false);
        tooltipPanel.gameObject.SetActive(false);
        claimButton.onClick.AddListener(OnClick_ClaimButton);
    }
    
    public void ShowTooltip(in SkillSO skill, in SkillState skillState)
    {
        if (skill == null) return;

        if (isTooltipOpened)
        {
            UpdateTooltip(skill, skillState);
            return;      
        }
        
        isTooltipOpened = true;

        UpdateTooltip(skill,skillState);
        
        tooltipPanel.gameObject.SetActive(true);
        tooltipPanel.anchoredPosition = new Vector2(0, -150);
        
        DOTween.Sequence().Append(tooltipPanel.DOAnchorPosY(-210, 0.6f).SetEase(Ease.OutBounce));
    }

    private void UpdateTooltip(in SkillSO skill, in SkillState skillState)
    {
        cachedSkillSO = skill;
        titleText.text = skill.skillName;
        descriptionText.text = skill.skillDescription;
        icon.sprite = skill.skillIcon;
        moneyCostText.text = "$" + skill.skillMoneyCost.ToString();
        spCostText.text = skill.skillPointCost.ToString() + " SP";

        switch (skillState)
        {
            case SkillState.Unlocked:
                prePurchaseState.SetActive(false);
                claimedText.gameObject.SetActive(true);
                break;
            case SkillState.Available:
                prePurchaseState.SetActive(true);
                claimedText.gameObject.SetActive(false);
                claimButton.interactable = true;
                moneyCostText.color = canClaimColor;
                spCostText.color = canClaimColor;
                break;
            default:
                prePurchaseState.SetActive(true);
                claimedText.gameObject.SetActive(false);
                claimButton.interactable = false;
                moneyCostText.color = cantClaimColor;
                spCostText.color = cantClaimColor;
                break;
        }
    }

    private void OnClick_ClaimButton()
    {
        SkillManager.Instance.TryUnlockSkill(cachedSkillSO);
        skillUI.RefreshAllButtons();
    }
    
    public void HideTooltip()
    {
        DOTween.Sequence().Append(tooltipPanel.DOAnchorPosY(-150, 0.3f).SetEase(Ease.InBounce));
        tooltipPanel.gameObject.SetActive(false);
        isTooltipOpened = false;
    }
}
