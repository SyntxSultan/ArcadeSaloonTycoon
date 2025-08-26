using UnityEngine;
using System.Collections.Generic;

public class RipplePool : MonoBehaviour
{
    [SerializeField] private GameObject ripplePrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Transform poolParent;
    
    private Queue<GameObject> availableRipples = new Queue<GameObject>();
    private HashSet<GameObject> activeRipples = new HashSet<GameObject>();
    
    private static RipplePool instance;
    public static RipplePool Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        InitializePool();
    }

    private void InitializePool()
    {
        if (poolParent == null)
            poolParent = transform;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject ripple = Instantiate(ripplePrefab, poolParent);
            ripple.SetActive(false);
            availableRipples.Enqueue(ripple);
        }
    }

    public GameObject GetRipple(Vector2 position, Transform parent = null)
    {
        GameObject ripple;
        
        if (availableRipples.Count > 0)
        {
            ripple = availableRipples.Dequeue();
        }
        else
        {
            ripple = Instantiate(ripplePrefab, poolParent);
            Debug.LogWarning("Ripple pool kapasitesi doldu, yeni ripple oluşturuluyor!");
        }

        ripple.transform.SetParent(parent ?? poolParent);
        ripple.GetComponent<RectTransform>().position = position;
        ripple.SetActive(true);
        activeRipples.Add(ripple);
        
        return ripple;
    }

    public void ReturnRipple(GameObject ripple)
    {
        if (ripple == null || !activeRipples.Contains(ripple)) return;
        
        ripple.SetActive(false);
        ripple.transform.SetParent(poolParent);
        activeRipples.Remove(ripple);
        availableRipples.Enqueue(ripple);
    }

    // Tüm aktif ripple'ları geri döndür
    public void ReturnAllRipples()
    {
        var activeList = new List<GameObject>(activeRipples);
        foreach (var ripple in activeList)
        {
            ReturnRipple(ripple);
        }
    }
    
    private void OnDestroy()
    {
        availableRipples.Clear();
        activeRipples.Clear();
        instance = null;
    }

}
