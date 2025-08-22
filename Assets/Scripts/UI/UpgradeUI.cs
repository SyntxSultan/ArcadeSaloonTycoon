using System;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;
    
    private MachineItemSO upgradeItem;

    private void Start()
    {
        upgradeManager.OnUpgradeItemChanged += UpgradeManager_OnUpgradeItemChanged;
    }

    private void UpgradeManager_OnUpgradeItemChanged(MachineItemSO machineItemSO)
    {
        upgradeItem = machineItemSO;
    }
}
