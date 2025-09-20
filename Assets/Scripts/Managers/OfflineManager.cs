using System;
using UnityEngine;

public class OfflineManager : MonoBehaviour
{
    private void Start()
    {
        ScreenManager.Instance.OpenOfflineIncomeUI();
    }
}
