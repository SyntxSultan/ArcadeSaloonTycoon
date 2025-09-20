using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour, IJsonSaveable
{
    public static SkillManager Instance { get; private set; }
    
    private HashSet<SkillSO> unlockedSkills = new HashSet<SkillSO>();
    private int availableSkillPoints;
    
    public int AvailableSkillPoints => availableSkillPoints;
    public IReadOnlyCollection<SkillSO> UnlockedSkills => unlockedSkills;
    
    public Action<int> OnSkillPointsChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LevelSystem.Instance.OnLevelUp += LevelSystem_OnLevelUp;
        //LoadSkillData();
    }

    private void OnDestroy()
    {
        if (LevelSystem.Instance != null)
            LevelSystem.Instance.OnLevelUp -= LevelSystem_OnLevelUp;
    }

    private void LevelSystem_OnLevelUp(int obj)
    {
        availableSkillPoints++;
        OnSkillPointsChanged?.Invoke(availableSkillPoints);
    }
        
    public bool IsSkillUnlocked(SkillSO skill) => unlockedSkills.Contains(skill);

    public bool CanUnlockSkill(SkillSO skill)
    {
        // Already unlocked
        if (IsSkillUnlocked(skill)) return false;
        
        // Not enough points
        if (availableSkillPoints < skill.skillPointCost) return false;
        if (CurrencyManager.Instance?.GetMoney() < skill.skillMoneyCost) return false;

        // Check prerequisites
        if (skill.prerequisites != null)
        {
            foreach (var prerequisite in skill.prerequisites)
            {
                if (!IsSkillUnlocked(prerequisite))
                    return false;
            }
        }
        
        return true;
    }
    
    public bool TryUnlockSkill(SkillSO skill)
    {
        if (!CanUnlockSkill(skill)) return false;

        // Unlock the skill
        unlockedSkills.Add(skill);
        
        availableSkillPoints -= skill.skillPointCost;
        OnSkillPointsChanged?.Invoke(availableSkillPoints);

        CurrencyManager.Instance.RemoveMoney(skill.skillMoneyCost);
        
        ApplySkillEffect(skill);
        
        //SaveSkillData();
        
        Debug.Log($"Unlocked skill: {skill.skillName}");
        return true;
    }

    private void ApplySkillEffect(SkillSO skill)
    {
        // Apply the skill's effect to the player
        // This could be done through interfaces, events, or direct references
        // Example: PlayerStats.Instance.ApplySkillBonus(skill);
    }
    
    public JToken CaptureAsJToken()
    {
        JObject state = new JObject();
        state["availableSkillPoints"] = availableSkillPoints;

        JArray unlockedArray = new JArray();
        foreach (var skill in unlockedSkills)
        {
            unlockedArray.Add(skill.name);
        }
        state["unlockedSkills"] = unlockedArray;

        return state;
    }

    public void RestoreFromJToken(JToken state)
    {
        JObject stateObj = (JObject)state;

        if (stateObj.TryGetValue("availableSkillPoints", out JToken pointsToken))
        {
            availableSkillPoints = pointsToken.Value<int>();
        }

        unlockedSkills.Clear(); // Clear existing to avoid duplicates on load
        if (stateObj.TryGetValue("unlockedSkills", out JToken skillsToken) && skillsToken is JArray skillsArray)
        {
            foreach (var skillIdToken in skillsArray)
            {
                string skillId = skillIdToken.Value<string>();
                if (!string.IsNullOrEmpty(skillId))
                {
                    SkillSO skill = Resources.Load<SkillSO>($"Skills/{skillId}");
                    if (skill != null)
                    {
                        unlockedSkills.Add(skill);
                    }
                }
            }
        }

        OnSkillPointsChanged?.Invoke(availableSkillPoints);
    }

    // Debug methods
    [Button("Reset All Skills")]
    public void ResetAllSkills()
    {
        unlockedSkills.Clear();
        availableSkillPoints = 0;
        OnSkillPointsChanged?.Invoke(availableSkillPoints);
    }

    [Button("Add 5 Skill Points")]
    public void Debug_AddSkillPoints()
    {
        AddSkillPoints(5);
    }

    private void AddSkillPoints(int i)
    {
        availableSkillPoints += i;
        OnSkillPointsChanged?.Invoke(availableSkillPoints);
    }
}
