using System;
using UnityEngine;
using System.Globalization;
using TMPro;
using UnityEngine.UI;

public class ReviewUI : MonoBehaviour
{
    [SerializeField] private ReviewSystem reviewSystem;

    [SerializeField] private Button closeButton;
    
    [Header("Prefabs & Parents")]
    [SerializeField] private RectTransform contentParent;           // ScrollRect.content where review items will be placed
    [SerializeField] private GameObject reviewItemPrefab;           // Prefab that has ReviewItemUI component

    [Header("Star Prefabs")]
    [SerializeField] private GameObject starPrefab;                 // filled star prefab (Image only, no color changes in code)
    [SerializeField] private GameObject emptyStarPrefab;            // empty star prefab

    [Header("Overall UI")]
    [SerializeField] private TextMeshProUGUI totalReviewsText;
    [SerializeField] private TextMeshProUGUI averageStarsText;    // show like "4.25"
    [SerializeField] private RectTransform starContainer;        // place to instantiate star/emptyStar prefabs (Horizontal Layout recommended)

    private void OnEnable()
    {
        if (reviewSystem == null)
        {
            Debug.LogWarning("ReviewSystem not found in scene.");
            return;
        }

        closeButton.onClick.AddListener(OnCloseButtonClicked);
        reviewSystem.OnReviewAdded += HandleNewReview;
        reviewSystem.OnReviewsChanged += RefreshAll;
        RefreshAll();
    }

    private void OnDisable()
    {
        if (reviewSystem == null) return;
        reviewSystem.OnReviewAdded -= HandleNewReview;
        reviewSystem.OnReviewsChanged -= RefreshAll;
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked()
    {
        ScreenManager.Instance?.CloseReviewsUI();
    }

    // Called for each new review to append it (cheap)
    private void HandleNewReview(Review r)
    {
        if (reviewItemPrefab == null || contentParent == null)
        {
            Debug.LogWarning("ReviewUI not fully configured.");
            return;
        }
        var go = Instantiate(reviewItemPrefab, contentParent);
        var item = go.GetComponent<ReviewItem>();
        if (item == null)
        {
            Debug.LogError("reviewItemPrefab must have ReviewItemUI component.");
            return;
        }
        item.Setup(r, starPrefab, emptyStarPrefab);
        UpdateOverallDisplay();
    }

    // Rebuilds whole list (use at start or when cleared)
    public void RefreshAll()
    {
        if (contentParent == null || reviewItemPrefab == null) return;
        // clear existing children
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        if (reviewSystem == null) return;

        foreach (var r in reviewSystem.GetAllReviews())
        {
            var go = Instantiate(reviewItemPrefab, contentParent);
            var item = go.GetComponent<ReviewItem>();
            item.Setup(r, starPrefab, emptyStarPrefab);
        }

        UpdateOverallDisplay();
    }

    private void UpdateOverallDisplay()
    {
        if (reviewSystem == null) return;

        int total = reviewSystem.GetTotalReviews();
        double avg = reviewSystem.GetAverageStars(); // e.g. 4.333333
        
        UpdateOverallStars();

        if (totalReviewsText != null) totalReviewsText.text = total.ToString();
        if (averageStarsText != null) averageStarsText.text = avg.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private void UpdateOverallStars()
    {
        int intAverage = Mathf.RoundToInt(reviewSystem.GetAverageStars());
        
        foreach (Transform child in starContainer.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < intAverage; i++)
        {
            Instantiate(starPrefab, starContainer.transform);
        }
        for (int i = 0; i < 5 - intAverage; i++)
        {
            Instantiate(emptyStarPrefab, starContainer.transform);
        }
    }
}
