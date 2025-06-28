using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    
    [SerializeField] private EnterNameScreen enterNameScreen;
    
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
    
    public void ShowEnterNameScreen()
    {
        enterNameScreen.gameObject.SetActive(true);
    }
    public void HideEnterNameScreen()
    {
        enterNameScreen.gameObject.SetActive(false);
    }
}