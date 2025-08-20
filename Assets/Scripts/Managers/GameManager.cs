using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour, IJsonSaveable
{
    public static GameManager Instance { get; private set; }
    
    private string saloonName = "NULL";
    private float startTime;
    private int lastLoggedMinute = 0;
    
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
        startTime = Time.realtimeSinceStartup;

    }
    
    private void Update()
    {
        // Her frame'de oyun süresini kontrol et
        CheckPlaytime();
    }

    private void CheckPlaytime()
    {
        float currentPlaytime = GetCurrentPlaytime();
        int currentMinute = Mathf.FloorToInt(currentPlaytime / 60f);
        
        if (currentMinute > lastLoggedMinute && currentMinute >= 1)
        {
            Debug.Log($"Oyun Süresi: {currentMinute} dakika ({currentPlaytime:F1} saniye)");
            QuestManager.Instance.UpdateQuestProgress(QuestType.PlayForTime, 1);
            lastLoggedMinute = currentMinute;
        }
    }

    public float GetCurrentPlaytime()
    {
        return Time.realtimeSinceStartup - startTime;
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
