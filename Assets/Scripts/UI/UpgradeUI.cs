using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;
    
    [Header("UI Field")]
    [SerializeField] private TextMeshProUGUI itemNameText;

    [SerializeField] private ItemStatRow stat1;
    [SerializeField] private ItemStatRow stat2;
    [SerializeField] private ItemStatRow stat3;

    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI upgradePriceText;
    [SerializeField] private Button buyButton;
    
    [SerializeField] private Transform upgradeButtonsContainer;
    [SerializeField] private GameObject upgradeButtonPrefab;
    
    private ArcadeMachine currentMachine;
    private ItemUpgradeSO cachedUpgradeSO;
    
    private List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

    private void Start()
    {
        upgradeManager.OnUpgradeItemChanged += UpgradeManager_OnUpgradeItemChanged;
        buyButton.onClick.AddListener(BuyUpgrade);
    }

    private void UpgradeManager_OnUpgradeItemChanged(ArcadeMachine arcadeMachine)
    {
        if (arcadeMachine != null)
        {
            currentMachine = arcadeMachine;
            UpdateUI();
        }
        else
        {
            currentMachine = null;
            cachedUpgradeSO = null;
            upgradeButtons.Clear();
            foreach (Transform ub in upgradeButtonsContainer)
            {
                Destroy(ub.gameObject);
            }
        }
    }

    private void BuyUpgrade()
    {
        if (cachedUpgradeSO == null)
        {
            Debug.LogError("Upgrade SO is null!");
            return;
        }
        if (currentMachine == null)
        {
            Debug.LogError("Current machine is null!");
            return;
        }

        
        upgradeManager.ApplyUpgrade(cachedUpgradeSO);
        CurrencyManager.Instance.RemoveMoney(cachedUpgradeSO.upgradeCost);
        
        UpdateUI();
        buyButton.interactable = CanBuyUpgrade(cachedUpgradeSO);
        UpdateAllButtonsCanUpgrade();
    }

    private void UpdateUI()
    {
        itemNameText.text = currentMachine.MachineData.gridItemData.objectName;
        
        stat1.SetStat(currentMachine.CurrentCoinPerUse);
        stat2.SetStat(currentMachine.CurrentDurability);
        stat3.SetStat(currentMachine.CurrentOfflineIncome);
        
        if (currentMachine.MachineData.upgrades == null || currentMachine.MachineData.upgrades.Length == 0)
        {
            Debug.LogError("No upgrades available for this machine!");
            return;
        }

        
        cachedUpgradeSO = currentMachine.MachineData.upgrades[0];
        UpdateUpgradeDetails(cachedUpgradeSO);
        InstantiateUpgradeButtons();        
    }

    private void InstantiateUpgradeButtons()
    {
        upgradeButtons.Clear();
        
        foreach (Transform obj in upgradeButtonsContainer)
        {
            Destroy(obj.gameObject);
        }
        
        for (int i = 0; i < currentMachine.MachineData.upgrades.Length; i++)
        {
            var instantiatedButton = Instantiate(upgradeButtonPrefab, upgradeButtonsContainer);
            var canUpgrade = CurrencyManager.Instance.GetMoney() >= currentMachine.MachineData.upgrades[i].upgradeCost;
            instantiatedButton.GetComponent<UpgradeButton>().Initialize(currentMachine.MachineData.upgrades[i], 1, canUpgrade, OnUpgradeButtonClicked);
            upgradeButtons.Add(instantiatedButton.GetComponent<UpgradeButton>());
            if (i == 0)
            {
                instantiatedButton.GetComponent<Button>().Select();
            }
        }
    }

    private void OnUpgradeButtonClicked(ItemUpgradeSO upgradeSO)
    {
        cachedUpgradeSO = upgradeSO;
        
        UpdateBuyButton();
        
        UpdateUpgradeDetails(upgradeSO);
    }

    private bool CanBuyUpgrade(ItemUpgradeSO upgradeSO)
    {
        return CurrencyManager.Instance.CanBuy(upgradeSO.upgradeCost);
    }

    private void UpdateUpgradeDetails(in ItemUpgradeSO upgradeSO)
    {
        upgradeNameText.text = upgradeSO.upgradeName;
        upgradeDescriptionText.text = upgradeSO.upgradeDescription;
        upgradeIcon.sprite = upgradeSO.upgradeIcon;
        upgradePriceText.text = upgradeSO.upgradeCost.ToString();
        UpdateAllButtonsCanUpgrade();
    }

    private void UpdateAllButtonsCanUpgrade()
    {
        UpdateBuyButton();
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].SetCanUpgrade(CurrencyManager.Instance.CanBuy(currentMachine.MachineData.upgrades[i].upgradeCost));
        }
    }

    private void UpdateBuyButton()
    {
        buyButton.interactable = CanBuyUpgrade(cachedUpgradeSO);
    }
}
