using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabSystem : MonoBehaviour
{
    [Header("Buttons")]
    public List<Button> tabButtons;

    [Header("Panels")]
    public List<GameObject> tabPanels;
    
    [Header("Button Colors")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;
    
    private int currentActiveTab = -1;

    void Start()
    {
        if(tabButtons.Count != tabPanels.Count)
        {
            Debug.LogError("Buton ve panel sayılarını eşitlemelisin!");
            return;
        }

        InitializeTabButtons();
        for (int i = 0; i < tabPanels.Count; i++)
        {
            tabPanels[i].SetActive(false);
            SetButtonColor(i, inactiveColor);
        }
        ShowTab(0);
    }
    
    private void InitializeTabButtons()
    {
        for(int i = 0; i < tabButtons.Count; i++)
        {
            int index = i;  
            tabButtons[i].onClick.AddListener(() => ShowTab(index));
        }
    }


    private void ShowTab(int tabIndex)
    {
        if (tabIndex == currentActiveTab) return;
        
        if (currentActiveTab >= 0)
        {
            tabPanels[currentActiveTab].SetActive(false);
            SetButtonColor(currentActiveTab, inactiveColor);
        }

        tabPanels[tabIndex].SetActive(true);
        SetButtonColor(tabIndex, activeColor);
        
        currentActiveTab = tabIndex;
    }

    private void SetButtonColor(int buttonIndex, Color color)
    {
        ColorBlock colors = tabButtons[buttonIndex].colors;
        colors.normalColor = color;
        tabButtons[buttonIndex].colors = colors;
    }

}