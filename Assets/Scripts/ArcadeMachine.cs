using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeMachine : MonoBehaviour
{
    [SerializeField] private MachineItemSO machinesData;
    [SerializeField] private Transform customerPoint;
    [SerializeField] private Camera playerCamera;
    
    [Header("Runtime Values")]
    [SerializeField][ReadOnly] private int currentMaxCoinCapacity;
    [SerializeField][ReadOnly] private int currentCoinPerUse;
    [SerializeField][ReadOnly] private int currentDurability;
    [SerializeField][ReadOnly] private int currentPlayTime;
    [SerializeField][ReadOnly] private int currentOfflineIncome;
    
    public Transform CustomerPoint => customerPoint;
    public MachineItemSO MachineData => machinesData;
    
    public int CurrentMaxCoinCapacity => currentMaxCoinCapacity;
    public int CurrentCoinPerUse => currentCoinPerUse;
    public int CurrentDurability => currentDurability;
    public int CurrentPlayTime => currentPlayTime;
    public int CurrentOfflineIncome => currentOfflineIncome;
    
    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (machinesData == null)
        {
            Debug.LogError("Machine data not set!" + gameObject.name);
            return;
        }
        
        InitializeMachineValues();
    }
    
    private void InitializeMachineValues()
    {
        currentMaxCoinCapacity = machinesData.maxCoinCapacity;
        currentCoinPerUse = machinesData.coinPerUse;
        currentDurability = machinesData.durability;
        currentPlayTime = machinesData.playTime;
        currentOfflineIncome = machinesData.offlineIncome;
    }
    
    public void ApplyUpgrade(ItemUpgradeSO upgradeSO)
    {
        switch (upgradeSO.upgradeType)
        {
            case UpgradeType.CoinPerUse:
                currentCoinPerUse += upgradeSO.upgradeValue;
                break;
            case UpgradeType.Durability:
                currentDurability += upgradeSO.upgradeValue;
                break;
            case UpgradeType.OfflineIncome:
                currentOfflineIncome += upgradeSO.upgradeValue;
                break;
        }
        
        Debug.Log($"{upgradeSO.upgradeName} upgrade'i uygulandı!");
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame) return;
        if (ASTLibrary.IsPointerOverUI()) return;
        CheckRaycast();
    }
    
    private void CheckRaycast()
    {
        if (Pointer.current == null)
        {
            Debug.LogError("Pointer bulunamadı!");
            return;
        }
        
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = playerCamera.ScreenPointToRay(screenPosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                OnMachineTouched();
            }
        }
    }
    
    private void OnMachineTouched()
    {
        UpgradeManager.Instance.SetUpgradingMachine(this);
    }
}
