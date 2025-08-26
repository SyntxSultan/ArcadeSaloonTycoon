using System;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    
    public event Action<ArcadeMachine> OnUpgradeItemChanged;
    
    private ArcadeMachine currentMachine;

    private void Awake()
    {
        Instance = this;
    }

    public void SetUpgradingMachine(ArcadeMachine machine)
    {
        currentMachine = machine;
        OnUpgradeItemChanged?.Invoke(machine);
        ScreenManager.Instance.OpenUpgradePanel();
    }
    
    public void ApplyUpgrade(ItemUpgradeSO upgradeSO)
    {
        if (currentMachine != null)
        {
            currentMachine.ApplyUpgrade(upgradeSO);
        }
    }

    public void Reset()
    {
        currentMachine = null;
        OnUpgradeItemChanged?.Invoke(null);
    }
}
