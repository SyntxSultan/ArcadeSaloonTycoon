
public class GoToArcadeState : ICustomerState
{
    public void Enter(ICustomerContext context)
    {
        context.AnimationService.PlayWalkAnimation();
        context.MovementService.MoveToTarget(context.CurrentArcade, () => {
            context.ChangeState(new PlayArcadeState());
        });
    }
    
    public void Update(ICustomerContext context) { }
    
    public void Exit(ICustomerContext context)
    {
        context.AnimationService.PlayIdleAnimation();
    }
}
