using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterNameScreen : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button confirmButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(SubmitName);
    }

    private void SubmitName()
    {
        GameManager.Instance.SetSaloonName(nameInput.text);
        ScreenManager.Instance.HideEnterNameScreen();
    }
}