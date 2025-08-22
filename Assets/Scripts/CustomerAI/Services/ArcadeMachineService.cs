using System;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMachineService : MonoBehaviour, IArcadeService
{
    public static ArcadeMachineService Instance { get; private set; }
    
    public List<ArcadeMachine> arcadeMachines;
    private HashSet<Transform> reservedArcades = new HashSet<Transform>();

    private void Start()
    {
        Instance = this;
    }

    public ArcadeMachine FindAvailableArcade()
    {
        List<ArcadeMachine> availableArcades = new List<ArcadeMachine>();
        
        foreach (var arcade in arcadeMachines)
        {
            if (!reservedArcades.Contains(arcade.CustomerPoint))
                availableArcades.Add(arcade);
        }
        
        if (availableArcades.Count == 0)
            return null;
            
        int randomIndex = UnityEngine.Random.Range(0, availableArcades.Count);
        return availableArcades[randomIndex];
    }
    
    public void ReserveArcade(Transform arcade)
    {
        reservedArcades.Add(arcade);
    }
    
    public void ReleaseArcade(Transform arcade)
    {
        reservedArcades.Remove(arcade);
    }

    public int GetAvailableArcadeCount()
    {
        return arcadeMachines.Count - reservedArcades.Count;
    }
}

