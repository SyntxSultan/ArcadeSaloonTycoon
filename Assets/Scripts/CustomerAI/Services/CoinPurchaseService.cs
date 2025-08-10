using UnityEngine;
using System;

public class CoinPurchaseService : MonoBehaviour, IPurchaseService
{
    [SerializeField] private int minCoins = 5;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private float purchaseDelay = 2f;
    
    public float PurchaseDelay => purchaseDelay;
    
    public int GetRandomCoinAmount()
    {
        return UnityEngine.Random.Range(minCoins, maxCoins + 1);
    }
    
    public bool CanPurchase(int amount)
    {
        return amount > 0; // In a real scenario, check player money
    }

    public void ProcessPurchase(int amount, Action<bool> onComplete)
    {
        StartCoroutine(ProcessPurchaseCoroutine(amount, onComplete));
    }

    private System.Collections.IEnumerator ProcessPurchaseCoroutine(int amount, Action<bool> onComplete)
    {
        yield return new WaitForSeconds(purchaseDelay);
        //Check if player had enough coins   
        onComplete?.Invoke(true);
    }
}
