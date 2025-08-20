using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeedTouch = 0.1f;
    [SerializeField] private float minFOV = 15f;
    [SerializeField] private float maxFOV = 50f;
    [SerializeField] private float targetTiltX = 50f;
    
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 0.01f;
    [SerializeField] private float maxMoveDistance = 10f;
    
    [SerializeField] private GameObject ripplePrefab;
    [SerializeField] private Canvas canvas;
    
    private const float zoomSpeedMouse = 5f;
    private Camera cam;
    private float initialTiltX;
    private float lastFOV;
    private Vector3 initialPosition;
    private bool isRippleSpawned;

    private void Start()
    {
        cam = GetComponent<Camera>();
        initialTiltX = cam.gameObject.transform.eulerAngles.x;
        initialPosition = cam.gameObject.transform.position;
        lastFOV = cam.fieldOfView;
        UpdateTiltBasedOnFOV();
    }

    private void Update()
    {
        var touch = Touchscreen.current.touches[0];
        var phase = touch.phase.ReadValue();

        Debug.Log(phase);
        if (!isRippleSpawned && phase == UnityEngine.InputSystem.TouchPhase.Began) 
        {
            Vector2 pos = touch.position.ReadValue();
            var ripple = Instantiate(ripplePrefab, canvas.transform);
            ripple.GetComponent<RectTransform>().position = pos;
            isRippleSpawned = true;
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended || 
                 phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            isRippleSpawned = false;
        }
        
        HandleTouchInput();
        
#if UNITY_EDITOR
        HandleMouseZoom();
#endif
        
        if (Mathf.Abs(cam.fieldOfView - lastFOV) > 0.001f)
        {
            UpdateTiltBasedOnFOV();
            lastFOV = cam.fieldOfView;
        }
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null || GridBridge.Instance.IsGridModeBuilding()) return;
        
        int touchCount = 0;
        for (int i = 0; i < Touchscreen.current.touches.Count; i++)
        {
            var touch = Touchscreen.current.touches[i];
            var phase = touch.phase.ReadValue();
            if (phase is 
                UnityEngine.InputSystem.TouchPhase.Began or 
                UnityEngine.InputSystem.TouchPhase.Moved or 
                UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                touchCount++;
            }
        }
        if (ASTLibrary.IsPointerOverUI()) return;
        if (touchCount == 1)
        {
            HandleSingleTouchMovement();
        }
        else if (touchCount >= 2)
        {
            HandleTouchZoom();
        }
    }

    private void HandleSingleTouchMovement()
    {
        var touch = Touchscreen.current.touches[0];
        
        if (touch.isInProgress)
        {
            Vector2 touchDelta = touch.delta.ReadValue();
            
            Vector3 movement = new Vector3(-touchDelta.x * movementSpeed, 0, -touchDelta.y * movementSpeed);
            
            Vector3 newPosition = cam.gameObject.transform.position + movement;
            
            Vector3 offset = newPosition - initialPosition;
            offset = Vector3.ClampMagnitude(offset, maxMoveDistance);
            newPosition = initialPosition + offset;
            
            cam.gameObject.transform.position = newPosition;
        }
    }

    private void HandleTouchZoom()
    {
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

    #if UNITY_EDITOR
    private void HandleMouseZoom()
    {
        Vector2 scrollValue = Mouse.current.scroll.ReadValue();
            
        if (Mathf.Abs(scrollValue.y) > 0.01f)
        {
            cam.fieldOfView -= scrollValue.y * zoomSpeedMouse;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }
    #endif
    
    private void UpdateTiltBasedOnFOV()
    {
        float zoomPercent = (cam.fieldOfView - minFOV) / (maxFOV - minFOV);

        float tiltX = Mathf.Lerp(targetTiltX, initialTiltX, zoomPercent);

        Vector3 euler = cam.gameObject.transform.eulerAngles;
        euler.x = tiltX;
        cam.gameObject.transform.eulerAngles = euler;
    }
}