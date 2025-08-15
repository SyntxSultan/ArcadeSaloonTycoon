using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Quest System/Reward")]
public class Reward : ScriptableObject
{
    public Sprite icon;
    public int amount;
    public QuestRewardType rewardType;
}

public enum QuestRewardType
{
    Coin,
    Money
}