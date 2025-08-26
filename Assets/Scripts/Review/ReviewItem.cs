using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RectTransform starsContainer; // place to instantiate star/emptyStar prefabs (Horizontal Layout recommended)
    [SerializeField] private TextMeshProUGUI reviewStarText;
    
    [SerializeField] private Image likeImage;
    [SerializeField] private Sprite likeIcon;    // optional, set active if like
    [SerializeField] private Sprite dislikeIcon; // optional, set active if dislike
    
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color likeBGColor;
    [SerializeField] private Color disLikeBGColor;

    // Setup called by ReviewUI; star/empty star prefabs are passed in so the item doesn't manage global prefabs
    public void Setup(Review r, GameObject starPrefab, GameObject emptyStarPrefab)
    {
        if (headerText != null) headerText.text = r.header;
        if (descriptionText != null) descriptionText.text = r.description;

        if (starsContainer == null || starPrefab == null || emptyStarPrefab == null)
        {
            Debug.LogWarning("ReviewItemUI missing references for stars.");
            return;
        }

        // clear any previous stars
        for (int i = starsContainer.childCount - 1; i >= 0; i--) Destroy(starsContainer.GetChild(i).gameObject);

        // instantiate stars: exactly 5 slots (user said stars and empty star are separate prefabs)
        int starCount = Mathf.Clamp(r.stars, 1, 5);
        for (int i = 0; i < 5; i++)
        {
            GameObject prefabToUse = (i < starCount) ? starPrefab : emptyStarPrefab;
            Instantiate(prefabToUse, starsContainer);
        }
        reviewStarText.text = $"{starCount}/5";
        likeImage.sprite = r.liked == LikedEnum.Like ? likeIcon : dislikeIcon;
        backgroundImage.color = r.liked == LikedEnum.Like ? likeBGColor : disLikeBGColor;
    }
}