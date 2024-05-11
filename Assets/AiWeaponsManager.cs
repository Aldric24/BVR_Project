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
    // Hardpoints
    // public List<HardPoint> hardpoints = new List<HardPoint>();

    //// Weapon Types
    //public List<string> availableWeaponTypes = new List<string>();
    //public string currentMissileType = null;
    //private int currentWeaponTypeIndex = 0;
    //private int currentMissileIndex = 0;

    //// Start (for Initialization)
    //public void Initialize()
    //{
    //    Tempmattach();
    //    SwitchMissile();
    //}

    //void Tempmattach()
    //{
    //    foreach (HardPoint hardpoint in hardpoints)
    //    {
    //        hardpoint.AttachMissile(hardpoint.missile);
    //    }
    //}

    //// Weapon Switching and Cycling
    //public void SwitchMissile()
    //{
    //    List<string> missileTypes = new List<string> { "Fox1", "Fox2", "Fox3" };

    //    do
    //    {
    //        currentWeaponTypeIndex = (currentWeaponTypeIndex + 1) % missileTypes.Count;
    //        currentMissileType = missileTypes[currentWeaponTypeIndex];
    //    } while (hardpoints.Count(hp => hp.missiletype == currentMissileType) == 0);

    //    currentMissileIndex = FindNextAvailableOfType(currentMissileType);
    //    DisableAllMissileScripts();
    //}

    //// Missile Firing
    //public void FireMissile()
    //{
    //    if (currentMissileType != null)
    //    {
    //        FireCurrentMissile();
    //    }
    //}

    //private void FireCurrentMissile()
    //{
    //    int nextIndex = currentMissileIndex;

    //    if (nextIndex != -1)
    //    {
    //        hardpoints[nextIndex].Fire(this);
    //        currentMissileIndex = FindNextAvailableOfType(currentMissileType);

    //    }
    //    else
    //    {
    //        Debug.LogWarning($"No more {currentMissileType} missiles available!");
    //    }
    //}

    //// Helper Methods
    //private int FindNextAvailableOfType(string type)
    //{
    //    for (int i = (currentMissileIndex + 1) % hardpoints.Count; i != currentMissileIndex; i = (i + 1) % hardpoints.Count)
    //    {
    //        if (hardpoints[i].missiletype == type && !hardpoints[i].MissileFired)
    //        {
    //            hardpoints[i].EnableMissile();
    //            return i;
    //        }
    //    }
    //    return -1;
    //}

    //private void DisableAllMissileScripts()
    //{
    //    foreach (HardPoint hardpoint in hardpoints)
    //    {
    //        if (hardpoint.missiletype != currentMissileType)
    //        {
    //            hardpoint.DisableMissile();
    //        }
    //    }
    //}
}
