using System.Collections.Generic;
using SaveSystem;
using SoulGames.EasyGridBuilderPro;
using UnityEngine;

[RequireComponent(typeof(JsonSavingSystem))]

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string saveFile = "save";
    private const float saveInterval = 15f;
    private float timeSinceLastSave;
    private JsonSavingSystem saveSystem;
    private List<EasyGridBuilderPro> gridList;

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
        gridList = MultiGridManager.Instance.easyGridBuilderProList;
    }

    private void Update()
    {
        timeSinceLastSave += Time.deltaTime;
        if (timeSinceLastSave > saveInterval)
        {
            Save();
        }
    }

    public void LoadSave()
    {
        saveSystem.Load(saveFile);
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.TriggerGridLoad();
        }
    }

    public void Save()
    {
        saveSystem.Save(saveFile);
        timeSinceLastSave = 0f;
        foreach(EasyGridBuilderPro easyGridBuilderPro in gridList)
        {
            easyGridBuilderPro.TriggerGridSave();
        }
    }

    public bool HasSave()
    {
        return saveSystem.SaveExists(saveFile);
    }

    public void DBG_ClearSave()
    {
        saveSystem.Delete(saveFile);
    }
}
