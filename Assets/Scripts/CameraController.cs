using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeedTouch = 0.1f;
    [SerializeField] private float zoomSpeedMouse = 5f;
    [SerializeField] private float minFOV = 30f;
    [SerializeField] private float maxFOV = 60f;
    
    private Camera cam;
    private float initialTiltX;
    private float targetTiltX = 50f;

    private void Start()
    {
        cam = GetComponent<Camera>();
        initialTiltX = cam.gameObject.transform.eulerAngles.x;
    }

    private void Update()
    {
        HandleTouchZoom();
        #if UNITY_EDITOR
            HandleMouseZoom();
        #endif
        UpdateTiltBasedOnFOV();
    }

    void HandleTouchZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevDistance = Vector2.Distance(prevTouch0, prevTouch1);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            float delta = prevDistance - currentDistance;

            cam.fieldOfView += delta * zoomSpeedTouch;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.fieldOfView -= scroll * zoomSpeedMouse;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }
    void UpdateTiltBasedOnFOV()
    {
        float zoomPercent = (cam.fieldOfView - minFOV) / (maxFOV - minFOV);

        float tiltX = Mathf.Lerp(targetTiltX, initialTiltX, zoomPercent);

        Vector3 euler = cam.gameObject.transform.eulerAngles;
        euler.x = tiltX;
        cam.gameObject.transform.eulerAngles = euler;
    }
}
