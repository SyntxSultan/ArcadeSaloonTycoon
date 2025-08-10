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
    
    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }
    public void RemoveMoney(int amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke(money);
    }
    public void AddCoin(int amount)
    {
        coin += amount;
        OnCoinChanged?.Invoke(coin); 
    }

    public bool CanBuy(int cost)
    {
        return money >= cost;
    }

    public void SetPlacingItemForMoneyDraw(ItemSO itemSO)
    {
        cachedItem = itemSO;
    }

    public void WithdrawMoneyFromCachedItem()
    {
        RemoveMoney(cachedItem.cost);
    }

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
