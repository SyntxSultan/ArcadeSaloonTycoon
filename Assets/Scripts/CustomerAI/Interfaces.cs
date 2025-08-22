using UnityEngine;

public interface IMovementService
{
    void MoveToTarget(Transform target, System.Action onReached);
    bool IsMoving { get; }
    void Stop();
}

public interface IAnimationService
{
    void PlayWalkAnimation();
    void PlayIdleAnimation();  
    void PlayInteractAnimation();
    void StopInteractAnimation();
}

public interface IPurchaseService
{
    int GetRandomCoinAmount();
    bool CanPurchase(int amount);
    void ProcessPurchase(int amount, System.Action<bool> onComplete);
    float PurchaseDelay { get; }
}

public interface IArcadeService
{
    ArcadeMachine FindAvailableArcade();
    void ReserveArcade(Transform arcade);
    void ReleaseArcade(Transform arcade);
}

public interface ICustomerState
{
    void Enter(ICustomerContext context);
    void Update(ICustomerContext context);
    void Exit(ICustomerContext context);
}

public interface IProgressBarService
{
    void ShowProgressBar();
    void UpdateProgress(float progress); // 0.0f to 1.0f
    void HideProgressBar();
    bool IsVisible { get; }
}

public interface ICustomerContext
{
    IMovementService MovementService { get; }
    IAnimationService AnimationService { get; }
    IPurchaseService PurchaseService { get; }
    IArcadeService ArcadeService { get; }
    IProgressBarService ProgressBarService { get; }
    
    Transform DeskTransform { get; }
    Transform ExitTransform { get; }
    ArcadeMachine CurrentArcade { get; set; }
    int CoinsOwned { get; set; }
    
    void ChangeState(ICustomerState newState);
    void CompleteCustomerCycle();
}
