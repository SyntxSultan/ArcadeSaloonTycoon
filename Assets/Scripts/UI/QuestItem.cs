using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUIItem : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image questIcon;
    [SerializeField] private TextMeshProUGUI questHeader;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Transform rewardsContainer;
    [SerializeField] private GameObject rewardItemPrefab;
    
    private Quest currentQuest;
    
    public void SetupQuest(Quest quest)
    {
        currentQuest = quest;
        
        if (questIcon != null) questIcon.sprite = quest.questIcon;
        if (questHeader != null) questHeader.text = quest.questHeader;
        
        SetupRewards();
        
        UpdateProgress();
    }
    
    void SetupRewards()
    {
        if (rewardsContainer == null || rewardItemPrefab == null) return;
        
        // Clear existing reward items
        foreach (Transform child in rewardsContainer)
        {
            Destroy(child.gameObject);
        }
        
        if (currentQuest == null)
        {
            Debug.LogError("Current quest is null!");
            return;
        }

        if (currentQuest.rewards == null)
        {
            Debug.LogError("Current quest rewards are null!");
            return;       
        }
        // Create reward items
        foreach (QuestRewardStruct reward in currentQuest.rewards.rewardStructs)
        {
            GameObject rewardItem = Instantiate(rewardItemPrefab, rewardsContainer);
            QuestRewardItem rewardUI = rewardItem.GetComponent<QuestRewardItem>();
            if (rewardUI != null)
            {
                rewardUI.SetupReward(reward);
            }
        }
    }
    
    void Update()
    {
        if (currentQuest != null)
        {
            UpdateProgress();
        }
    }
    
    void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text = $"{currentQuest.currentProgress}/{currentQuest.targetAmount}";
        }
        
        if (progressSlider != null)
        {
            progressSlider.value = (float)currentQuest.currentProgress / currentQuest.targetAmount;
        }
    }
}
