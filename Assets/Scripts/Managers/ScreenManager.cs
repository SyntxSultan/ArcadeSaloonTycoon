using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    
    [SerializeField] private EnterNameScreen enterSaloonNameUI;
    
    [Header("Store UI")]
    [SerializeField] private RectTransform storeUIPanel;
    [SerializeField] private Button storeOpenButton;
    [SerializeField] private Button closeStorePanelButton;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        storeOpenButton.onClick.AddListener(OpenStoreUI);
        closeStorePanelButton.onClick.AddListener(CloseStoreUI);
    }

    private void OpenStoreUI()
    {
        storeUIPanel.gameObject.SetActive(true);

        Vector2 startPos = storeUIPanel.anchoredPosition;
        storeUIPanel.anchoredPosition = new Vector2(startPos.x, -316f);

        Sequence seq = DOTween.Sequence();
        seq.Append(
            storeUIPanel.DOAnchorPosY(303f, 0.5f).SetEase(Ease.OutBack)
        );
    }

    private void CloseStoreUI()
    {
        DOTween.Sequence().Append(
            storeUIPanel.DOAnchorPosY(-316f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                storeUIPanel.gameObject.SetActive(false);
                
            })
        );
    }
    
    public void ShowEnterNameScreen()
    {
        enterSaloonNameUI.gameObject.SetActive(true);
    }
    public void HideEnterNameScreen()
    {
        enterSaloonNameUI.gameObject.SetActive(false);
    }
}