using UnityEngine;

public class FindArcadeState : ICustomerState
{
    public void Enter(ICustomerContext context)
    {
        ArcadeMachine availableArcade = context.ArcadeService.FindAvailableArcade();
        
        if (availableArcade != null)
        {
            context.CurrentArcade = availableArcade;
            context.ArcadeService.ReserveArcade(availableArcade.CustomerPoint);
            context.ChangeState(new GoToArcadeState());
        }
        else
        {
            // No available arcade, leave store
            context.ChangeState(new LeaveSaloonState());
        }
    }
    
    public void Update(ICustomerContext context) { }
    
    public void Exit(ICustomerContext context) { }
}