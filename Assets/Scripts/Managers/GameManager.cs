using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour, IJsonSaveable
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private StoreDetailsUI storeDetailsUI;
    
    private string saloonName = "NULL";

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
        if (!SaveManager.Instance.HasSave())
        {
            ScreenManager.Instance.ShowEnterNameScreen();
        }
        else
        {
            SaveManager.Instance.LoadSave();
        }
    }

    public void SetSaloonName(string name)
    {
        saloonName = name;
        storeDetailsUI.SetSaloonName(name);
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
