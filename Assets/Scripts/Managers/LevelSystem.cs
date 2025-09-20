using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class LevelData
{
    [ReadOnly] public int level;
    public int requiredXP;

    public LevelData(int level, int requiredXP)
    {
        this.level = level;
        this.requiredXP = requiredXP;
    }
}

public class LevelSystem : MonoBehaviour, IJsonSaveable
{
    public static LevelSystem Instance { get; private set; }

    public event Action<int> OnLevelUp; // new level
    public event Action<int, int> OnXpGained; // amount, currentXP
    public event Action ForceSetValues;

    [Header("Initial")]
    [SerializeField] private int startLevel = 1;
    [SerializeField] private int startXP = 0;

    [Header("Progression")]
    [SerializeField, Tooltip("Eğer levels listesi boşsa formülle hesaplanır.")] 
    private List<LevelData> levels = new List<LevelData>();

    [SerializeField, Tooltip("baseXP * level^exponent")]
    private bool useFormulaIfNoList = true;
    [SerializeField, ShowIf(nameof(useFormulaIfNoList))] private int baseXP = 100;
    [SerializeField, ShowIf(nameof(useFormulaIfNoList))] private float exponent = 1.2f;
    [SerializeField] private int maxLevel = 100;

    [ReadOnly] [SerializeField] private int currentLevel;
    [ReadOnly] [SerializeField] private int currentXP;

    private void Awake()
    {
        Instance = this;

        currentLevel = Mathf.Max(1, startLevel);
        currentXP = Mathf.Max(0, startXP);

        // Ensure level list has proper levels if given
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].level = i + 1;
        }
    }

    // Public read properties
    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentXP() => currentXP;
    public int GetXPToNextLevel() => Mathf.Max(0, GetRequiredXPForLevel(currentLevel) - currentXP);
    public float GetNormalizedProgressToNextLevel()
    {
        int req = GetRequiredXPForLevel(currentLevel);
        if (req <= 0) return 0f;
        return Mathf.Clamp01((float)currentXP / (float)req);
    }

    /// <summary>
    /// Gain XP (handles multi-level ups and carry-over).
    /// </summary>
    public void GainXP(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;
        OnXpGained?.Invoke(amount, currentXP);

        // while we have enough XP for next level and haven't hit max level
        while (currentLevel < maxLevel && currentXP >= GetRequiredXPForLevel(currentLevel))
        {
            int required = GetRequiredXPForLevel(currentLevel);
            currentXP -= required; // carry-over: remove required XP for this level
            currentLevel++;
            OnLevelUp?.Invoke(currentLevel);
        }

        // If at max level, clamp XP (choice: either clamp to required-1 or to 0)
        if (currentLevel >= maxLevel)
        {
            currentLevel = Mathf.Min(currentLevel, maxLevel);
            // let's clamp currentXP so it doesn't overflow ridiculously
            currentXP = Mathf.Min(currentXP, GetRequiredXPForLevel(currentLevel) - 1);
        }
    }

    /// <summary>
    /// Returns required XP to go from 'level' to next level.
    /// If user provided a levels list, use that; otherwise use formula.
    /// Levels list is 1-indexed by design (level 1 is index 0).
    /// </summary>
    private int GetRequiredXPForLevel(int level)
    {
        if (levels != null && levels.Count >= level && level > 0)
        {
            return Mathf.Max(1, levels[level - 1].requiredXP);
        }
        else if (useFormulaIfNoList)
        {
            // formula: baseXP * level^exponent
            double calc = baseXP * Math.Pow(level, exponent);
            int req = Mathf.Max(1, (int)Math.Floor(calc));
            return req;
        }
        else
        {
            // fallback
            return Mathf.Max(1, baseXP);
        }
    }

    /// <summary>
    /// Force set level (useful for debug / loading saves)
    /// </summary>
    private void SetLevel(int level, int xp = 0)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        currentXP = Mathf.Clamp(xp, 0, GetRequiredXPForLevel(currentLevel) - 1);
        ForceSetValues?.Invoke();
    }

    public JToken CaptureAsJToken()
    {
        JObject state = new JObject();
        state["level"] = currentLevel;
        state["xp"] = currentXP;
        Debug.Log("Captured level: " + currentLevel + " xp: " + currentXP + "");
        return state;
    }

    public void RestoreFromJToken(JToken state)
    {
        JObject stateObj = (JObject)state;
        if (stateObj.TryGetValue("level", out JToken levelToken))
        {
            currentLevel = levelToken.Value<int>();
        }
        if (stateObj.TryGetValue("xp", out JToken xpToken))
        {
            currentXP = xpToken.Value<int>();
        }
        SetLevel(currentLevel, currentXP);
    }
    
    [Button("Gain 10 XP")]
    public void Gain10XP()
    {
        GainXP(10);   
    }
}

