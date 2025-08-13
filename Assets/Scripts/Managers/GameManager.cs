using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour, IJsonSaveable
{
    public static GameManager Instance { get; private set; }
    
    private string saloonName = "NULL";
    
    private void OnEnable()
    {
        MobileNotificationHelper.Initialize();
        MobileNotificationHelper.CancelAllNotifications();
    }

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
        Application.targetFrameRate = 60;
        if (!SaveManager.Instance.HasSave())
        {
            ScreenManager.Instance.ShowEnterNameScreen();
        }
        else
        {
            SaveManager.Instance.LoadSave();
        }
    }

    public void SetSaloonName(string salonNewName)
    {
        saloonName = salonNewName;
    }

    public JToken CaptureAsJToken()
    {
        return JToken.FromObject(saloonName);
    }

    public void RestoreFromJToken(JToken state)
    {
        SetSaloonName(state.ToObject<string>());
    }
}
