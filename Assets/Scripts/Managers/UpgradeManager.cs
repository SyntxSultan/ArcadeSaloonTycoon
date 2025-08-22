using System;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    
    public event Action<MachineItemSO> OnUpgradeItemChanged;
    
    private MachineItemSO machineSO;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }

    public void SetUpgradingMachine(MachineItemSO machine)
    {
        machineSO = machine;
        OnUpgradeItemChanged?.Invoke(machineSO);
    }

    public void Reset()
    {
        machineSO = null;
    }
}
