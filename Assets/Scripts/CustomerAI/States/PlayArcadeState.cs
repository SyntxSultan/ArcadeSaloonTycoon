using UnityEngine;

public class PlayArcadeState : ICustomerState
{
    private float playTime;
    private float playDuration;
    
    public void Enter(ICustomerContext context)
    {
        context.AnimationService.PlayInteractAnimation();
        
        playDuration = context.CoinsOwned;
        playTime = 0f;
        
        context.ProgressBarService.ShowProgressBar();
    }
    
    public void Update(ICustomerContext context)
    {
        playTime += Time.deltaTime;
        
        float progress = playDuration > 0 ? playTime / playDuration : 1f;
        context.ProgressBarService.UpdateProgress(progress);
        
        if (playTime >= playDuration)
        {
            context.CoinsOwned = 0; // Spent all coins
            context.ChangeState(new LeaveSaloonState());
        }
    }
    
    public void Exit(ICustomerContext context)
    {
        context.AnimationService.StopInteractAnimation();
        context.ProgressBarService.HideProgressBar();
        if (context.CurrentArcade != null)
        {
            context.ArcadeService.ReleaseArcade(context.CurrentArcade);
            context.CurrentArcade = null;
        }
    }
}
