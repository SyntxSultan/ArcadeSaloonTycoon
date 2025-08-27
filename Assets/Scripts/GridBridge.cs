using System.Collections.Generic;
using System.Linq;
using SoulGames.EasyGridBuilderPro;
using UnityEngine;

public class GridBridge : MonoBehaviour
{
    public static GridBridge Instance { get; private set; }
    
    [SerializeField]private GridObjectSelector gridObjectSelector;
    
    private List<EasyGridBuilderPro> gridList;
    
    private void Start()
    {
        Instance = this;
        gridList = MultiGridManager.Instance.easyGridBuilderProList;
    }
    
    public void OnBuildableItemBought(BuildableGridObjectTypeSO buildItemSO)
    {
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.SetGridModeReset();
            easyGridBuilderPro.SetSelectedBuildableGridObjectType(buildItemSO);
            easyGridBuilderPro.SetGridModeBuilding();
        }
    }

    public void ConfirmBuilding()
    {
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.TriggerBuildablePlacement();
            easyGridBuilderPro.SetGridModeReset();
            easyGridBuilderPro.SetSelectedBuildableGridObjectType(null);
        }
        gridObjectSelector.SetGridModeReset();
        ScreenManager.Instance.HideBuildingUI();
        CurrencyManager.Instance.WithdrawMoneyFromCachedItem();
        AudioManager.Instance?.PlaySFX(SFX.LoseMoney);
        LevelSystem.Instance.GainXP(25);
        SaveManager.Instance.Save();
        SaveManager.Instance.SaveGrid();
    }
    public void CancelBuilding()
    {
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.TriggerBuildablePlacementCancelled();
            easyGridBuilderPro.SetGridModeReset();
            easyGridBuilderPro.SetSelectedBuildableGridObjectType(null);
        }
        gridObjectSelector.SetGridModeReset();
        ScreenManager.Instance.HideBuildingUI();
        CurrencyManager.Instance.SetPlacingItemForMoneyDraw(null);
    }
    public void RotateBuilding()
    {
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.TriggerGhostRotateRight();
            easyGridBuilderPro.TriggerGhostRotateRightCancelled();
        }
    }

    public bool IsGridModeBuilding()
    {
        return gridList.Select(easyGridBuilderPro => easyGridBuilderPro.GetGridMode() == GridMode.Build).FirstOrDefault();
    }
}
