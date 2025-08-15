using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform questContainer;
    [SerializeField] private GameObject questItemPrefab;
    
    private QuestManager questManager;
    private Dictionary<Quest, GameObject> questUIItems = new Dictionary<Quest, GameObject>();
    
    void Start()
    {
        questManager = FindAnyObjectByType<QuestManager>();
        if (questManager == null)
        {
            Debug.LogError("QuestManager not found!");
            return;
        }
        
        // Subscribe to events
        questManager.OnQuestCompleted += OnQuestCompleted;
        questManager.OnQuestActivated += OnQuestActivated;
        
        // Initialize UI with current active quests
        RefreshQuestUI();
    }
    
    void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.OnQuestCompleted -= OnQuestCompleted;
            questManager.OnQuestActivated -= OnQuestActivated;
        }
    }
    
    void OnQuestActivated(Quest quest)
    {
        CreateQuestUIItem(quest);
    }
    
    void OnQuestCompleted(Quest quest)
    {
        if (questUIItems.ContainsKey(quest))
        {
            // Add completion animation here if needed
            Destroy(questUIItems[quest]);
            questUIItems.Remove(quest);
        }
    }
    
    void CreateQuestUIItem(Quest quest)
    {
        if (questItemPrefab == null || questContainer == null) return;
        
        GameObject questItem = Instantiate(questItemPrefab, questContainer);
        QuestUIItem questUIComponent = questItem.GetComponent<QuestUIItem>();
        
        if (questUIComponent != null)
        {
            questUIComponent.SetupQuest(quest);
            questUIItems[quest] = questItem;
        }
    }
    
    void RefreshQuestUI()
    {
        // Clear existing UI items
        foreach (var kvp in questUIItems)
        {
            if (kvp.Value != null) Destroy(kvp.Value);
        }
        questUIItems.Clear();
        
        // Create UI items for active quests
        foreach (var quest in questManager.GetActiveQuests())
        {
            CreateQuestUIItem(quest);
        }
    }
}