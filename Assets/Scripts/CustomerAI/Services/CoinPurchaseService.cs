using UnityEngine;
using System;

public class CoinPurchaseService : MonoBehaviour, IPurchaseService
{
    [SerializeField] private int minCoins = 5;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private float purchaseDelay = 2f;
    
    private CurrencyManager currencyManager;
    
    public float PurchaseDelay => purchaseDelay;

    private void Start()
    {
        currencyManager = CurrencyManager.Instance;
    }

    public int GetRandomCoinAmount()
    {
        int customerWantedCoins = UnityEngine.Random.Range(minCoins, maxCoins + 1);
        return currencyManager.CanBuyCoin(customerWantedCoins) ? customerWantedCoins : currencyManager.GetCoin();
    }
    
    public bool CanPurchase(int amount)
    {
        return true;
    }

    public void ProcessPurchase(int amount, Action<bool> onComplete)
    {
        StartCoroutine(ProcessPurchaseCoroutine(amount, onComplete));
    }

    private System.Collections.IEnumerator ProcessPurchaseCoroutine(int amount, Action<bool> onComplete)
    {
        yield return new WaitForSeconds(purchaseDelay);
        //TODO
        //currencyManager.RemoveCoin(amount);
        currencyManager.AddMoney(currencyManager.GetMoneyForCoin(amount));
        onComplete?.Invoke(true);
    }
}
