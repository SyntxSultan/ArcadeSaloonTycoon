using UnityEngine;

public class CustomerAI : MonoBehaviour, ICustomerContext
{
    [Header("Services")]
    [SerializeField] private MovementService movementService;
    [SerializeField] private AnimationService animationService;
    [SerializeField] private CoinPurchaseService purchaseService;
    [SerializeField] private ArcadeMachineService arcadeService;
    [SerializeField] private ProgressBarService progressBarService;
    
    // ICustomerContext Properties
    public IMovementService MovementService => movementService;
    public IAnimationService AnimationService => animationService;
    public IPurchaseService PurchaseService => purchaseService;
    public IArcadeService ArcadeService => arcadeService;
    public IProgressBarService ProgressBarService => progressBarService;
    
    public Transform DeskTransform { get; private set; }
    public Transform ExitTransform { get; private set; }

    public Transform CurrentArcade { get; set; }
    public int CoinsOwned { get; set; }
    
    private ICustomerState currentState;

    public System.Action<CustomerAI> OnCustomerCompleted;
    
    private void Start()
    {
        InitializeServices();
        StartCustomerCycle();
    }

    public void SetTransforms(Transform desk, Transform exit)
    {
        DeskTransform = desk;
        ExitTransform = exit;
    }
    
    private void InitializeServices()
    {
        if (movementService == null)
            movementService = GetComponent<MovementService>() ?? gameObject.AddComponent<MovementService>();
        
        if (animationService == null)
            animationService = GetComponent<AnimationService>() ?? gameObject.AddComponent<AnimationService>();
        
        if (progressBarService == null)
            progressBarService = GetComponent<ProgressBarService>() ?? gameObject.AddComponent<ProgressBarService>();
        
        if (purchaseService == null)
            purchaseService = FindFirstObjectByType<CoinPurchaseService>();
        
        if (arcadeService == null)
            arcadeService = FindFirstObjectByType<ArcadeMachineService>();
    }
    
    public void StartCustomerCycle()
    {
        ChangeState(new GoToDeskState());
    }
    
    public void ChangeState(ICustomerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }
    
    public void CompleteCustomerCycle()
    {
        OnCustomerCompleted?.Invoke(this);
        // Destroy customer or return to pool
        Destroy(gameObject);
    }
    
    private void Update()
    {
        currentState?.Update(this);
    }
    
    private void OnDestroy()
    {
        currentState?.Exit(this);
        
        // Clean up any reserved arcade
        if (CurrentArcade != null)
        {
            ArcadeService?.ReleaseArcade(CurrentArcade);
        }
    }
}
