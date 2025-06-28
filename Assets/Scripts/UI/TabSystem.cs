using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabSystem : MonoBehaviour
{
    [Header("Buttons")]
    public List<Button> tabButtons;

    [Header("Panels")]
    public List<GameObject> tabPanels;

    void Start()
    {
        if(tabButtons.Count != tabPanels.Count)
        {
            Debug.LogError("Buton ve panel sayılarını eşitlemelisin!");
            return;
        }
        ShowTab(0);
        for(int i = 0; i < tabButtons.Count; i++)
        {
            int index = i;  
            tabButtons[i].onClick.AddListener(() => ShowTab(index));
        }
    }

    public void ShowTab(int tabIndex)
    {
        for(int i = 0; i < tabPanels.Count; i++)
        {
            bool isActive = (i == tabIndex);
            tabPanels[i].SetActive(isActive);

            ColorBlock colors = tabButtons[i].colors;
            colors.normalColor = isActive ? Color.white : Color.gray;
            tabButtons[i].colors = colors;
        }
    }
}