using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    
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
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        CloseAllUIWithoutAnimation();

        BindButtons();
    }

    private void BindButtons()
    {
        storeOpenButton.onClick.AddListener(OpenStoreUI);
        closeStorePanelButton.onClick.AddListener(CloseStoreUI);
        closeItemDetailsButton.onClick.AddListener(CloseItemDetailsPopup);
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
        
        itemDetailsPanel.gameObject.GetComponent<ItemDetailsPopup>().SetItemDetails(itemSO);
        
        CloseStoreUI();
        
        if (itemDetailsPanelSeq != null && itemDetailsPanelSeq.IsActive())
        {
            itemDetailsPanelSeq.Kill();
        }
        itemDetailsPanel.localScale = Vector3.one;
        
        itemDetailsPanelSeq = DOTween.Sequence();
        itemDetailsPanelSeq.Append(itemDetailsPanel.DOPunchScale(
            new Vector3(0.15f, 0.15f, 0),
            0.35f,                       
            4,                          
            0.5f                       
        ));
    }

    public void CloseItemDetailsPopup()
    {
        if (itemDetailsPanelSeq != null && itemDetailsPanelSeq.IsActive())
        {
            itemDetailsPanelSeq.Kill();
        }

        itemDetailsPanelSeq = DOTween.Sequence();
        itemDetailsPanelSeq.Append(itemDetailsPanel.DOScale(Vector3.zero, 0.15f)).OnComplete(() => 
            itemDetailsPanel.gameObject.SetActive(false)
        );
    }

    private void CloseAllUIWithoutAnimation()
    {
        storeUIPanel.gameObject.SetActive(false);
        itemDetailsPanel.gameObject.SetActive(false);
        buildingUI.gameObject.SetActive(false);
    }
}