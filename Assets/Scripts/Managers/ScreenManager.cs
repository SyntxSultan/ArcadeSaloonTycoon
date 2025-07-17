using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    
    [FormerlySerializedAs("enterNameScreen")] 
    [SerializeField] private EnterNameScreen enterSaloonNameUI;
    
    [Header("Store UI")]
    [SerializeField] private StoreUI storeUIPanel;
    [SerializeField] private Button storeOpenButton;
    [SerializeField] private Button closeStorePanelButton;
    [SerializeField] private Animator storeAnimator;
    
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
    
    void Start()
    {
        storeOpenButton.onClick.AddListener(OpenStoreUI);
        closeStorePanelButton.onClick.AddListener(CloseStoreUI);
    }

    public void OpenStoreUI()
    {
        storeUIPanel.gameObject.SetActive(true);
        storeAnimator.SetTrigger("OpenStore");
    }

    public void CloseStoreUI()
    {
        storeUIPanel.gameObject.SetActive(false);
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