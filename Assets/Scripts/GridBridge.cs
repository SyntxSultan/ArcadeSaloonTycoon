using System.Collections.Generic;
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
            Debug.Log("Confirm called");
            easyGridBuilderPro.TriggerBuildablePlacement();
            easyGridBuilderPro.SetGridModeReset();
            easyGridBuilderPro.SetSelectedBuildableGridObjectType(null);
        }
        gridObjectSelector.SetGridModeReset();
        ScreenManager.Instance.HideBuildingUI();
    }
    public void CancelBuilding()
    {
        Debug.Log("Cancel called");
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.TriggerBuildablePlacementCancelled();
            easyGridBuilderPro.SetGridModeReset();
            easyGridBuilderPro.SetSelectedBuildableGridObjectType(null);
        }
        gridObjectSelector.SetGridModeReset();
        ScreenManager.Instance.HideBuildingUI();
    }
    public void RotateBuilding()
    {
        Debug.Log("Rotate called");
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.TriggerGhostRotateRight();
            easyGridBuilderPro.TriggerGhostRotateRightCancelled();
        }
    }
}
