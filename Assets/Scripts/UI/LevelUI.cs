using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider levelProgressSlider;

    private void Start()
    {
        if (LevelSystem.Instance == null)
        {
            Debug.LogError("LevelSystem.Instance yok. LevelUI çalışmayacak.");
            return;
        }

        RefreshAll();

        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
        levelSystem.OnXpGained += LevelSystem_OnXpGained;
        levelSystem.ForceSetValues += ForceRefresh;
    }

    private void OnDestroy()
    {
        if (LevelSystem.Instance == null) return;
        levelSystem.OnLevelUp -= LevelSystem_OnLevelUp;
        levelSystem.OnXpGained -= LevelSystem_OnXpGained;
        levelSystem.ForceSetValues -= ForceRefresh;
    }

    private void LevelSystem_OnXpGained(int amount, int currentXP)
    {
        RefreshProgress();
    }

    private void LevelSystem_OnLevelUp(int newLevel)
    {
        RefreshAll();
        ScreenManager.Instance.OpenLevelUpPopup();
    }

    private void RefreshAll()
    {
        RefreshLevelText();
        RefreshProgress();
    }

    private void RefreshLevelText()
    {
        levelText.text = $"Lv. {levelSystem.GetCurrentLevel()}";
    }

    private void RefreshProgress()
    {
        levelProgressSlider.value = Mathf.Clamp01(levelSystem.GetNormalizedProgressToNextLevel());
    }
    
    private void ForceRefresh()
    {
        RefreshAll();
    }
}