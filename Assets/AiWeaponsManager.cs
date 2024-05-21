using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AiWeaponsManager : WeaponsManager
{
    EnemyAI ai ;
    public HardPoint selectedHardpoint;
    private void Start()
    {
        ai=GetComponent<EnemyAI>();
        Tempmattach();
        SwitchMissile();
        
    }
    private void FixedUpdate()
    {
        if (ai.target != null)
        {
            target = ai.target;
        }
      
    }

    public void SwitchToMissileType(string selectedMissileType)
    {
        // Iterate through hardpoints to find the correct missile type
        foreach (HardPoint hardpoint in hardpoints)
        {
            if (hardpoint.missiletype == selectedMissileType && hardpoint.MissileFired!=true)
            {
                Debug.Log("Switching to hardpoint: " + hardpoint.name + " with missile type: " + selectedMissileType);
                selectedHardpoint = hardpoint; // Update the selected hardpoint
               
            }
            else
                Debug.LogWarning($"Missile type '{selectedMissileType}' not found or cannot fire. Using default hardpoint.");
        }

        // Handle case where the specified missile type is not found or available
       
       
    }
    
    
}
