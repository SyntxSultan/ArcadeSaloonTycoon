using UnityEngine;

public class TrashSpawnValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    [SerializeField] private float validationRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayerMask = -1;
    
    public static bool IsValidSpawnPosition(Vector3 position, float radius, LayerMask layerMask)
    {
        Collider[] obstacles = Physics.OverlapSphere(position, radius, layerMask);
        
        foreach (Collider obstacle in obstacles)
        {
            // Spawn plane'i ve çöpleri göz ardı et
            if (obstacle.CompareTag("SpawnPlane") || obstacle.CompareTag("Trash"))
                continue;
                
            return false; // Engel bulundu
        }
        
        return true; // Geçerli pozisyon
    }
    
    // Debug görselleştirmesi
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, validationRadius);
    }
}