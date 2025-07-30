using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeedTouch = 0.1f;
    [SerializeField] private float zoomSpeedMouse = 5f;
    [SerializeField] private float minFOV = 30f;
    [SerializeField] private float maxFOV = 60f;
    [SerializeField] private float targetTiltX = 50f;
    
    private Camera cam;
    private float initialTiltX;
    private float lastFOV;

    private void Start()
    {
        cam = GetComponent<Camera>();
        initialTiltX = cam.gameObject.transform.eulerAngles.x;
        lastFOV = cam.fieldOfView;
        UpdateTiltBasedOnFOV();
    }

    private void Update()
    {
        HandleTouchZoom();
        
        #if UNITY_EDITOR
            HandleMouseZoom();
        #endif
        
        if (Mathf.Abs(cam.fieldOfView - lastFOV) > 0.001f)
        {
            UpdateTiltBasedOnFOV();
            lastFOV = cam.fieldOfView;
        }
    }

    private void HandleTouchZoom()
    {
        if (!(Touchscreen.current?.touches.Count >= 2)) return;
        
        var touch0 = Touchscreen.current.touches[0];
        var touch1 = Touchscreen.current.touches[1];
            
        if (touch0.isInProgress && touch1.isInProgress)
        {
            Vector2 touch0Pos = touch0.position.ReadValue();
            Vector2 touch1Pos = touch1.position.ReadValue();
                
            Vector2 touch0Delta = touch0.delta.ReadValue();
            Vector2 touch1Delta = touch1.delta.ReadValue();
                
            Vector2 prevTouch0 = touch0Pos - touch0Delta;
            Vector2 prevTouch1 = touch1Pos - touch1Delta;

            float prevDistance = Vector2.Distance(prevTouch0, prevTouch1);
            float currentDistance = Vector2.Distance(touch0Pos, touch1Pos);

            float delta = prevDistance - currentDistance;

            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + delta * zoomSpeedTouch, minFOV, maxFOV);
        }
    }

    private void HandleMouseZoom()
    {
        Vector2 scrollValue = Mouse.current.scroll.ReadValue();
            
        if (Mathf.Abs(scrollValue.y) > 0.01f)
        {
            cam.fieldOfView -= scrollValue.y * zoomSpeedMouse;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }
    
    private void UpdateTiltBasedOnFOV()
    {
        float zoomPercent = (cam.fieldOfView - minFOV) / (maxFOV - minFOV);

        float tiltX = Mathf.Lerp(targetTiltX, initialTiltX, zoomPercent);

        Vector3 euler = cam.gameObject.transform.eulerAngles;
        euler.x = tiltX;
        cam.gameObject.transform.eulerAngles = euler;
    }
}
