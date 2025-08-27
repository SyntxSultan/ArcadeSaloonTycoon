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
        int coinPerUse = context.CurrentArcade.CurrentCoinPerUse;
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
        int random = Random.Range(1, 3);
        if (random == 1)
        {
            ReviewSystem.MakeReview("Boring", "I feel bored in this place. Buy some new arcades.", 2, LikedEnum.Dislike);
        }
        else
        {
            ReviewSystem.MakeReview("Best Place In The World", "This place is awesome. I will definitely come back.", 4, LikedEnum.Like);
        }
    }
}
