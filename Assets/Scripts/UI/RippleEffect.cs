using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RippleEffect : MonoBehaviour
{
    [SerializeField] private Image image2;
    public float duration = 0.8f;

    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Image img = GetComponent<Image>();

        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, duration).SetEase(Ease.OutSine);
        img.DOFade(0, duration).OnComplete(() => Destroy(gameObject));
        if (image2 == null) Debug.LogWarning("No image 2 found");
        image2.rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutSine);
        image2.DOFade(0, duration);
    }
}
