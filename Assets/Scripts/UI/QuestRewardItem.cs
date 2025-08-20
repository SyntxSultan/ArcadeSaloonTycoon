using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestRewardItem : MonoBehaviour
{
    public Image rewardIcon;
    public TextMeshProUGUI amountText;
    
    public void SetupReward(QuestRewardStruct reward)
    {
        if (rewardIcon != null) rewardIcon.sprite = reward.icon;
        if (amountText != null) amountText.text = reward.amount.ToString();
    }
}