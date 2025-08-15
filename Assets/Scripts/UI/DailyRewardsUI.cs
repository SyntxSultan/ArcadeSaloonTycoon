using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // DOTween

public class DailyRewardsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button[] dayButtons;                // button0 == Day 1
    [SerializeField] private RectTransform daysPopup;           // container with all day buttons
    [SerializeField] private RectTransform collectPopup;        // popup shown after collecting or info
    [SerializeField] private TMP_Text collectText;              // text inside collectPopup (no icon)
    [SerializeField] private Button CloseAllButton;             // closes collectPopup and returns to days popup

    [Header("Visual States")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color claimedColor = Color.green;
    [SerializeField] private Color lockedColor = Color.gray;

    [Header("Highlight (pulse) settings")]
    [SerializeField, Tooltip("Scale multiplier for the highlighted (claimable) button.")]
    private float highlightScale = 1.08f;
    [SerializeField, Tooltip("Time (seconds) for one half of the pulse (up or down).")]
    private float pulseHalfDuration = 0.25f;
    [SerializeField, Tooltip("If true, pulse starts automatically when UI opens.")]
    private bool autoHighlightOnOpen = true;

    private int selectedDayIndex = -1;

    // DOTween tween & index
    private Tween highlightTween = null;
    private int highlightedIndex = -1;

    private void Start()
    {
        SetupButtons();
        UpdateUI();

        if (DailyRewardManager.Instance != null)
        {
            DailyRewardManager.Instance.OnRewardClaimed += OnRewardClaimed;
            DailyRewardManager.Instance.OnRewardsReset += OnRewardsReset;
        }

        if (autoHighlightOnOpen)
            UpdateHighlight(); // start highlight for current claimable day
    }

    private void OnDestroy()
    {
        if (DailyRewardManager.Instance != null)
        {
            DailyRewardManager.Instance.OnRewardClaimed -= OnRewardClaimed;
            DailyRewardManager.Instance.OnRewardsReset -= OnRewardsReset;
        }

        StopHighlight();
    }

    private void SetupButtons()
    {

        if (CloseAllButton != null)
            CloseAllButton.onClick.AddListener(CloseCollectPopup);

        // Setup day buttons
        for (int i = 0; i < dayButtons.Length; i++)
        {
            int dayIndex = i; // capture
            Button b = dayButtons[i];
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => OnDayButtonClicked(dayIndex));
        }
    }

    private void UpdateUI()
    {
        if (DailyRewardManager.Instance == null)
            return;

        UpdateDayButtons();

        // Ensure popups state: show days popup by default
        if (daysPopup != null)
            daysPopup.gameObject.SetActive(true);
        if (collectPopup != null)
            collectPopup.gameObject.SetActive(false);
    }

    private void UpdateDayButtons()
    {
        for (int i = 0; i < dayButtons.Length; i++)
            UpdateDayButton(i);

        // refresh highlight after buttons updated (colors/scales may have been reset)
        UpdateHighlight();
    }

    private void UpdateDayButton(int dayIndex)
    {
        if (dayIndex < 0 || dayIndex >= dayButtons.Length)
            return;

        Button button = dayButtons[dayIndex];
        Image img = button.GetComponent<Image>();
        TMP_Text label = button.GetComponentInChildren<TMP_Text>();

        if (label != null)
            label.text = $"Day {dayIndex + 1}";

        if (DailyRewardManager.Instance.IsRewardClaimed(dayIndex))
        {
            if (img != null) img.color = claimedColor;
            button.interactable = false;
        }
        else if (DailyRewardManager.Instance.CanClaimReward(dayIndex))
        {
            if (img != null) img.color = availableColor;
            button.interactable = true;
        }
        else
        {
            if (img != null) img.color = lockedColor;
            button.interactable = false;
        }

        // If this button is not the highlighted one, ensure scale reset
        if (dayIndex != highlightedIndex && button != null)
            button.transform.localScale = Vector3.one;
    }

    private void OnDayButtonClicked(int dayIndex)
    {
        selectedDayIndex = dayIndex;

        if (DailyRewardManager.Instance == null)
            return;

        // If can claim immediately, claim and show collect popup
        if (DailyRewardManager.Instance.CanClaimReward(dayIndex))
        {
            bool success = DailyRewardManager.Instance.ClaimReward(dayIndex);
            if (success)
            {
                // The manager fires OnRewardClaimed and handles SFX; we show confirmation popup
                ShowCollectPopup(dayIndex, claimed: true);
            }
            else
            {
                // Shouldn't normally happen, but fallback
                ShowCollectPopup(dayIndex, claimed: false, message: "Could not claim reward.");
            }
        }
        else if (DailyRewardManager.Instance.IsRewardClaimed(dayIndex))
        {
            // Already claimed - show info popup
            ShowCollectPopup(dayIndex, claimed: false, message: "Already claimed.");
        }
        else
        {
            // Locked - show info what reward would be
            ShowCollectPopup(dayIndex, claimed: false, message: "Locked. Claim previous days first.");
        }
    }

    private void ShowCollectPopup(int dayIndex, bool claimed, string message = null)
    {
        DailyReward reward = DailyRewardManager.Instance.GetReward(dayIndex);

        if (daysPopup != null)
            daysPopup.gameObject.SetActive(false);
        if (collectPopup != null)
            collectPopup.gameObject.SetActive(true);

        if (collectText != null)
        {
            if (claimed && reward != null)
            {
                collectText.text = $"{reward.rewardName} x{reward.amount}\nCollected!";
            }
            else if (!string.IsNullOrEmpty(message))
            {
                // show fallback message
                if (reward != null)
                    collectText.text = $"{reward.rewardName} x{reward.amount}\n{message}";
                else
                    collectText.text = message;
            }
            else
            {
                collectText.text = "Info";
            }
        }

        // Update visuals for the day button that changed
        UpdateDayButton(dayIndex);
    }

    private void CloseCollectPopup()
    {
        if (collectPopup != null)
            collectPopup.gameObject.SetActive(false);
        if (daysPopup != null)
            daysPopup.gameObject.SetActive(true);

        selectedDayIndex = -1;
        UpdateDayButtons();
    }

    private void OnRewardClaimed(int dayIndex)
    {
        UpdateDayButton(dayIndex);

        // After claiming, highlight might need to move to next claimable day
        UpdateHighlight();
    }

    private void OnRewardsReset()
    {
        selectedDayIndex = -1;
        UpdateUI();

        // reset highlight
        UpdateHighlight();
    }

    private void OnEnable()
    {
        UpdateUI();

        if (autoHighlightOnOpen)
            UpdateHighlight();
    }

    // ---------- Highlight logic (DOTween) ----------

    /// <summary>
    /// Call this to start/refresh highlight of the next claimable day.
    /// </summary>
    public void UpdateHighlight()
    {
        if (DailyRewardManager.Instance == null || dayButtons == null || dayButtons.Length == 0)
        {
            StopHighlight();
            return;
        }

        int next = DailyRewardManager.Instance.GetNextClaimableDay();

        if (next < 0 || next >= dayButtons.Length)
        {
            StopHighlight();
            return;
        }

        if (highlightedIndex == next && highlightTween != null && highlightTween.IsActive())
            return;

        StopHighlight();
        highlightedIndex = next;
        StartHighlightDOTween(dayButtons[next]);
    }

    private void StartHighlightDOTween(Button button)
    {
        if (button == null) return;

        // Ensure scale reset before starting
        button.transform.localScale = Vector3.one;

        // Create a looping yoyo scale tween
        highlightTween = button.transform.DOScale(Vector3.one * highlightScale, pulseHalfDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // Make sure button stays interactable
        button.interactable = true;
    }

    private void StopHighlight()
    {
        if (highlightTween != null)
        {
            if (highlightTween.IsActive()) highlightTween.Kill(true);
            highlightTween = null;
        }

        if (highlightedIndex >= 0 && highlightedIndex < dayButtons.Length && dayButtons[highlightedIndex] != null)
        {
            dayButtons[highlightedIndex].transform.localScale = Vector3.one;
        }

        highlightedIndex = -1;
    }

}
