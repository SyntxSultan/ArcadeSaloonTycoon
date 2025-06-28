using TMPro;
using UnityEngine;

public class StoreDetailsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI saloonNameTextbox;
    
    public void SetSaloonName(string name)
    {
        saloonNameTextbox.text = name;
    }
}    