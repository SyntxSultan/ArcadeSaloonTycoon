using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirtnessManager : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    [SerializeField] private Transform spawnPlane;
    [SerializeField] private int maxTrashCount = 20; 
    
    [Header("Spawn Zamanlaması")]
    [SerializeField] private float spawnInterval = 3f; // Kaç saniyede bir spawn
    [SerializeField] private bool autoSpawn = true; // Otomatik spawn açık/kapalı
    
    [Header("Collision Detection")]
    [SerializeField] private LayerMask obstacleLayerMask = -1; // Çakışma kontrolü için layer mask
    [SerializeField] private float collisionCheckRadius = 0.5f; // Çakışma kontrolü radius'u
    
    [Header("Raycast Optimization")]
    [SerializeField] private LayerMask trashLayerMask = 9;
    
    [SerializeField] private int xpPerClean = 5;
    
    private float nextSpawnTime;
    
    public System.Action OnTrashCountChanged;
    
    private Camera cam;
    
    void Start()
    {
        if (spawnPlane == null)
        {
            Debug.LogError("Spawn Plane atanmadı! Lütfen bir plane objesi atayın.");
            return;
        }
        cam = Camera.main;
        
        nextSpawnTime = Time.time + spawnInterval;
        
        if (TrashPool.Instance == null)
        {
            Debug.LogError("TrashPool bulunamadı! Lütfen TrashPool prefab'ını sahneye ekleyin.");
        }
        
        OnTrashCountChanged?.Invoke();
    }
    
    void Update()
    {
        HandleInput();
        
        if (autoSpawn && Time.time >= nextSpawnTime && CanSpawnTrash())
        {
            SpawnTrash();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void HandleInput()
    {
        if (Pointer.current == null) return;
        if (!Pointer.current.press.wasPressedThisFrame) return;
        if (ASTLibrary.IsPointerOverUI()) return;

        Ray ray = cam.ScreenPointToRay(Pointer.current.position.ReadValue());
    
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, trashLayerMask))
        {
            if (hit.collider.TryGetComponent(out TrashItem trash))
            {
                if (trash.isClickable)
                {
                    trash.CleanTrash();
                    LevelSystem.Instance.GainXP(xpPerClean);
                }
            }
        }
    }

    private bool CanSpawnTrash()
    {
        return TrashPool.Instance != null && TrashPool.Instance.GetActiveTrashCount() < maxTrashCount;
    }
    
    private void SpawnTrash()
    {
        if (!CanSpawnTrash())
            return;
            
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        if (IsPositionBlocked(spawnPosition)) return;
        
        GameObject newTrash = TrashPool.Instance.GetRandomTrash(spawnPosition, Random.rotation);
        
        if (newTrash == null) return;
        
        TrashItem trashItem = newTrash.GetComponent<TrashItem>();
        if (trashItem == null)
        {
            trashItem = newTrash.AddComponent<TrashItem>();
        }
        trashItem.Initialize(this);
        
        OnTrashCountChanged?.Invoke();
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        Bounds planeBounds = spawnPlane.GetComponent<Renderer>().bounds;
        
        float randomX = Random.Range(planeBounds.min.x, planeBounds.max.x);
        float randomZ = Random.Range(planeBounds.min.z, planeBounds.max.z);
        
        float yPosition = planeBounds.max.y + 0.1f;
        
        return new Vector3(randomX, yPosition, randomZ);
    }
    
    private bool IsPositionBlocked(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, collisionCheckRadius, obstacleLayerMask);
        
        foreach (Collider col in colliders)
        {
            if (col.transform == spawnPlane || col.CompareTag("Trash"))
                continue;
                
            return true;
        }
        
        return false;
    }
    
    public void RemoveTrash(GameObject trash)
    {
        TrashPool.Instance?.ReturnTrash(trash);
        OnTrashCountChanged?.Invoke();
    }
    
    public void CleanAllTrash()
    {
        TrashPool.Instance?.ReturnAllTrash();
        OnTrashCountChanged?.Invoke();
    }
    
    public float GetCleanlinessPercentage()
    {
        int activeTrashCount = TrashPool.Instance?.GetActiveTrashCount() ?? 0;
        return (1f - (float)activeTrashCount / maxTrashCount) * 100f;
    }
}
