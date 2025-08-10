
public class LeaveSaloonState : ICustomerState
{
    public void Enter(ICustomerContext context)
    {
        context.AnimationService.PlayWalkAnimation();
        context.MovementService.MoveToTarget(context.ExitTransform, () => {
            context.CompleteCustomerCycle();
        });
    }
    
    public void Update(ICustomerContext context) { }
    
    public void Exit(ICustomerContext context) { }
}
