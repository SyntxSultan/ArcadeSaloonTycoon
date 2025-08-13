using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DailyReward
{
    public string rewardName;
    public int amount;
    public RewardType RewardType;
}

[Serializable]
public enum RewardType
{
    Money,
    Coin
}

public class DailyRewardManager : MonoBehaviour
{
    [Header("Reward Configuration")]
    [SerializeField] private DailyReward[] rewards = new DailyReward[7];
    
    [Header("Events")]
    public System.Action<int> OnRewardClaimed;
    public System.Action OnRewardsReset;
    
    private const string LAST_CLAIM_DATE_KEY = "LastClaimDate";
    private const string CLAIMED_DAYS_KEY = "ClaimedDays";
    
    private DateTime lastClaimDate;
    private HashSet<int> claimedDays = new HashSet<int>();
    private string scheduledNotificationId = null;
    
    public static DailyRewardManager Instance { get; private set; }
    
    private void OnApplicationPause(bool paused)
    {
        if (paused)
            ScheduleDailyRewardNotificationIfNeeded();
        else
            MobileNotificationHelper.CancelAllNotifications();
    }

    private void OnApplicationQuit()
    {
        ScheduleDailyRewardNotificationIfNeeded();
    }

    private void ScheduleDailyRewardNotificationIfNeeded()
    {
        int next = GetNextClaimableDay();
        if (next >= 0 && !HasClaimedToday())
        {
            var reward = GetReward(next);
            string title = "Günlük ödülün hazır!";
            string body = "Günlük ödülünü almayı unuttun çabuk geri gel!";
            
            scheduledNotificationId = MobileNotificationHelper.ScheduleNotification(title, body, DateTime.Now.AddSeconds(5));
            Debug.Log($"Scheduled immediate daily reward notification for day {next}");
            return;
        }

        // Eğer bugün zaten claim edilmişse, bir sonraki claim zamanına göre bildirim planla
        var until = GetTimeUntilNextClaim();
        if (until > TimeSpan.Zero)
        {
            DateTime fireAt = DateTime.Now + until + TimeSpan.FromSeconds(5); 
            string title = "Günlük ödülün hazır!";
            string body = "Hadi acele et ödülün al!";
            scheduledNotificationId = MobileNotificationHelper.ScheduleNotification(title, body, fireAt);
            Debug.Log($"Scheduled next-day daily reward notification at {fireAt}");
        }
    }

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadRewardData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        CheckForNewDay();
        if (GetNextClaimableDay() >= 0)
        {
            ScreenManager.Instance.OpenDailyRewardsUI();
        }
    }
    
    private void LoadRewardData()
    {
        // Load last claim date
        string lastClaimString = PlayerPrefs.GetString(LAST_CLAIM_DATE_KEY, "");
        if (!string.IsNullOrEmpty(lastClaimString))
        {
            DateTime.TryParse(lastClaimString, out lastClaimDate);
        }
        
        // Load claimed days
        string claimedDaysString = PlayerPrefs.GetString(CLAIMED_DAYS_KEY, "");
        if (!string.IsNullOrEmpty(claimedDaysString))
        {
            string[] days = claimedDaysString.Split(',');
            foreach (string day in days)
            {
                if (int.TryParse(day, out int dayIndex))
                {
                    claimedDays.Add(dayIndex);
                }
            }
        }
    }
    
    private void SaveRewardData()
    {
        // Save last claim date
        PlayerPrefs.SetString(LAST_CLAIM_DATE_KEY, lastClaimDate.ToString());
        
        // Save claimed days
        List<string> daysList = new List<string>();
        foreach (int day in claimedDays)
        {
            daysList.Add(day.ToString());
        }
        PlayerPrefs.SetString(CLAIMED_DAYS_KEY, string.Join(",", daysList));
        PlayerPrefs.Save();
    }
    
    private void CheckForNewDay()
    {
        DateTime today = DateTime.Today;
        
        // If it's a new day and more than 1 day has passed since last claim, reset progress
        if (lastClaimDate != DateTime.MinValue)
        {
            TimeSpan timeDifference = today - lastClaimDate.Date;
            if (timeDifference.TotalDays > 1)
            {
                ResetRewards();
            }
        }
    }
    
    public bool CanClaimReward(int dayIndex)
    {
        if (dayIndex < 0 || dayIndex >= rewards.Length)
            return false;
            
        // Can't claim if already claimed
        if (claimedDays.Contains(dayIndex))
            return false;
            
        // Day 0 can always be claimed if not already claimed
        if (dayIndex == 0)
            return true;
            
        // For other days, previous day must be claimed
        return claimedDays.Contains(dayIndex - 1);
    }
    
    public bool IsRewardClaimed(int dayIndex)
    {
        return claimedDays.Contains(dayIndex);
    }
    
    public DailyReward GetReward(int dayIndex)
    {
        if (dayIndex >= 0 && dayIndex < rewards.Length)
            return rewards[dayIndex];
        return null;
    }
    
    public bool ClaimReward(int dayIndex)
    {
        if (!CanClaimReward(dayIndex))
            return false;
            
        // Mark as claimed
        claimedDays.Add(dayIndex);
        lastClaimDate = DateTime.Today;
        
        // Save data
        SaveRewardData();
        
        // Play sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFX.CollectReward);
        }
        
        GiveReward(GetReward(dayIndex));
        
        // Trigger event
        OnRewardClaimed?.Invoke(dayIndex);
        
        return true;
    }

    private void GiveReward(DailyReward reward)
    {
        switch (reward.RewardType)
        {
            case RewardType.Money:
                CurrencyManager.Instance.AddMoney(reward.amount, false);
                break;
            case RewardType.Coin:
                CurrencyManager.Instance.AddCoin(reward.amount, false);
                break;
            default:
                break;
        }
    }
    
    public void ResetRewards()
    {
        claimedDays.Clear();
        lastClaimDate = DateTime.MinValue;
        SaveRewardData();
        OnRewardsReset?.Invoke();
    }
    
    public int GetCurrentStreak()
    {
        return claimedDays.Count;
    }
    
    public int GetNextClaimableDay()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            if (CanClaimReward(i))
                return i;
        }
        return -1; // No claimable days
    }
    
    public bool HasClaimedToday()
    {
        return lastClaimDate.Date == DateTime.Today;
    }
    
    public TimeSpan GetTimeUntilNextClaim()
    {
        if (!HasClaimedToday())
            return TimeSpan.Zero;
            
        DateTime nextClaimTime = DateTime.Today.AddDays(1);
        return nextClaimTime - DateTime.Now;
    }
}