using UnityEngine;

public enum QuestType
{
    ServeCustomers,
    ReachLevel,
    EarnMoney,
    PlayForTime
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Information")]
    public string questHeader;
    public Sprite questIcon;
    public QuestType questType;
    
    [Header("Quest Progress")]
    public int targetAmount = 1;
    public int currentProgress = 0;
    
    [Header("Rewards")]
    public Reward rewards;
    
    [Header("Quest Status")]
    public bool isActive = false;
    public bool isCompleted = false;
    
    public bool CheckCompletion()
    {
        return currentProgress >= targetAmount;
    }
    
    public void UpdateProgress(int amount = 1)
    {
        if (!isActive || isCompleted) return;
        
        currentProgress += amount;
        currentProgress = Mathf.Clamp(currentProgress, 0, targetAmount);
    }
    
    public void CompleteQuest()
    {
        isCompleted = true;
        isActive = false;
    }
    
    public void ResetQuest()
    {
        currentProgress = 0;
        isCompleted = false;
        isActive = false;
    }
}