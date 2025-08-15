using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    
    [Header("Quest Settings")]
    [SerializeField] private Quest[] allQuests; 
    [SerializeField] private int maxActiveQuests = 4;
    
    public Action<Quest> OnQuestCompleted;
    public Action<Quest> OnQuestActivated;
    
    private List<Quest> activeQuests = new List<Quest>();
    private int currentQuestIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeQuests();
    }
    
    private void InitializeQuests()
    {
        // Reset all quests
        foreach (var quest in allQuests)
        {
            quest.ResetQuest();
        }
        
        // Activate first 4 quests
        for (int i = 0; i < Mathf.Min(maxActiveQuests, allQuests.Length); i++)
        {
            ActivateQuest(allQuests[i]);
            currentQuestIndex = i + 1;
        }
        
    }
    
    private void ActivateQuest(Quest quest)
    {
        quest.isActive = true;
        activeQuests.Add(quest);
        OnQuestActivated?.Invoke(quest);
    }
    
    private void CompleteQuest(Quest quest)
    {
        if (!quest.isActive || quest.isCompleted) return;
        
        quest.CompleteQuest();
        activeQuests.Remove(quest);
        
        // Give rewards
        GiveRewards(quest.rewards);
        
        OnQuestCompleted?.Invoke(quest);
        
        // Activate next quest if available
        ActivateNextQuest();
    }
    
    void ActivateNextQuest()
    {
        if (currentQuestIndex < allQuests.Length)
        {
            ActivateQuest(allQuests[currentQuestIndex]);
            currentQuestIndex++;
        }
    }
    
    void GiveRewards(Reward[] rewards)
    {
        foreach (var reward in rewards)
        {
            switch (reward.rewardType)
            {
                case QuestRewardType.Coin:
                    CurrencyManager.Instance.AddCoin(reward.amount);
                    break;
                case QuestRewardType.Money:
                    CurrencyManager.Instance.AddMoney(reward.amount);
                    break;
            }
        }
    }
    
    public void UpdateQuestProgress(QuestType questType, int amount = 1)
    {
        var questsToUpdate = activeQuests.Where(q => q.questType == questType && !q.isCompleted).ToList();
        
        foreach (var quest in questsToUpdate)
        {
            quest.UpdateProgress(amount);
            if (quest.CheckCompletion())
            {
                CompleteQuest(quest);
            }
        }
    }
    
    public void UpdateSpecificQuest(string questHeader, int amount = 1)
    {
        var quest = activeQuests.FirstOrDefault(q => q.questHeader == questHeader && !q.isCompleted);
        if (quest != null)
        {
            quest.UpdateProgress(amount);
            if (quest.CheckCompletion())
            {
                CompleteQuest(quest);
            }
        }
    }
    
    public List<Quest> GetActiveQuests()
    {
        return new List<Quest>(activeQuests);
    }
}
