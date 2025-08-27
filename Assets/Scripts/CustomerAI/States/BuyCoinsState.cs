using UnityEngine;

public class BuyCoinsState : ICustomerState
{
    private float purchaseStartTime;
    private float purchaseDuration;
    private bool purchaseStarted;
    
    public void Enter(ICustomerContext context)
    {
        context.AnimationService.PlayInteractAnimation();
        context.ProgressBarService.ShowProgressBar();
        
        int coinsToBuy = context.PurchaseService.GetRandomCoinAmount();
        
        if (context.PurchaseService.CanPurchase(coinsToBuy))
        {
            purchaseStarted = true;
            purchaseStartTime = Time.time;
            purchaseDuration = context.PurchaseService.PurchaseDelay;
            
            context.PurchaseService.ProcessPurchase(coinsToBuy, (success) => {
                if (success)
                {
                    context.CoinsOwned = coinsToBuy;
                    context.ChangeState(new FindArcadeState());
                }
                else
                {
                    context.ChangeState(new LeaveSaloonState());
                }
            });
        }
        else
        {
            context.ChangeState(new LeaveSaloonState());
        }
    }

    public void Update(ICustomerContext context)
    {
        if (purchaseStarted)
        {
            float elapsedTime = Time.time - purchaseStartTime;
            float progress = Mathf.Clamp01(elapsedTime / purchaseDuration);
            context.ProgressBarService.UpdateProgress(progress);
        }
    }

    public void Exit(ICustomerContext context)
    {
        LevelSystem.Instance.GainXP(10);
        context.AnimationService.StopInteractAnimation();
        context.ProgressBarService.HideProgressBar();
    }
}