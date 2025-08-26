using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashItem : MonoBehaviour
{
    [HideInInspector] public GameObject OriginalPrefab;
    private DirtnessManager dirtinessManager;
    public bool isClickable = true;
    
    [SerializeField] private MMF_Player onCleanEffects;
    
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col.isTrigger)
        {
            col.isTrigger = false;
        }
        gameObject.tag = "Trash";
    }
    
    public void Initialize(DirtnessManager manager)
    {
        dirtinessManager = manager;
    }
    
    public void CleanTrash()
    {
        if (!isClickable) return;
        
        isClickable = false;
        
        if (onCleanEffects != null)
        {
            onCleanEffects.enabled = true;
            onCleanEffects.PlayFeedbacks();
            
            StartCoroutine(WaitForFeedbackAndReturn());
        }
        else
        {
            // Feedback yoksa direkt pool'a döndür
            ReturnToPoolImmediate();
        }
    }
    
    private System.Collections.IEnumerator WaitForFeedbackAndReturn()
    {
        yield return new WaitForSeconds(onCleanEffects.TotalDuration);
        
        ReturnToPoolImmediate();
    }
    
    private void ReturnToPoolImmediate()
    {
        dirtinessManager?.RemoveTrash(gameObject);
        ReturnToPool();
    }
    
    public void ResetItem()
    {
        isClickable = true;
        
        onCleanEffects.StopFeedbacks();
        onCleanEffects.enabled = false;
        
        StopAllCoroutines();
    }

    private void ReturnToPool()
    {
        TrashPool.Instance?.ReturnTrash(gameObject);
    }
}