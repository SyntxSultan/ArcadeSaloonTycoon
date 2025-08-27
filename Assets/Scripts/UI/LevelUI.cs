using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
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

        LevelSystem.Instance.OnLevelUp += LevelSystem_OnLevelUp;
        LevelSystem.Instance.OnXpGained += LevelSystem_OnXpGained;
    }

    private void OnDestroy()
    {
        if (LevelSystem.Instance == null) return;
        LevelSystem.Instance.OnLevelUp -= LevelSystem_OnLevelUp;
        LevelSystem.Instance.OnXpGained -= LevelSystem_OnXpGained;
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
        levelText.text = $"Lv. {LevelSystem.Instance.GetCurrentLevel()}";
    }

    private void RefreshProgress()
    {
        levelProgressSlider.value = Mathf.Clamp01(LevelSystem.Instance.GetNormalizedProgressToNextLevel());
    }
}