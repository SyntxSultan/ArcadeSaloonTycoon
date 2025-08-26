using System.Collections.Generic;
using UnityEngine;

public class TrashPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject[] trashPrefabs;
    [SerializeField] private int poolSizePerPrefab = 20;
    [SerializeField] private Transform poolParent;

    private Dictionary<GameObject, Queue<GameObject>> availableTrashPools = new Dictionary<GameObject, Queue<GameObject>>();
    private HashSet<GameObject> activeTrashItems = new HashSet<GameObject>();

    public static TrashPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePools();
    }

    private void InitializePools()
    {
        if (poolParent == null)
            poolParent = transform;

        foreach (GameObject prefab in trashPrefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                GameObject trashObj = Instantiate(prefab, poolParent);
                trashObj.SetActive(false);

                // Prefab referansını TrashItem’e kaydet
                if (!trashObj.TryGetComponent(out TrashItem trashItem))
                    trashItem = trashObj.AddComponent<TrashItem>();

                trashItem.OriginalPrefab = prefab;

                pool.Enqueue(trashObj);
            }

            availableTrashPools[prefab] = pool;
        }
    }

    private GameObject GetTrash(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!availableTrashPools.ContainsKey(prefab))
        {
            Debug.LogError($"Prefab {prefab.name} pool'da yok!");
            return null;
        }

        Queue<GameObject> pool = availableTrashPools[prefab];

        if (pool.Count == 0)
        {
            Debug.LogWarning($"Trash pool ({prefab.name}) dolu, yeni trash oluşturulmayacak.");
            return null; // sadece sahne başında üretim yapılacak
        }

        GameObject trash = pool.Dequeue();
        trash.transform.SetPositionAndRotation(position, rotation);
        trash.SetActive(true);

        activeTrashItems.Add(trash);

        return trash;
    }

    public GameObject GetRandomTrash(Vector3 position, Quaternion rotation)
    {
        if (trashPrefabs == null || trashPrefabs.Length == 0)
        {
            Debug.LogError("TrashPool'da prefab yok!");
            return null;
        }

        GameObject randomPrefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];
        return GetTrash(randomPrefab, position, rotation);
    }

    public void ReturnTrash(GameObject trash)
    {
        if (trash == null) return;
        if (!activeTrashItems.Contains(trash)) return;

        TrashItem trashItem = trash.GetComponent<TrashItem>();
        if (trashItem == null || trashItem.OriginalPrefab == null)
        {
            Debug.LogWarning("TrashItem veya OriginalPrefab eksik, obje yok ediliyor.");
            Destroy(trash);
            return;
        }

        trash.SetActive(false);
        trash.transform.SetParent(poolParent);

        activeTrashItems.Remove(trash);
        availableTrashPools[trashItem.OriginalPrefab].Enqueue(trash);

        trashItem.ResetItem();
    }

    public void ReturnAllTrash()
    {
        var activeList = new List<GameObject>(activeTrashItems);
        foreach (var trash in activeList)
            ReturnTrash(trash);
    }

    public int GetActiveTrashCount()
    {
        return activeTrashItems.Count;
    }
}