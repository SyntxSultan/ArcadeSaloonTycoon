using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button rotateButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        rotateButton.onClick.AddListener(OnRotate);
    }

    private void OnConfirm()
    {
        GridBridge.Instance.ConfirmBuilding();
    }
    private void OnCancel()
    {
        GridBridge.Instance.CancelBuilding();
    }
    private void OnRotate()
    {
        GridBridge.Instance.RotateBuilding();
    }
}
