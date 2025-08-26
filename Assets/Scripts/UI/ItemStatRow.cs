using TMPro;
using UnityEngine;

public class ItemStatRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statText;
    
    public void SetStat(int stat)
    {
        statText.text = stat.ToString("F0");
    }
}
