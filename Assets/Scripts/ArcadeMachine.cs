using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeMachine : MonoBehaviour
{
    [SerializeField] private MachineItemSO machinesData;
    [SerializeField] private Transform customerPoint;
    [SerializeField] private Camera playerCamera;
    
    public Transform CustomerPoint => customerPoint;
    public MachineItemSO MachineData => machinesData;
    
    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (machinesData == null)
        {
            Debug.LogError("Machine data not set!" + gameObject.name);
        }
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
        ScreenManager.Instance.OpenUpgradePanel();
        UpgradeManager.Instance.SetUpgradingMachine(machinesData);
    }
}
