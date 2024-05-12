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
    public Dictionary<string, Dictionary<int, Weapon>> missionLoadouts = new Dictionary<string, Dictionary<int, Weapon>>();
    Dictionary<int, Weapon> Savedloadout = new Dictionary<int, Weapon>();
    void Start()
    {
        Debug.Log("Loadout screen started");
        selectedWeapons = new Dictionary<int, Weapon>();
        //Weapon empty = new Weapon();
        //empty.weaponName = "Empty";
        //availableWeapons.Add(empty);    
        sortieScreen = FindObjectOfType<SortieScreen>();
        PopulateDropdowns();
        for (int i = 0; i < hardpointDropdowns.Count; i++)
        {
            OnHardpointDropdownChange(i);
        }
        FindAnyObjectByType<GameController>().loadout = selectedWeapons;
        LoadMissionData();
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
    void attachweapons(int hardpointIndex)
    {
        TMP_Dropdown dropdown = hardpointDropdowns[hardpointIndex];
        Weapon selectedWeapon = availableWeapons[dropdown.value];
        selectedWeapons[hardpointIndex] = selectedWeapon;
    }
    public void OnHardpointDropdownChange(int hardpointIndex)
    {
        attachweapons(hardpointIndex);
        //TMP_Dropdown dropdown = hardpointDropdowns[hardpointIndex];
        //Weapon selectedWeapon = availableWeapons[dropdown.value];
        //selectedWeapons[hardpointIndex] = selectedWeapon;
        //FindAnyObjectByType<GameController>().loadout = selectedWeapons;
        //Debug.Log("Dictionary size:" + selectedWeapons.Count);
        //SaveLoadout(FindAnyObjectByType<GameController>().selectedmission, selectedWeapons);
        


    }
    public void SaveLoadout()
    {
        // Save the selected weapons to the GameController
        SaveLoadout(FindAnyObjectByType<GameController>().selectedmission, selectedWeapons);
    }
    public void LoadData()
    {
        LoadMissionData(FindAnyObjectByType<GameController>().selectedmission);
        string missionName = FindAnyObjectByType<GameController>().selectedmission; // Replace with your mission selection logic

        // Check if the mission exists in your dictionary
        //if (!missionLoadouts.ContainsKey(missionName))
        //{
        //    Debug.LogError("Mission not found: " + missionName);
        //    return; // Exit the function if the mission is missing
        //}

        //// Get the saved loadout dictionary
        //Dictionary<int, Weapon> savedLoadout = missionLoadouts[missionName];

        //// Check if the saved loadout is null
        //if (savedLoadout == null)
        //{
        //    Debug.LogError("Saved loadout data is null for mission: " + missionName);
        //    return; // Exit the function if the loadout is null
        //}

        // Hardpoint Setting (Needs Adjustment)
        for (int i = 0; i < hardpointDropdowns.Count; i++)
        {
            // Additional check: Make sure the savedLoadout has data for this hardpoint
            if (Savedloadout.ContainsKey(i))
            {
                hardpointDropdowns[i].value = availableWeapons.FindIndex(w => w == Savedloadout[i]);
            }
            else
            {
                Debug.LogWarning($"No saved loadout data found for hardpoint index {i} in mission {missionName}.");
                // You might want to handle this case by setting a default weapon or doing nothing.
            }
        }

        // Finally, assign the loadout to the GameController
        FindAnyObjectByType<GameController>().loadout = Savedloadout;
    }
    public void SaveLoadout(string missionName, Dictionary<int, Weapon> loadout)
    {
        missionLoadouts[missionName] = new Dictionary<int, Weapon>(loadout); // Create a copy
        SaveMissionData(); // You'll need to implement this serialization part
    }
    public Dictionary<int, Weapon> GetSelectedWeapons() // Change return type
    {
        Debug.Log("Returning count "+ selectedWeapons.Count);
        return selectedWeapons;  // Return the dictionary 
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
            Debug.Log("Mission Name: " + kvp.Key);

            foreach (var weaponData in kvp.Value)
            {
                int weaponIndex = availableWeapons.FindIndex(w => w == weaponData.Value);
                sb.Append(weaponData.Key + "," + weaponIndex + ":");
                Debug.Log("Hardpoint: " + weaponData.Key + ", Weapon Index: " + weaponIndex);
            }

            sb.Append("|");
        }

        Debug.Log("Final Save String: " + sb.ToString()); // See the complete string
        PlayerPrefs.SetString("missionLoadouts", sb.ToString());
        PlayerPrefs.Save();
    }
    public void LoadMissionData(string missionNameToLoad)
    {
        
        string savedData = PlayerPrefs.GetString("missionLoadouts");
        Debug.Log("Loading mission data for: " + savedData);
        if (!string.IsNullOrEmpty(savedData))
        {
            Savedloadout.Clear();
            string[] missionsData = savedData.Split('|');

            foreach (string missionData in missionsData)
            {
                Debug.Log("Mission Data Chunk: " + missionData); // Examine each mission's data

                string[] parts = missionData.Split(';');
                string missionName = parts[0];

                if (missionName == missionNameToLoad)
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string[] weaponParts = parts[i].Split(':');
                        // Nested Loop!
                        foreach (string weaponPart in weaponParts)
                        {
                            if(weaponPart == "") continue; // Skip empty entries
                            string[] indexAndName = weaponPart.Split(',');
                            int hardpointIndex = int.Parse(indexAndName[0]);
                            int weaponIndex = int.Parse(indexAndName[1]);

                            Debug.Log("Hardpoint: " + hardpointIndex + ", Weapon Index: " + weaponIndex);
                            Savedloadout[hardpointIndex] = availableWeapons[weaponIndex];
                        }
                    }
                    break;
                }
            }
        }
    }
    public void LoadMissionData()
    {
        string savedData = PlayerPrefs.GetString("missionLoadouts");

        if (!string.IsNullOrEmpty(savedData))
        {
            string[] missionsData = savedData.Split('|');

            foreach (string missionData in missionsData)
            {
                string[] parts = missionData.Split(';');
                string missionName = parts[0];

                if (!missionLoadouts.ContainsKey(missionName)) // Only if mission isn't already loaded
                {
                    missionLoadouts[missionName] = new Dictionary<int, Weapon>();
                }

                for (int i = 1; i < parts.Length; i++)
                {
                    string[] weaponParts = parts[i].Split(':');

                    // Nested loop to handle multiple weapons per mission
                    foreach (string weaponPart in weaponParts)
                    {
                        if (string.IsNullOrEmpty(weaponPart)) continue;
                        string[] indexAndName = weaponPart.Split(',');
                        int hardpointIndex = int.Parse(indexAndName[0]);
                        int weaponIndex = int.Parse(indexAndName[1]);
                        missionLoadouts[missionName][hardpointIndex] = availableWeapons[weaponIndex];
                    }
                }
            }
        }

        // Load UI from the missionLoadouts if the mission is currently selected
        if (missionLoadouts.ContainsKey(FindAnyObjectByType<GameController>().selectedmission))
        {
            Savedloadout = missionLoadouts[FindAnyObjectByType<GameController>().selectedmission];
        }

        // Call LoadData() here to update the UI with the loaded mission's weapons
        LoadData();
    }


}
