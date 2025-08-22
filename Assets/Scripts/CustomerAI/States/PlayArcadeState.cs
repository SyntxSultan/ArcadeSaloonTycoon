using UnityEngine;

public class PlayArcadeState : ICustomerState
{
    private float playTime;
    private float playDuration;
    
    public void Enter(ICustomerContext context)
    {
        context.AnimationService.PlayInteractAnimation();
        
        playDuration = GetPlayDuration(context);
        playTime = 0f;
        
        context.ProgressBarService.ShowProgressBar();
    }

    private float GetPlayDuration(ICustomerContext context)
    {
        int coins = context.CoinsOwned;
        int coinPerUse = context.CurrentArcade.MachineData.coinPerUse;
        float playTimePerUse = context.CurrentArcade.MachineData.playTime;

        if (coinPerUse <= 0) return 0f;

        int fullPlays = coins / coinPerUse;
        int remaining = coins % coinPerUse;

        float totalDuration = fullPlays * playTimePerUse;
        if (remaining > 0)
        {
            float fraction = (float)remaining / coinPerUse;
            totalDuration += playTimePerUse * fraction;
        }

        Debug.Log($"CoinsOwned: {coins}, FullPlays: {fullPlays}, Remaining: {remaining}, TotalDuration: {totalDuration}");
        return totalDuration;
    }

    
    public void Update(ICustomerContext context)
    {
        playTime += Time.deltaTime;
        
        float progress = playDuration > 0 ? playTime / playDuration : 1f;
        context.ProgressBarService.UpdateProgress(progress);
        
        if (playTime >= playDuration)
        {
            context.CoinsOwned = 0; 
            context.ChangeState(new LeaveSaloonState());
        }
    }
    
    public void Exit(ICustomerContext context)
    {
        context.AnimationService.StopInteractAnimation();
        context.ProgressBarService.HideProgressBar();
        if (context.CurrentArcade != null)
        {
            context.ArcadeService.ReleaseArcade(context.CurrentArcade.CustomerPoint);
            context.CurrentArcade = null;
        }
    }
}
