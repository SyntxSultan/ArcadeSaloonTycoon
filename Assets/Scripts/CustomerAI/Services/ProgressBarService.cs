using UnityEngine;
using UnityEngine.UI;

public class ProgressBarService : MonoBehaviour, IProgressBarService
{
    [Header("Progress Bar Settings")]
    [SerializeField] private GameObject progressBarPrefab;
    [SerializeField] private Vector3 offsetFromCustomer = new Vector3(0, 3f, 0);
    
    private GameObject progressBarInstance;
    private Canvas progressCanvas;
    private Slider progressSlider;
    
    public bool IsVisible => progressBarInstance != null && progressBarInstance.activeInHierarchy;
    
    private void Awake()
    {
        CreateProgressBarIfNeeded();
    }
    
    private void CreateProgressBarIfNeeded()
    {
        if (progressBarPrefab != null && progressBarInstance == null)
        {
            progressBarInstance = Instantiate(progressBarPrefab, transform);
            progressBarInstance.transform.localPosition = offsetFromCustomer;
            
            // Get components
            progressCanvas = progressBarInstance.GetComponent<Canvas>();
            progressSlider = progressBarInstance.GetComponentInChildren<Slider>();
            
            // Configure canvas
            if (progressCanvas != null)
            {
                progressCanvas.worldCamera = Camera.main;
            }
            
            HideProgressBar();
        }
    }
    
    public void ShowProgressBar()
    {
        CreateProgressBarIfNeeded();
        
        if (progressBarInstance != null)
        {
            progressBarInstance.SetActive(true);
            
            if (progressSlider != null)
                progressSlider.value = 0f;
        }
    }
    
    public void UpdateProgress(float progress)
    {
        if (progressSlider != null && IsVisible)
        {
            progressSlider.value = Mathf.Clamp01(progress);
        }
    }
    
    public void HideProgressBar()
    {
        if (progressBarInstance != null)
        {
            progressBarInstance.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (progressBarInstance != null)
        {
            Destroy(progressBarInstance);
        }
    }
}