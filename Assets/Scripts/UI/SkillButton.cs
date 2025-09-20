using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private SkillSO skill;
    
    [SerializeField] private Image iconImage; // Assign in Inspector: the UI Image for the skill icon
    
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color availableColor = Color.green;
    [SerializeField] private Color cannotAffordColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.white;

    [SerializeField] private SkillTooltip tooltip;
    
    [SerializeField] private Image[] roadToNextSkill;
    
    private Button button;
    private Image buttonImage; 
    
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        UpdateButtonState();
        button.onClick.AddListener(OnClick);
        SkillManager.Instance.OnSkillPointsChanged += OnSkillPointsChanged;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnSkillPointsChanged -= OnSkillPointsChanged;
    }

    private void UpdateButtonState()
    {
        Debug.Log("Updating button state");
        if (skill == null) return;

        SkillState state = GetSkillState();

        switch (state)
        {
            case SkillState.Unlocked:
                //button.interactable = false;
                buttonImage.color = unlockedColor;
                SetRoadColorGreen();
                break;
            case SkillState.Available:
                //button.interactable = true;
                buttonImage.color = availableColor;
                break;
            case SkillState.CannotAfford:
                //button.interactable = false;
                buttonImage.color = cannotAffordColor;
                break;
            case SkillState.Locked:
                //button.interactable = false;
                buttonImage.color = lockedColor;
                break;
        }
    }

    private void SetRoadColorGreen()
    {
        foreach (var i in roadToNextSkill)
        {
            i.color = Color.green;
        }
    }

    private SkillState GetSkillState()
    {
        if (SkillManager.Instance.IsSkillUnlocked(skill))
            return SkillState.Unlocked;

        bool prereqsMet = true;
        if (skill.prerequisites != null)
        {
            foreach (var prereq in skill.prerequisites)
            {
                if (!SkillManager.Instance.IsSkillUnlocked(prereq))
                {
                    prereqsMet = false;
                    break;
                }
            }
        }

        if (!prereqsMet)
            return SkillState.Locked;

        bool canAfford = SkillManager.Instance.AvailableSkillPoints >= skill.skillPointCost &&
                         CurrencyManager.Instance.GetMoney() >= skill.skillMoneyCost;

        return canAfford ? SkillState.Available : SkillState.CannotAfford;
    }

    private void OnClick()
    {
        tooltip.ShowTooltip(skill, GetSkillState());
    }

    private void OnSkillPointsChanged(int newPoints)
    {
        UpdateButtonState();
    }
}

public enum SkillState
{
    Locked,
    Available,
    CannotAfford,
    Unlocked
}

