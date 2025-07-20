using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreviewRotator : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private float rotationSpeed = 1f;

    private GameObject targetModel;
    private Vector2 lastMousePosition;

    private void OnEnable()
    {
        if (PreviewSpawner.Instance != null)
        {
            PreviewSpawner.Instance.OnPreviewSpawned += SetTarget;
        }
        else
        {
            Debug.LogWarning("PreviewSpawner.Instance is null in PreviewRotator.OnEnable()");
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (PreviewSpawner.Instance != null)
        {
            PreviewSpawner.Instance.OnPreviewSpawned -= SetTarget;
        }
    }

    private void SetTarget(GameObject newTarget)
    {
        targetModel = newTarget;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (targetModel == null)
        {
            Debug.LogWarning("Target model not set!");
            return;
        }
        
        Vector2 delta = eventData.position - lastMousePosition;
        lastMousePosition = eventData.position;

        float rotationY = -delta.x * rotationSpeed;
        targetModel.transform.Rotate(Vector3.up, rotationY, Space.World);
    }
}
