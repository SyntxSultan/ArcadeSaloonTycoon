using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class ASTLibrary
{
    public static bool IsPointerOverUI()
    {
        if (!Application.isPlaying) return false;
        
        // Get the current pointer position from the new Input System
        Vector2 pointerPosition = Pointer.current.position.ReadValue();
    
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = pointerPosition;
    
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    
        foreach (RaycastResult r in results)
        {
            if (r.gameObject.GetComponent<RectTransform>() != null)
            {
                return true;
            }
        }
        return false;
    }
    
    public static void SetLayerRecursive(GameObject targetGameObject, int layer) //Change 'visual' object's and it's children's layer
    {
        targetGameObject.layer = layer; //Set passed object's layer
        foreach (Transform child in targetGameObject.transform) //Loop through all the child objects
        {
            SetLayerRecursive(child.gameObject, layer); //Call the function again but pass the child object
        }
    }
}
