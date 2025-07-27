using SaveSystem;
using UnityEngine;

[RequireComponent(typeof(JsonSavingSystem))]

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string saveFile = "save";
    private const float saveInterval = 5f;
    private float timeSinceLastSave;
    private JsonSavingSystem saveSystem;

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
        saveSystem = GetComponent<JsonSavingSystem>();
    }

    void Update()
    {
        timeSinceLastSave += Time.deltaTime;
        if (timeSinceLastSave > saveInterval)
        {
            saveSystem.Save(saveFile);
            timeSinceLastSave = 0f;
            //Debug.Log("Saved");
        }
    }

    public void LoadSave()
    {
        saveSystem.Load(saveFile);
    }

    public bool HasSave()
    {
        return saveSystem.SaveExists(saveFile);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DBG_ClearSave()
    {
        saveSystem.Delete(saveFile);
    }
}
