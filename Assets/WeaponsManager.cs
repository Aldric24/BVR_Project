using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    // List to store hardpoint references
    public List<HardPoint> hardpoints = new List<HardPoint>();

    // Currently equipped cannon object (if applicable)
    public Cannon equippedCannon;
    private int currentMissileIndex = 0;
    // Index for the currently selected weapon (0-based)
    [SerializeField]private int currentWeaponIndex = 0;
    public GameObject target;
    [SerializeField] private SweepRotation S;
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        target = S.LocekdTarget;
    }

    // Function to switch between cannon and missiles
    public void SwitchWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % 2; // Cycle between cannon (0) and missiles (1)

        // Reset missile index if switching away from missiles
        if (currentWeaponIndex != 1)
        {
            currentMissileIndex = 0;
        }
    }
    private bool HasAvailableMissile()
    {
        foreach (HardPoint hardpoint in hardpoints)
        {
            if (hardpoint.Missile != null && !hardpoint.MissileFired)
            {
                return true;
            }
        }
        return false;
    }
    // Function to Fire Selected Weapon
    public void FireWeapon()
    {
        if (currentWeaponIndex == 0)
        {
           equippedCannon.StartFiring();
            
        }
        else // currentWeaponIndex == 1 (missiles)
        {
            if (HasAvailableMissile())
            {
                hardpoints[currentMissileIndex].Fire(this);
                currentMissileIndex = (currentMissileIndex + 1) % hardpoints.Count;
            }
            else
            {
                Debug.LogWarning("No missiles available to fire!");
            }
        }
    }

}
