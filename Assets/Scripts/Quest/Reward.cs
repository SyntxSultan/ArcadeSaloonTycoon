using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Quest System/Reward")]
public class Reward : ScriptableObject
{
    public QuestRewardStruct[] rewardStructs;
}

public enum QuestRewardType
{
    Coin,
    Money
}

[System.Serializable]
public struct QuestRewardStruct
{
    public Sprite icon;
    public QuestRewardType rewardType;
    public int amount;
}