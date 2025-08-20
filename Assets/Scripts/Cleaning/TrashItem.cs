using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashItem : MonoBehaviour
{
    private DirtnessManager dirtinessManager;
    public bool isClickable = true;
    
    [Header("Trash Settings")]
    [SerializeField] private float destroyDelay = 0.1f;
    [SerializeField] private GameObject cleanEffect;
    
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
        
        if (AudioManager.Instance) AudioManager.Instance.PlaySFX(SFX.CollectTrash);
        
        if (cleanEffect != null)
        {
            var spawnedEffect = Instantiate(cleanEffect, transform.position, transform.rotation);
            Destroy(spawnedEffect, 2f);
        }
        
        dirtinessManager.RemoveTrash(gameObject);
        
        Destroy(gameObject, destroyDelay);
    }
}