using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

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
    [SerializeField] private float movementThreshold = 10f; 
    
    [SerializeField] private float momentumDecayRate = 0.92f;
    [SerializeField] private float minMomentumThreshold = 0.001f; 
    
    [SerializeField] private GameObject ripplePrefab;
    [SerializeField] private Canvas canvas;
    
    private const float zoomSpeedMouse = 5f;
    private CinemachineCamera virtualCamera;
    private float initialTiltX;
    
    private Vector3 cameraVelocity = Vector3.zero;
    private bool wasTouchingLastFrame;
    private Vector2 touchStartPosition;
    private bool isMovementStarted;
    
    private float lastFOV;
    private Vector3 initialPosition;
    private bool isRippleSpawned;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Camera component not found!");
            return;
        }

        initialTiltX = transform.eulerAngles.x;
        initialPosition = transform.position;
        lastFOV = virtualCamera.Lens.FieldOfView;
        UpdateTiltBasedOnFOV();
    }

    private void Update()
    {
        if (virtualCamera == null) return;

        HandleTouchEffect();
        HandleTouchInput();
        HandleMomentum();

#if UNITY_EDITOR
        HandleMouseZoom();
#endif
        
        if (Mathf.Abs(virtualCamera.Lens.FieldOfView - lastFOV) > 0.001f)
        {
            UpdateTiltBasedOnFOV();
            lastFOV = virtualCamera.Lens.FieldOfView;
        }
    }

    private void HandleTouchEffect()
    {
        if (!Touchscreen.current.enabled) return;

        var touch = Touchscreen.current.touches[0];
        var phase = touch.phase.ReadValue();

        if (!isRippleSpawned && phase == TouchPhase.Began) 
        {
            Vector2 pos = touch.position.ReadValue();
            
            if (RipplePool.Instance != null)
            {
                RipplePool.Instance.GetRipple(pos, canvas.transform);
            }
            
            isRippleSpawned = true;
        }
        else if (phase == TouchPhase.Ended || 
                 phase == TouchPhase.Canceled)
        {
            isRippleSpawned = false;
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
                TouchPhase.Began or 
                TouchPhase.Moved or 
                TouchPhase.Stationary)
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
            HandleSingleTouchMovement();
        }
        else if (wasTouchingLastFrame)
        {
            wasTouchingLastFrame = false;
        }
    }

    private void HandleSingleTouchMovement()
    {
        var touch = Touchscreen.current.touches[0];
        var phase = touch.phase.ReadValue();
        
        if (phase == TouchPhase.Began)
        {
            touchStartPosition = touch.position.ReadValue();
            isMovementStarted = false;
        }
        else if (phase == TouchPhase.Moved)
        {
            wasTouchingLastFrame = true;
            Vector2 currentTouchPosition = touch.position.ReadValue();
            
            if (!isMovementStarted)
            {
                float distanceFromStart = Vector2.Distance(touchStartPosition, currentTouchPosition);
                if (distanceFromStart < movementThreshold)
                {
                    return;
                }
                else
                {
                    isMovementStarted = true;
                }
            }
            
            Vector2 touchDelta = touch.delta.ReadValue();
            
            Vector3 movement = new Vector3(-touchDelta.x * movementSpeed, 0, -touchDelta.y * movementSpeed);
            
            cameraVelocity = movement / Time.deltaTime;
            
            Vector3 newPosition = transform.position + movement;
            
            Vector3 offset = newPosition - initialPosition;
            offset = Vector3.ClampMagnitude(offset, maxMoveDistance);
            newPosition = initialPosition + offset;
            
            transform.position = newPosition;
        }
        else if (wasTouchingLastFrame)
        {
            wasTouchingLastFrame = false;
            isMovementStarted = false;
        }
    }
    
    private void HandleMomentum()
    {
        if (!wasTouchingLastFrame && cameraVelocity.magnitude > minMomentumThreshold)
        {
            Vector3 movement = cameraVelocity * Time.deltaTime;
            
            Vector3 newPosition = transform.position + movement;
            
            Vector3 offset = newPosition - initialPosition;
            offset = Vector3.ClampMagnitude(offset, maxMoveDistance);
            newPosition = initialPosition + offset;
            
            transform.position = newPosition;
            
            cameraVelocity *= momentumDecayRate;
        }
        else if (cameraVelocity.magnitude <= minMomentumThreshold)
        {
            cameraVelocity = Vector3.zero;
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

            var lens = virtualCamera.Lens;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView + delta * zoomSpeedTouch, minFOV, maxFOV);
            virtualCamera.Lens = lens;
        }
    }

    #if UNITY_EDITOR
    private void HandleMouseZoom()
    {
        Vector2 scrollValue = Mouse.current.scroll.ReadValue();
            
        if (Mathf.Abs(scrollValue.y) > 0.01f)
        {
            var lens = virtualCamera.Lens;
            lens.FieldOfView -= scrollValue.y * zoomSpeedMouse;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minFOV, maxFOV);
            virtualCamera.Lens = lens;
        }
    }
    #endif
    
    private void UpdateTiltBasedOnFOV()
    {
        float zoomPercent = (virtualCamera.Lens.FieldOfView - minFOV) / (maxFOV - minFOV);

        float tiltX = Mathf.Lerp(targetTiltX, initialTiltX, zoomPercent);

        Vector3 euler = transform.eulerAngles;
        euler.x = tiltX;
        transform.eulerAngles = euler;
    }
}