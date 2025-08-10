using System;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMachineService : MonoBehaviour, IArcadeService
{
    public static ArcadeMachineService Instance { get; private set; }
    public List<Transform> arcadeMachines;
    private HashSet<Transform> reservedArcades = new HashSet<Transform>();

    private void Start()
    {
        Instance = this;
    }

    public Transform FindAvailableArcade()
    {
        foreach (Transform arcade in arcadeMachines)
        {
            if (!reservedArcades.Contains(arcade))
                return arcade;
        }
        return null;
    }
    
    public void ReserveArcade(Transform arcade)
    {
        reservedArcades.Add(arcade);
    }
    
    public void ReleaseArcade(Transform arcade)
    {
        reservedArcades.Remove(arcade);
    }
    
    public bool IsArcadeAvailable(Transform arcade)
    {
        return !reservedArcades.Contains(arcade);
    }

    public int GetAvailableArcadeCount()
    {
        return arcadeMachines.Count - reservedArcades.Count;
    }
}

