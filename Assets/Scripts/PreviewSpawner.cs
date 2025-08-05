using System;
using System.Linq;
using UnityEngine;

public class PreviewSpawner : MonoBehaviour
{
    public static PreviewSpawner Instance { get; private set; }
    
    public Action<GameObject> OnPreviewSpawned;
    
    [SerializeField] private Transform previewRoot; 
    [SerializeField] private Camera previewCamera; 
    [SerializeField] private string previewLayerName = "PreviewLayer";
    [SerializeField] private float previewScale = 1f;

    private GameObject spawnedGO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        previewCamera.gameObject.SetActive(false);
    }

    public void SpawnPreview(GameObject prefab)
    {
        // Clear existing previews
        foreach (Transform c in previewRoot) Destroy(c.gameObject);
        
        previewCamera.gameObject.SetActive(true);

        // Spawn new preview
        spawnedGO = Instantiate(prefab, previewRoot);

        // Set layer
        int layer = LayerMask.NameToLayer(previewLayerName);
        ASTLibrary.SetLayerRecursive(spawnedGO, layer);

        // Reset transform
        spawnedGO.transform.localPosition = Vector3.zero;
        spawnedGO.transform.localRotation = Quaternion.identity;
        spawnedGO.transform.localScale = Vector3.one * previewScale;

        CenterOnPivot(spawnedGO);

        if (previewCamera != null)
        {
            Vector3 camForward = previewCamera.transform.forward;
            spawnedGO.transform.rotation = Quaternion.LookRotation(-camForward, Vector3.up);
        }
        
        OnPreviewSpawned?.Invoke(spawnedGO);
    }

    public void OnPreviewClosed()
    {
        previewCamera.gameObject.SetActive(false);
        Destroy(spawnedGO);
    }

    private void CenterOnPivot(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) 
        {
            Debug.LogWarning($"No renderers found on {obj.name} - cannot center model");
            return;
        }

        // Calculate combined bounds in world space
        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        // Calculate the offset needed to center the model
        Vector3 centerOffset = combinedBounds.center - obj.transform.position;
        
        // Apply the offset to move the model so its visual center aligns with the pivot
        // We move all children, not the root transform itself
        foreach (Transform child in obj.transform)
        {
            child.position -= centerOffset;
        }
    }
}