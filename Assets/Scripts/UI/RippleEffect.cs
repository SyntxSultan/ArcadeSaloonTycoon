using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RippleEffect : MonoBehaviour
{
    [SerializeField] private Image image2;
    public float duration = 0.8f;

    private RectTransform rt;
    private Image img;
    private Tween scaleTween;
    private Tween fadeTween;
    private Tween image2ScaleTween;
    private Tween image2FadeTween;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        
        if (image2 == null) 
            Debug.LogWarning("No image 2 found");
    }

    void OnEnable()
    {
        StartRippleAnimation();
    }

    void OnDisable()
    {
        KillAllTweens();
    }

    private void StartRippleAnimation()
    {
        KillAllTweens();
        
        rt.localScale = Vector3.zero;
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
        
        if (image2 != null)
        {
            image2.rectTransform.localScale = Vector3.zero;
            image2.color = new Color(image2.color.r, image2.color.g, image2.color.b, 1f);
        }

        scaleTween = rt.DOScale(Vector3.one, duration).SetEase(Ease.OutSine);
        fadeTween = img.DOFade(0, duration).OnComplete(() => {
            RipplePool.Instance?.ReturnRipple(gameObject);
        });

        if (image2 != null)
        {
            image2ScaleTween = image2.rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutSine);
            image2FadeTween = image2.DOFade(0, duration);
        }
    }

    private void KillAllTweens()
    {
        scaleTween?.Kill();
        fadeTween?.Kill();
        image2ScaleTween?.Kill();
        image2FadeTween?.Kill();
    }

    void OnDestroy()
    {
        KillAllTweens();
    }
}
