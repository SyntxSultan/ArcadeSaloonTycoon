using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashItem : MonoBehaviour
{
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
        
        onCleanEffects?.PlayFeedbacks();
        
        dirtinessManager.RemoveTrash(gameObject);
    }
}