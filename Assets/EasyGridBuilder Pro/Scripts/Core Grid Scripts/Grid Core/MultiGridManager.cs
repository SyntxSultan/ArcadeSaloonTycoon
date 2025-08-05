using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulGames.EasyGridBuilderPro
{
    public class MultiGridManager : MonoBehaviour
    {
        public static MultiGridManager Instance { get; private set; } //This instance

        public event OnActiveGridChangedDelegate OnActiveGridChanged;
        public delegate void OnActiveGridChangedDelegate(EasyGridBuilderPro activeGridSystem);
        
        [Tooltip("Simply select the 'Grid Surface' Layer Mask")]
        public LayerMask mouseColliderLayerMask; //Layermask for raycast hit(Ground layer)

        [HideInInspector]public List<EasyGridBuilderPro> easyGridBuilderProList = new List<EasyGridBuilderPro>();
        [HideInInspector]public EasyGridBuilderPro activeGridSystem;
        [HideInInspector]public bool onGrid;

        private void Awake()
        {
            Instance = this;
            
            foreach (EasyGridBuilderPro grid in FindObjectsByType<EasyGridBuilderPro>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                easyGridBuilderProList.Add(grid);
            }
            
            if (easyGridBuilderProList.Count <= 0)
            {
                Debug.Log("<color=Red>Grid objects not found - Multi Grid Manager</color>");
            }
            else
            {
                activeGridSystem = easyGridBuilderProList[0];
                OnActiveGridChanged?.Invoke(activeGridSystem);
            }
        }

        private void Update()
        {
            if (easyGridBuilderProList.Count <= 0)
            {
                Debug.Log("<color=Red>Grid objects not found - Multi Grid Manager</color>");
                return;
            }
            if (activeGridSystem != GetUsingGrid())
            {
                activeGridSystem = GetUsingGrid();
                OnActiveGridChanged?.Invoke(activeGridSystem);
            }
        }

        private EasyGridBuilderPro GetUsingGrid()
        {
            Collider collider = GetMouseWorldPositionCollider3D();
            if (collider)
            {
                if (collider.gameObject.GetComponent<EasyGridBuilderPro>())
                {
                    onGrid = true;
                    foreach (EasyGridBuilderPro grid in easyGridBuilderProList)
                    {
                        if (collider.gameObject.GetComponent<EasyGridBuilderPro>() == grid)
                        {
                            return grid;
                        }
                    }
                    if (activeGridSystem) return activeGridSystem;
                    else return easyGridBuilderProList[0];
                }
                else
                {
                    //Debug.Log("Colliding but not Grid");
                    onGrid = false;
                    if (activeGridSystem) return activeGridSystem;
                    else return easyGridBuilderProList[0];
                }
            }
            else
            {
                //Debug.Log("Not Colliding");
                onGrid = false;
                if (activeGridSystem) return activeGridSystem;
                else return easyGridBuilderProList[0];
            }
        }

        private Collider GetMouseWorldPositionCollider3D()
        {
            foreach (EasyGridBuilderPro easyGridBuilderLite in easyGridBuilderProList)
            {
                if (Pointer.current == null)
                {
                    Debug.LogError("Pointer Null!");
                    return null;
                }

                Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
                
                if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, easyGridBuilderLite.mouseColliderLayerMask))
                {
                    return raycastHit.collider;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}
