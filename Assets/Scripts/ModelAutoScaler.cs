using UnityEngine;
using UnityEditor;

public class ModelAutoScaler : MonoBehaviour
{
    [Header("Target Scale Settings")]
    [Tooltip("Target size for all models (width, height, depth)")]
    public Vector3 targetSize = new Vector3(2f, 2f, 2f);
    
    [Tooltip("Use uniform scaling (maintains proportions)")]
    public bool uniformScaling = true;
    
    [Tooltip("Auto-scale on Start")]
    public bool scaleOnStart = true;
    
    [Tooltip("Include inactive child objects in bounds calculation")]
    public bool includeInactive = false;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    public bool showDebugInfo = false;
    
    private Vector3 originalScale;
    private Bounds originalBounds;
    
    void Start()
    {
        if (scaleOnStart)
        {
            ScaleToTarget();
        }
    }
    
    /// <summary>
    /// Scale the model to fit the target size
    /// </summary>
    public void ScaleToTarget()
    {
        // Store original scale
        originalScale = transform.localScale;
        
        // Calculate bounds of the entire model
        Bounds bounds = CalculateModelBounds();
        originalBounds = bounds;
        
        if (bounds.size == Vector3.zero)
        {
            Debug.LogWarning($"No renderers found on {gameObject.name}. Cannot scale model.");
            return;
        }
        
        // Calculate scale factor
        Vector3 scaleFactor = CalculateScaleFactor(bounds.size);
        
        // Apply scaling
        transform.localScale = Vector3.Scale(originalScale, scaleFactor);
        
        if (showDebugInfo)
        {
            Debug.Log($"Scaled {gameObject.name}:");
            Debug.Log($"  Original bounds: {bounds.size}");
            Debug.Log($"  Target size: {targetSize}");
            Debug.Log($"  Scale factor: {scaleFactor}");
            Debug.Log($"  New scale: {transform.localScale}");
        }
    }
    
    /// <summary>
    /// Reset model to original scale
    /// </summary>
    public void ResetScale()
    {
        transform.localScale = originalScale;
    }
    
    /// <summary>
    /// Calculate the bounds of the entire model including all child renderers
    /// </summary>
    private Bounds CalculateModelBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(includeInactive);
        
        if (renderers.Length == 0)
            return new Bounds();
        
        // Start with the first renderer's bounds
        Bounds bounds = renderers[0].bounds;
        
        // Encapsulate all other renderer bounds
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        
        return bounds;
    }
    
    /// <summary>
    /// Calculate the scale factor needed to fit target size
    /// </summary>
    private Vector3 CalculateScaleFactor(Vector3 currentSize)
    {
        Vector3 factor = new Vector3(
            targetSize.x / currentSize.x,
            targetSize.y / currentSize.y,
            targetSize.z / currentSize.z
        );
        
        if (uniformScaling)
        {
            // Use the smallest scale factor to maintain proportions
            float minFactor = Mathf.Min(factor.x, factor.y, factor.z);
            factor = Vector3.one * minFactor;
        }
        
        return factor;
    }
    
    /// <summary>
    /// Scale multiple models to the same size (useful for batch processing)
    /// </summary>
    public static void ScaleModels(GameObject[] models, Vector3 targetSize, bool uniformScaling = true)
    {
        foreach (GameObject model in models)
        {
            ModelAutoScaler scaler = model.GetComponent<ModelAutoScaler>();
            if (scaler == null)
            {
                scaler = model.AddComponent<ModelAutoScaler>();
            }
            
            scaler.targetSize = targetSize;
            scaler.uniformScaling = uniformScaling;
            scaler.ScaleToTarget();
        }
    }
}

// Editor utility for batch scaling (optional)
#if UNITY_EDITOR


[CustomEditor(typeof(ModelAutoScaler))]
public class ModelAutoScalerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        ModelAutoScaler scaler = (ModelAutoScaler)target;
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Scale to Target"))
        {
            scaler.ScaleToTarget();
        }
        
        if (GUILayout.Button("Reset Scale"))
        {
            scaler.ResetScale();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Scale All Selected Models"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            ModelAutoScaler.ScaleModels(selectedObjects, scaler.targetSize, scaler.uniformScaling);
        }
    }
}
#endif