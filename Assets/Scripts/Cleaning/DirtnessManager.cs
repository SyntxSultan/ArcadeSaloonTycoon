using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirtnessManager : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    [SerializeField] private Transform spawnPlane; // Çöplerin spawn olacağı plane
    [SerializeField] private GameObject[] trashPrefabs; // Çöp prefab'ları
    [SerializeField] private int maxTrashCount = 20; // Maksimum çöp sayısı
    [SerializeField] private float spawnRadius = 5f; // Spawn radius'u
    
    [Header("Spawn Zamanlaması")]
    [SerializeField] private float spawnInterval = 3f; // Kaç saniyede bir spawn
    [SerializeField] private bool autoSpawn = true; // Otomatik spawn açık/kapalı
    
    [Header("Collision Detection")]
    [SerializeField] private LayerMask obstacleLayerMask = -1; // Çakışma kontrolü için layer mask
    [SerializeField] private float collisionCheckRadius = 0.5f; // Çakışma kontrolü radius'u
    
    private List<GameObject> activeTrashList = new List<GameObject>(); // Aktif çöp listesi
    private float nextSpawnTime;
    
    // Events
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
        
        OnTrashCountChanged?.Invoke();
    }
    
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 200))
        {
            TrashItem trash = hit.collider.GetComponent<TrashItem>();
            if (trash != null && trash.isClickable)
            {
                trash.CleanTrash();
            }
        }
        
        if (autoSpawn && Time.time >= nextSpawnTime && CanSpawnTrash())
        {
            SpawnTrash();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    private bool CanSpawnTrash()
    {
        return activeTrashList.Count < maxTrashCount;
    }
    
    private void SpawnTrash()
    {
        if (!CanSpawnTrash() || trashPrefabs == null || trashPrefabs.Length == 0)
            return;
            
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        if (IsPositionBlocked(spawnPosition)) return;
        
        GameObject trashPrefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];
        GameObject newTrash = Instantiate(trashPrefab, spawnPosition, Random.rotation);
        
        TrashItem trashItem = newTrash.GetComponent<TrashItem>();
        if (trashItem == null)
        {
            trashItem = newTrash.AddComponent<TrashItem>();
        }
        trashItem.Initialize(this);
        
        activeTrashList.Add(newTrash);
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
            // Spawn plane'i ve mevcut çöpleri göz ardı et
            if (col.transform == spawnPlane || col.CompareTag("Trash"))
                continue;
                
            return true; // Engel var
        }
        
        return false; // Engel yok
    }
    
    public void RemoveTrash(GameObject trash)
    {
        if (activeTrashList.Contains(trash))
        {
            activeTrashList.Remove(trash);
            OnTrashCountChanged?.Invoke();
        }
    }
    
    public void CleanAllTrash()
    {
        for (int i = activeTrashList.Count - 1; i >= 0; i--)
        {
            if (activeTrashList[i] != null)
            {
                Destroy(activeTrashList[i]);
            }
        }
        activeTrashList.Clear();
        OnTrashCountChanged?.Invoke();
    }
    
    public float GetCleanlinessPercentage()
    {
        return (1f - (float)activeTrashList.Count / maxTrashCount) * 100f;
    }
}
