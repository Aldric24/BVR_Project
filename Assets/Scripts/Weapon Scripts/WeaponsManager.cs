using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    // List to store hardpoint references
    public List<HardPoint> hardpoints = new List<HardPoint>();

    // Currently equipped cannon object (if applicable)
    public Cannon equippedCannon;
    // Index for the currently selected weapon (0-based)
    [SerializeField]private int currentWeaponIndex = 0;
    public GameObject target;
    [SerializeField] private SweepRotation S;
    [SerializeField] TextMeshProUGUI Weapon;
    int missilecount = 0;
    public List<string> availableWeaponTypes = new List<string>(); // To track found weapon types
    private int currentWeaponTypeIndex = 0;
   // All available missiles
    public string currentMissileType = null; // Currently equipped missile type (or null)
    private int currentMissileIndex = 0; // Index within the current missile type

    private int currentListIndex = 0; // Index of the currently equipped list
    private List<HardPoint> currentMissileList; // Reference to the active list
    void Start()
    {
        Tempmattach();
        missilecount = hardpoints.Count;
    }
    void FixedUpdate()
    {


        //Update UI
        if (availableWeaponTypes.Count > 0)
        {
            // Get the currently selected weapon type (if any)
            string weaponTypeToDisplay = currentWeaponTypeIndex < availableWeaponTypes.Count
                                           ? availableWeaponTypes[currentWeaponTypeIndex]
                                           : "None";
            Weapon.text = weaponTypeToDisplay == "Cannon" ? "Gun" : weaponTypeToDisplay + ": " + missilecount;
        }

        target = S.LocekdTarget;
    }
    void Tempmattach()
    {
        //availableWeaponTypes.Clear();
        foreach (HardPoint hardpoint in hardpoints)
        {
            hardpoint.AttachMissile(hardpoint.missile);
            
        }
    }
    
   

    public void SwitchWeapon()
    {
        // Cycle through weapon types (you might add "Cannon" as an option here)
        List<string> weaponTypes = new List<string> { "Fox1", "Fox2", "Fox3" };
        int typeIndex = weaponTypes.IndexOf(currentMissileType);
        typeIndex = (typeIndex + 1) % weaponTypes.Count;
        currentMissileType = weaponTypes[typeIndex];

        currentMissileIndex = 0; // Reset index for the new type
        FindNextAvailableOfType(currentMissileType); // Enable the first available missile
        // Enable/Disable scripts
        DisableAllMissileScripts();
        UpdateWeaponUI();
    }


    public void FireWeapon()
    {
        if (currentMissileType == "Cannon")
        {
            // ... Fire cannon logic ...
        }
        else if (currentMissileType != null)
        {
            FireCurrentMissile();
        }
    }

    //public void SwitchWeapon()
    //{
    //    currentWeaponTypeIndex = (currentWeaponTypeIndex + 1) % availableWeaponTypes.Count;
    //    if (currentWeaponTypeIndex == 0) { equippedCannon.cannonequipped = true; }
    //    else { equippedCannon.cannonequipped = false; }
    //    string desiredWeaponType = availableWeaponTypes[currentWeaponTypeIndex];



    private void FireCurrentMissile()
    {
        
        int nextIndex = currentMissileIndex;

        if (nextIndex != -1)
        {
            hardpoints[nextIndex].Fire(this);
            currentMissileIndex = FindNextAvailableOfType(currentMissileType);
            missilecount--; // Assuming you still want this
        }
        else
        {
            Debug.LogWarning($"No more {currentMissileType} missiles available!");
        }
    }

    private int FindNextAvailableOfType(string type)
    {
        // Start searching from the next index 
        for (int i = (currentMissileIndex + 1) % hardpoints.Count; i != currentMissileIndex; i = (i + 1) % hardpoints.Count)
        {
            if (hardpoints[i].missiletype == type && !hardpoints[i].MissileFired)
            {
                hardpoints[i].EnableMissile();
                return i;
            }
        }
        return -1; // None found
    }
    //}
    //private int FindNextAvailableInList(List<HardPoint> missileList)
    //{
    //    for (int i = 0; i < missileList.Count; i++)
    //    {
    //        if (hardpoints[i].missile != null && hardpoints[i].missiletype == missileList.ToString() && !hardpoints[i].MissileFired)
    //        {
    //            return i;
    //        }
    //    }
    //    return -1;
    //}
    private void EnableCurrentMissileScript()
    {
        int nextIndex = FindNextAvailableOfType(currentMissileType);
        hardpoints[nextIndex].EnableMissile();
    }

    private void DisableAllMissileScripts()
    {
        // (Implement based on your missile script setup)
    }

    private void UpdateWeaponUI()
    {
        // ... Update your UI based on currentMissileType ...
    }
   

}
