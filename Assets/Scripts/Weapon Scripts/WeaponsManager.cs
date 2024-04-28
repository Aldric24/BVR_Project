using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private int flareCount;
    [SerializeField] private float lastFlareDeployTime;
    [SerializeField] private int cooldown;
    [SerializeField] private int flaresPerDeployment;
    [SerializeField] GameObject flarePrefab;
    private Vector2 dispersion;

    void Start()
    {
        Tempmattach();
        SwitchMissile();
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



    public void FireMissile()
    {
        if (currentMissileType != null)
        {
            FireCurrentMissile();
        }
    }

    public void SwitchMissile() // Renamed for clarity
    {
        List<string> missileTypes = new List<string> { "Fox1", "Fox2", "Fox3" }; // Only missile types

        do
        {
            currentWeaponTypeIndex = (currentWeaponTypeIndex + 1) % missileTypes.Count;
            currentMissileType = missileTypes[currentWeaponTypeIndex];
        } while (hardpoints.Count(hp => hp.missiletype == currentMissileType) == 0);

        currentMissileIndex = FindNextAvailableOfType(currentMissileType);
        

        DisableAllMissileScripts();
        UpdateWeaponUI();
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
        foreach (HardPoint hardpoint in hardpoints)
        {
            if (hardpoint.missiletype != currentMissileType)
            {
                hardpoint.DisableMissile();
            }
        }
    }
    public void DeployFlares()
    {
        if (flareCount > 0 && Time.time > lastFlareDeployTime + cooldown)
        {
            for (int i = 0; i < flaresPerDeployment; i++)
            {
                Vector3 offset = Random.insideUnitCircle * dispersion;
                GameObject flare = Instantiate(flarePrefab, transform.position + offset, transform.rotation);
            }

            flareCount--;
            lastFlareDeployTime = Time.time;
        }
    }
    private void UpdateWeaponUI()
    {
        if (availableWeaponTypes.Count == 0)  // Check for any weapons at all
        {
            Weapon.text = "No weapons available";
            return;
        }

        // Ensure the current type is a missile type
        string weaponTypeToDisplay = currentWeaponTypeIndex < availableWeaponTypes.Count
                                     && IsMissileType(availableWeaponTypes[currentWeaponTypeIndex])
                                     ? availableWeaponTypes[currentWeaponTypeIndex]
                                     : "None";

        // Find the count of missiles of the current type
        int currentTypeCount = hardpoints.Count(hp => hp.missiletype == weaponTypeToDisplay);
        Weapon.text = weaponTypeToDisplay + ": " + currentTypeCount;
    }

    // Helper function to check if a weapon type is a missile
    private bool IsMissileType(string weaponType)
    {
        return weaponType == "Fox1" || weaponType == "Fox2" || weaponType == "Fox3";
    }


}