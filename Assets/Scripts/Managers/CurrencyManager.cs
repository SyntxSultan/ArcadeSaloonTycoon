using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, IJsonSaveable
{
    public static CurrencyManager Instance { get; private set; }
    
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnCoinChanged;
    
    [SerializeField] private int money = 100;
    [SerializeField] private int coin = 10;
    [SerializeField] private int moneyPerCoin = 10;

    public int GetCoin() => coin;
    public int GetMoney() => money;
    
    private ItemSO cachedItem;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        OnMoneyChanged?.Invoke(money);
        OnCoinChanged?.Invoke(coin);
    }
    
    /* Public Setters */
    public void AddMoney(int amount, bool playSound = true)
    {
        money += amount;
        if (playSound && AudioManager.Instance) AudioManager.Instance.PlaySFX(SFX.GainMoney);
        OnMoneyChanged?.Invoke(money);
        QuestManager.Instance.UpdateQuestProgress(QuestType.EarnMoney, amount);
    }
    public void RemoveMoney(int amount)
    {
        money -= amount;
        if (AudioManager.Instance) AudioManager.Instance.PlaySFX(SFX.LoseMoney);
        OnMoneyChanged?.Invoke(money);
    }
    public void AddCoin(int amount, bool playSound = true)
    {
        coin += amount;
        if (playSound && AudioManager.Instance) AudioManager.Instance.PlaySFX(SFX.GainCoin);
        OnCoinChanged?.Invoke(coin);
    }
    public void RemoveCoin(int amount)
    {
        coin -= amount;
        OnCoinChanged?.Invoke(coin);
    }

    public void SetMoneyPerCoin(int amount)
    {
        moneyPerCoin = amount;
    }

    /* Public Getters */
    public bool CanBuy(int cost)
    {
        return money >= cost;
    }

    public bool CanBuyCoin(int amount)
    {
        return coin >= amount;
    }

    public int GetMoneyForCoin(int coinAmount)
    {
        return coinAmount * moneyPerCoin;
    }

    public int GetMoneyPerCoin()
    {
        return moneyPerCoin;
    }

    //Caching For purchase 
    public void SetPlacingItemForMoneyDraw(ItemSO itemSO)
    {
        cachedItem = itemSO;
    }

    public void WithdrawMoneyFromCachedItem()
    {
        RemoveMoney(cachedItem.cost);
    }

    //Saving
    public JToken CaptureAsJToken()
    {
        JObject state = new JObject();
        state["money"] = money;
        state["coin"] = coin;
        return state;
    }

    public void RestoreFromJToken(JToken state)
    {
        JObject stateObj = (JObject)state;
        if (stateObj.TryGetValue("money", out JToken moneyToken))
        {
            money = moneyToken.Value<int>();
        }
        if (stateObj.TryGetValue("coin", out JToken coinToken))
        {
            coin = coinToken.Value<int>();
        }
    }
}
