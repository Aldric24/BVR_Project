using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loadoutscreen : MonoBehaviour
{

    public List<TMP_Dropdown> hardpointDropdowns;
    public List<Weapon> availableWeapons; // Assuming you have a 'Weapon' class
    SortieScreen sortieScreen;
    private Dictionary<int, Weapon> selectedWeapons; // Store hardpoint-weapon pairings
    Dictionary<string, Dictionary<int, Weapon>> missionLoadouts = new Dictionary<string, Dictionary<int, Weapon>>();

    void Start()
    {
        Debug.Log("Loadout screen started");
        //Weapon empty = new Weapon();
        //empty.weaponName = "Empty";
        //availableWeapons.Add(empty);    
        selectedWeapons = new Dictionary<int, Weapon>();
        sortieScreen = FindObjectOfType<SortieScreen>();
        PopulateDropdowns();
        for (int i = 0; i < hardpointDropdowns.Count; i++)
        {
            OnHardpointDropdownChange(i);
        }
        LoadLoadout(sortieScreen.SelectedMission.name, this);
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
    public void SetHardpointSelection(int hardpointIndex, Weapon weaponToSelect)
    {
        if (hardpointIndex >= 0 && hardpointIndex < hardpointDropdowns.Count)
        {
            // Find the index of the weaponToSelect in the availableWeapons list
            int weaponIndex = availableWeapons.FindIndex(w => w == weaponToSelect);

            if (weaponIndex != -1)
            {
                hardpointDropdowns[hardpointIndex].value = weaponIndex;
            }
            else
            {
                Debug.LogWarning("Weapon not found in available weapons list.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid hardpoint index provided.");
        }
    }
    public void OnHardpointDropdownChange(int hardpointIndex)
    {
        TMP_Dropdown dropdown = hardpointDropdowns[hardpointIndex];
        Weapon selectedWeapon = availableWeapons[dropdown.value];
        selectedWeapons[hardpointIndex] = selectedWeapon;
        SaveLoadout(sortieScreen.SelectedMission.name, selectedWeapons);

    }
    public void SaveLoadout(string missionName, Dictionary<int, Weapon> loadout)
    {
        missionLoadouts[missionName] = new Dictionary<int, Weapon>(loadout); // Create a copy
        SaveMissionData(); // You'll need to implement this serialization part
    }
    public Dictionary<int, Weapon> GetSelectedWeapons()
    {
        return selectedWeapons;
    }
    public void LoadLoadout(string missionName, Loadoutscreen loadoutScreen)
    {
        if (missionLoadouts.ContainsKey(missionName))
        {
            Dictionary<int, Weapon> savedLoadout = missionLoadouts[missionName];
            foreach (var kvp in savedLoadout)
            {
                loadoutScreen.SetHardpointSelection(kvp.Key, kvp.Value);
            }
        }
    }
    public void SaveMissionData()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var kvp in missionLoadouts)
        {
            sb.Append(kvp.Key + ";"); // Mission Name
            foreach (var weaponData in kvp.Value)
            {
                sb.Append(weaponData.Key + "," + weaponData.Value.weaponName + ":"); // Hardpoint Index, Weapon Name
            }
            sb.Append("|"); // Delimiter between missions
        }

        PlayerPrefs.SetString("missionLoadouts", sb.ToString());
        PlayerPrefs.Save();
    }

    public void LoadMissionData()
    {
        string savedData = PlayerPrefs.GetString("missionLoadouts");
        if (!string.IsNullOrEmpty(savedData))
        {
            missionLoadouts.Clear();
            string[] missionsData = savedData.Split('|');

            foreach (string missionData in missionsData)
            {
                string[] parts = missionData.Split(';');
                string missionName = parts[0];
                Dictionary<int, Weapon> loadout = new Dictionary<int, Weapon>();

                for (int i = 1; i < parts.Length; i++)
                {
                    string[] weaponParts = parts[i].Split(':');
                    string[] indexAndName = weaponParts[0].Split(',');
                    int hardpointIndex = int.Parse(indexAndName[0]);
                    string weaponName = indexAndName[1];
                    loadout[hardpointIndex] = availableWeapons.Find(w => w.weaponName == weaponName);
                }

                missionLoadouts[missionName] = loadout;
            }
        }
    }
}
