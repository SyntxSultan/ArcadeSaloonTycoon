using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [SerializeField] private Button uiOverlay;
    
    [Header("Enter Name UI")]
    [SerializeField] private EnterNameScreen enterSaloonNameUI;
    
    [Header("Store UI")]
    [SerializeField] private RectTransform storeUIPanel;
    [SerializeField] private Button storeOpenButton;
    [SerializeField] private Button closeStorePanelButton;
    
    [Header("Item Details UI")]
    [SerializeField] private RectTransform itemDetailsPanel;
    [SerializeField] private Button closeItemDetailsButton;
    private Sequence itemDetailsPanelSeq;
    
    [Header("Build Confirm Popup")]
    [SerializeField] private RectTransform buildingUI;
    
    [Header("Settings Popup")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private RectTransform settingsPopup;
    private Sequence settingsPopupSeq;
    
    [Header("Daily Rewards UI")]
    [SerializeField] private Button dailyRewardsButton;
    [SerializeField] private RectTransform dailyRewardsUI;
    private Sequence dailyRewardsSeq;
    
    [Header("Coin Price UI")]
    [SerializeField] private Button coinPriceButton;   
    [SerializeField] private RectTransform coinPriceUI;
    private Sequence coinPriceSeq;
    
    [Header("Quest")]
    [SerializeField] private Button questButton;
    [SerializeField] private RectTransform questPanel;
    private Sequence questSeq;
    
    [Header("Upgrade UI")]
    [SerializeField] private RectTransform upgradePanel;
    [SerializeField] private Button closeUpgradePanelButton;
    
    [Header("Reviews UI")]
    [SerializeField] private RectTransform reviewsPanel;
    [SerializeField] private Button closeReviewsPanelButton;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CloseAllUIWithoutAnimation();
    }
    
    private void Start()
    {
        BindButtons();
    }

    private void BindButtons()
    {
        storeOpenButton.onClick.AddListener(OpenStoreUI);
        closeStorePanelButton.onClick.AddListener(CloseStoreUI);
        settingsButton.onClick.AddListener(OpenSettingsPopup);
        dailyRewardsButton.onClick.AddListener(OpenDailyRewardsUI);
        coinPriceButton.onClick.AddListener(OpenCoinPriceUI);
        questButton.onClick.AddListener(OpenQuestUI);
        closeUpgradePanelButton.onClick.AddListener(CloseUpgradePanel);
        
        uiOverlay.onClick.AddListener(CloseStoreUI);
        uiOverlay.onClick.AddListener(CloseItemDetailsPopup);
        uiOverlay.onClick.AddListener(CloseCoinPriceUI);
        uiOverlay.onClick.AddListener(CloseDailyRewardsUI);
        uiOverlay.onClick.AddListener(CloseSettingsPopup);
        uiOverlay.onClick.AddListener(CloseQuestUI);
    }

    private void OpenQuestUI()
    {
        questPanel.gameObject.SetActive(true);
        uiOverlay.gameObject.SetActive(true);
    }
    
    private void CloseQuestUI()
    {
        questPanel.gameObject.SetActive(false);
        uiOverlay.gameObject.SetActive(false);
    }

    private void OpenCoinPriceUI()
    {
        coinPriceUI.gameObject.SetActive(true);
        uiOverlay.gameObject.SetActive(true);
        CloseSettingsPopup();
        
        IfSequenceActiveKillAndReset(ref coinPriceSeq);
        
        coinPriceSeq.Append(coinPriceUI.DOJumpAnchorPos(
            new Vector2(0, 100),
            10f,
            1,
            0.5f
            ));
    }
    private void CloseCoinPriceUI()
    {
        coinPriceUI.gameObject.SetActive(false);
        uiOverlay.gameObject.SetActive(false);
    }
    public void OpenDailyRewardsUI()
    {
        dailyRewardsUI.gameObject.SetActive(true);
        uiOverlay.gameObject.SetActive(true);
        
        IfSequenceActiveKillAndReset(ref dailyRewardsSeq);
        
        dailyRewardsUI.localScale = Vector3.one;

        dailyRewardsSeq.Append(dailyRewardsUI.DOPunchScale(
            new Vector3(0.15f, 0.15f, 0),
            0.35f,
            4,
            0.5f));
    }

    private void CloseDailyRewardsUI()
    {
        IfSequenceActiveKillAndReset(ref dailyRewardsSeq);
        
        dailyRewardsSeq.Append(dailyRewardsUI.DOScale(Vector3.zero, 0.15f)).OnComplete(() =>
        {
            dailyRewardsUI.gameObject.SetActive(false);
            uiOverlay.gameObject.SetActive(false);
        });
    }

    private void OpenSettingsPopup()
    {
        settingsPopup.gameObject.SetActive(true);
        uiOverlay.gameObject.SetActive(true);
        
        settingsPopup.localScale = Vector3.one;
        
        IfSequenceActiveKillAndReset(ref settingsPopupSeq);
        
        settingsPopupSeq.Append(settingsPopup.DOPunchScale(
            new Vector3(0.15f, 0.15f, 0),
            0.35f,
            4,
            0.5f));
    }
    
    private void CloseSettingsPopup()
    {
        IfSequenceActiveKillAndReset(ref settingsPopupSeq);
            
        settingsPopupSeq.Append(settingsPopup.DOScale(Vector3.zero, 0.15f)).OnComplete(() => 
            settingsPopup.gameObject.SetActive(false)
        );
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

    public void ShowBuildingUI()
    {
        buildingUI.gameObject.SetActive(true);
        DOTween.Sequence().Append(
            buildingUI.DOAnchorPosX(-100, 0.5f).SetEase(Ease.OutBack));
    }

    public void HideBuildingUI()
    {
        DOTween.Sequence().Append(
            buildingUI.DOAnchorPosX(100, 0.15f).SetEase(Ease.InBack).OnComplete(()=>
                buildingUI.gameObject.SetActive(false)));
        
    }

    public void OpenItemDetailsPopup(ItemSO itemSO)
    {
        itemDetailsPanel.gameObject.SetActive(true);
        uiOverlay.gameObject.SetActive(true);
        
        itemDetailsPanel.gameObject.GetComponent<ItemDetailsPopup>().SetItemDetails(itemSO);
        
        CloseStoreUI();
        
        itemDetailsPanel.localScale = Vector3.one;
        
        IfSequenceActiveKillAndReset(ref itemDetailsPanelSeq);
        
        itemDetailsPanelSeq.Append(itemDetailsPanel.DOPunchScale(
            new Vector3(0.15f, 0.15f, 0),
            0.35f,                       
            4,                          
            0.5f                       
        ));
    }

    public void CloseItemDetailsPopup()
    {
        IfSequenceActiveKillAndReset(ref itemDetailsPanelSeq);
        
        itemDetailsPanelSeq.Append(itemDetailsPanel.DOScale(Vector3.zero, 0.15f)).OnComplete(() =>
        {
            itemDetailsPanel.gameObject.SetActive(false);
            uiOverlay.gameObject.SetActive(false);
        });
    }
    
    public void OpenUpgradePanel()
    {
        upgradePanel.gameObject.SetActive(true);
        
        Vector2 startPos = upgradePanel.anchoredPosition;
        upgradePanel.anchoredPosition = new Vector2(startPos.x, -316f);

        Sequence seq = DOTween.Sequence();
        seq.Append(
            upgradePanel.DOAnchorPosY(303f, 0.5f).SetEase(Ease.OutBack)
        );
    }
    
    public void CloseUpgradePanel()
    {
        DOTween.Sequence().Append(
            upgradePanel.DOAnchorPosY(-316f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                upgradePanel.gameObject.SetActive(false);
                UpgradeManager.Instance.Reset();
            })
        );
    }

    private void CloseAllUIWithoutAnimation()
    {
        storeUIPanel.gameObject.SetActive(false);
        itemDetailsPanel.gameObject.SetActive(false);
        buildingUI.gameObject.SetActive(false);
        settingsPopup.gameObject.SetActive(false);
        dailyRewardsUI.gameObject.SetActive(false);
        coinPriceUI.gameObject.SetActive(false);
        questPanel.gameObject.SetActive(false);
        uiOverlay.gameObject.SetActive(false);
        upgradePanel.gameObject.SetActive(false);
    }
    
    private void IfSequenceActiveKillAndReset(ref Sequence seq)
    {
        if (seq != null && seq.IsActive())
        {
            seq.Kill();
        }

        seq = DOTween.Sequence();
    }
}