using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, IJsonSaveable
{
    [SerializeField] private CurrencyUI currencyUI;
    
    [SerializeField] private float money = 100f;
    [SerializeField] private float coin = 10f;


    private void Start()
    {
        currencyUI.UpdateMoneyText(money);
        currencyUI.UpdateCoinText(coin);
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
            money = moneyToken.Value<float>();
        }
        if (stateObj.TryGetValue("coin", out JToken coinToken))
        {
            coin = coinToken.Value<float>();
        }
    }
}
