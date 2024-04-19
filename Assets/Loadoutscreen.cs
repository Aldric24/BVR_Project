using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class Loadoutscreen : MonoBehaviour
{

    public List<TMP_Dropdown> hardpointDropdowns;
    public List<Weapon> availableWeapons; // Assuming you have a 'Weapon' class

    private Dictionary<int, Weapon> selectedWeapons; // Store hardpoint-weapon pairings

    void Start()
    {
        Weapon empty = new Weapon();
        empty.weaponName = "Empty";
        availableWeapons.Add(empty);    
        selectedWeapons = new Dictionary<int, Weapon>();
        PopulateDropdowns();
        for (int i = 0; i < hardpointDropdowns.Count; i++)
        {
            OnHardpointDropdownChange(i);
        }
    }

    void PopulateDropdowns()
    {
        for (int i = 0; i < hardpointDropdowns.Count; i++)
        {
            hardpointDropdowns[i].options.Clear();
            List<string> weaponNames = availableWeapons.ConvertAll(w => w.weaponName);
            hardpointDropdowns[i].AddOptions(weaponNames);
        }
    }

    public void OnHardpointDropdownChange(int hardpointIndex)
    {
        TMP_Dropdown dropdown = hardpointDropdowns[hardpointIndex];
        Weapon selectedWeapon = availableWeapons[dropdown.value];
        selectedWeapons[hardpointIndex] = selectedWeapon;

    }

    public Dictionary<int, Weapon> GetSelectedWeapons()
    {
        return selectedWeapons;
    }
}
