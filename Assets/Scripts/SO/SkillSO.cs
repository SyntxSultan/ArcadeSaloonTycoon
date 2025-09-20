using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    [TextArea] public string skillDescription;
    public int skillPointCost;
    public int skillMoneyCost;
    public SkillSO[] prerequisites;
}
