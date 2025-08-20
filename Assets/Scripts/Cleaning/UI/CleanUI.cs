using UnityEngine;
using UnityEngine.UI;

public class CleanUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider cleanlinessSlider;
    [SerializeField] private DirtnessManager dirtinessManager;
    
    [Header("UI Settings")]
    [SerializeField] private Color cleanColor = Color.green;
    [SerializeField] private Color dirtyColor = Color.red;
    
    void Start()
    {
        if (dirtinessManager != null)
        {
            dirtinessManager.OnTrashCountChanged += UpdateUI;
            
            UpdateUI();
        }
        else
        {
            Debug.LogError("DirtinessManager referansı atanmadı!");
        }
        
        // Slider'ı başlat
        if (cleanlinessSlider != null)
        {
            cleanlinessSlider.minValue = 0f;
            cleanlinessSlider.maxValue = 100f;
            cleanlinessSlider.value = 100f;
        }
    }
    
    void OnDestroy()
    {
        // Event'ten çık
        if (dirtinessManager != null)
        {
            dirtinessManager.OnTrashCountChanged -= UpdateUI;
        }
    }
    
    private void UpdateUI()
    {
        float cleanlinessPercentage = dirtinessManager.GetCleanlinessPercentage();
        
        if (cleanlinessSlider != null)
        {
            cleanlinessSlider.value = cleanlinessPercentage;
            
            // Slider rengini güncelle
            Image sliderFill = cleanlinessSlider.fillRect.GetComponent<Image>();
            if (sliderFill != null)
            {
                sliderFill.color = Color.Lerp(dirtyColor, cleanColor, cleanlinessPercentage / 100f);
            }
        }
    }
}
