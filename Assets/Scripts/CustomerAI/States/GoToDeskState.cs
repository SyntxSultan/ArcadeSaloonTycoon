public class GoToDeskState : ICustomerState
{
    public void Enter(ICustomerContext context)
    {
        context.AnimationService.PlayWalkAnimation();
        context.MovementService.MoveToTarget(context.DeskTransform, () => {
            context.ChangeState(new BuyCoinsState());
        });
    }
    
    public void Update(ICustomerContext context) { }
    
    public void Exit(ICustomerContext context)
    {
        context.AnimationService.PlayIdleAnimation();
    }
}