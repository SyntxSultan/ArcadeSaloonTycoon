using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private SFX sound = SFX.UIClick;

    private void Start()
    {
        if (AudioManager.Instance is not null)
        {
            GetComponent<Button>().onClick.AddListener(() => AudioManager.Instance.PlaySFX(sound));
        }
        else
        {
            Debug.LogError("AudioManager is not initialized buttons cant subscribe to click events");
        }
    }
}
