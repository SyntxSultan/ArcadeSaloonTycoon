using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
    public Action<int> OnRewardClaimed;
    public Action OnRewardsReset;

    private const string LAST_CLAIM_DATE_KEY = "LastClaimDate";
    private const string CLAIMED_DAYS_KEY = "ClaimedDays";

    private DateTime lastClaimDate;
    private readonly HashSet<int> claimedDays = new HashSet<int>();

    public static DailyRewardManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        LoadRewardData();
    }

    private void Start()
    {
        CheckForRewardReset();
        
        if (GetNextClaimableDay() >= 0)
        {
            //ScreenManager.Instance.OpenDailyRewardsUI();
        }
    }
    
    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            ScheduleNextRewardNotification();
        }
    }

    private void OnApplicationQuit()
    {
        ScheduleNextRewardNotification();
    }

    private void LoadRewardData()
    {
        string lastClaimString = PlayerPrefs.GetString(LAST_CLAIM_DATE_KEY, string.Empty);
        if (!string.IsNullOrEmpty(lastClaimString))
        {
            DateTime.TryParse(lastClaimString, out lastClaimDate);
        }

        string claimedDaysString = PlayerPrefs.GetString(CLAIMED_DAYS_KEY, string.Empty);
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
        PlayerPrefs.SetString(LAST_CLAIM_DATE_KEY, lastClaimDate.ToString());

        List<string> daysList = new List<string>();
        foreach (int day in claimedDays)
        {
            daysList.Add(day.ToString());
        }
        PlayerPrefs.SetString(CLAIMED_DAYS_KEY, string.Join(",", daysList));
        PlayerPrefs.Save();
    }

    private void CheckForRewardReset()
    {
        DateTime today = DateTime.Today;

        if (lastClaimDate != DateTime.MinValue)
        {
            // Son ödül Pazartesi alındıysa, Çarşamba günü girildiğinde sıfırlanır.
            TimeSpan timeDifference = today - lastClaimDate.Date;
            if (timeDifference.TotalDays > 1)
            {
                ResetRewards();
            }
        }
    }
    
    private void ScheduleNextRewardNotification()
    {
        // Eğer alınacak bir sonraki ödül yoksa (seri tamamlandıysa) bildirim kurma.
        //if (GetNextClaimableDay() < 0) return;

        TimeSpan timeUntilNextClaim = GetTimeUntilNextClaim();
        if (timeUntilNextClaim > TimeSpan.Zero)
        {
            DateTime fireTime = DateTime.Now + timeUntilNextClaim;
            string title = "Günlük ödülün hazır!";
            string body = "Hadi gel ve ödülünü al!";
            
            MobileNotificationHelper.ScheduleNotification(title, body, fireTime);
        }
    }
    
    public bool CanClaimReward(int dayIndex)
    {
        if (dayIndex < 0 || dayIndex >= rewards.Length)
            return false;
        
        // Bu günün ödülü zaten alınmışsa, tekrar alınamaz.
        if (claimedDays.Contains(dayIndex))
            return false;
        
        // Eğer bugün içinde başka bir ödül zaten alındıysa, yenisi alınamaz.
        if (HasClaimedToday())
            return false;
            
        // 0. gün (ilk gün) her zaman alınabilir (eğer daha önce alınmadıysa).
        if (dayIndex == 0)
            return true;
            
        // Diğer günler için, bir önceki günün ödülünün alınmış olması gerekir.
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

        claimedDays.Add(dayIndex);
        lastClaimDate = DateTime.Now;

        SaveRewardData();
        
        GiveReward(GetReward(dayIndex));
        
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
        }
    }

    private void ResetRewards()
    {
        claimedDays.Clear();
        PlayerPrefs.DeleteKey(CLAIMED_DAYS_KEY);
        PlayerPrefs.Save();
        OnRewardsReset?.Invoke();
    }

    public int GetNextClaimableDay()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            if (CanClaimReward(i))
                return i;
        }
        return -1;
    }
    
    private bool HasClaimedToday()
    {
        if (lastClaimDate == DateTime.MinValue) return false;
        return lastClaimDate.Date == DateTime.Today;
    }

    private TimeSpan GetTimeUntilNextClaim()
    {
        DateTime targetTime;

        if (HasClaimedToday())
        {
            targetTime = DateTime.Today.AddDays(1) + lastClaimDate.TimeOfDay;
        }
        else
        {
            targetTime = DateTime.Today.AddDays(1).AddHours(12);
        }

        return targetTime - DateTime.Now;
    }
}