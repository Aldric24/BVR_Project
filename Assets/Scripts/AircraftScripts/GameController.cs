using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject enemy;
    [SerializeField] public GameObject WaypointSystem;
    [SerializeField] public GameObject[] Weapons;
    public GameObject Success;
    public GameObject lose;
    public Loadoutscreen Loadoutscreen;
    public Dictionary<int, Weapon> loadout;
    public GameObject panel;
    System.Timers.Timer GameTimer;
    private float timer = 0f;
    private bool timerRunning = false;
    public Dictionary<string, float> bestMissionTimes = new Dictionary<string, float>();
    public string selectedmission;
    [SerializeField]TextMeshProUGUI MissionTime;
    [SerializeField]GameObject End;
       
    [SerializeField]TextMeshProUGUI BestMissionTime;
    [SerializeField]bool gameover = false;
    [SerializeField]bool ingame = false;
    void Awake()
    {
        Loadoutscreen = FindAnyObjectByType<Loadoutscreen>();
        loadout = new Dictionary<int, Weapon>();

        panel= GameObject.Find("Panel");
       
        DontDestroyOnLoad(gameObject);
        
       
    }
    
    void Start()
    {
        
        LoadBestTimes();
        
        
    }
    private void FixedUpdate()
    {
        if (ingame) { checkPlayeralive(); }
       
    }
    public void confirmLoadout()
    {
        Debug.Log("Loadout SIZE " + loadout.Count); // Might be 0 initially
        //Savedloadout = Loadoutscreen.GetSelectedWeapons(); // Get fresh data
        //
        //LoadMissionData();
        Debug.Log("Updated Loadout SIZE " + loadout.Count);

    }
    public void SetPlayerLoadout(Dictionary<int, Weapon> loadout,GameObject playerobj)
    {
       
        for (int i = 0; i < playerobj.GetComponentInChildren<WeaponsManager>().hardpoints.Count; i++)
        {
            GameObject missile = loadout[i].gameObject;
            playerobj.GetComponentInChildren<WeaponsManager>().hardpoints[i].AttachMissile(missile);
        }
        
    }
    public void setmission(string mission)
    {
        selectedmission = mission;
    }
    public void StartGame()
    {
        
        timerRunning = true;
        StartCoroutine(TimerCoroutine());
        //wait for scene to load
        StartCoroutine(waitforsecondsandlOad());
        

    }
    void checkPlayeralive()
    {         
        if (FindAnyObjectByType<NewControl>() == null && !gameover)
        {
            GameOver(false);
            gameover = true;
        }
    }
    void GameEndScreen()
    {

    }
    public IEnumerator waitforsecondsandlOad()
    {
        LeanTween.alphaCanvas(panel.GetComponent<CanvasGroup>(), 1, 2).setOnComplete(() => SceneManager.LoadScene(selectedmission));
        
        yield return new WaitForSeconds(2);
        Debug.Log("Loadout SIZE " + loadout.Count);
        Debug.Log("Scene Loaded");
        GameObject playerobj = Instantiate(player, new Vector3(-7507, 525, 121), Quaternion.identity);
        SetPlayerLoadout(loadout, playerobj);
        ingame = true;
        Debug.Log("Player Loadout set");

    }
    public void GameOver(bool win)
    {
        GetComponentInChildren<CanvasGroup>().blocksRaycasts=true;
        timerRunning = false;
        if (win)
        {
            fadein(End);
            fadein(Success);
            FindAnyObjectByType<NewControl>().gameObject.transform.parent.gameObject.SetActive(false);
            Success.SetActive(true);


            UpdateBestTime(); // Update the dictionary and save
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (bestMissionTimes.ContainsKey(currentSceneName))
            {
                float bestTime = bestMissionTimes[currentSceneName];
                MissionTime.text = "Mission Time: " + timer + " Seconds";
                BestMissionTime.text = "Best Mission Time: " + bestTime +" Seconds";
            }
            else
            {
                GameObject.Find("MissionTime").GetComponent<TextMeshProUGUI>().text = "Mission Time: " + timer; // No best time saved yet 
            }

            Debug.Log("You Win");

        }
        else
        {
            timerRunning = false;
            fadein(End);
            fadein(lose);
            lose.SetActive(true);

           
        }
       

        
    }
    void UpdateBestTime()
    {
        LoadBestTimes(); // Load the saved data

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (bestMissionTimes.ContainsKey(currentSceneName))
        {
            if (timer < bestMissionTimes[currentSceneName])
            {
                bestMissionTimes[currentSceneName] = timer;
                SaveBestTime(currentSceneName,timer);
            }
        }
        else
        {
            // Scene name not found, save the time
            bestMissionTimes[currentSceneName] = timer;
            SaveBestTime(currentSceneName,timer);
        }
    }
    public void Refly()
    {
      //reload the scene
        
        StartCoroutine(transitiontoMainMenue());
        MenuController menuController = FindAnyObjectByType<MenuController>();
        
    }
    public void mainmenu()
    {
        Debug.Log("Main Menu");
        StartCoroutine(transitiontoMainMenue());
    }
    public IEnumerator transitiontoMainMenue()
    {
        fadeOut(End);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenu");
        MenuController menuController = FindAnyObjectByType<MenuController>();
       
    }
    IEnumerator TimerCoroutine()
    {
        while (timerRunning)
        {
            timer += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Timer is stopped (e.g., player died or objectives achieved)
        Debug.Log("Timer stopped. Time: " + timer);
    }
    void SaveBestTime(string missionName, float time)
    {
        bestMissionTimes[missionName] = time;
        PlayerPrefs.SetString("bestTimesData", SerializeBestTimes());
    }

    string SerializeBestTimes() // Helper to convert dictionary to a savable string
    {
        // You'll need to choose a serialization format (e.g., JSON)
        // For simplicity, let's assume a comma-separated format
        StringBuilder sb = new StringBuilder();
        foreach (var kvp in bestMissionTimes)
        {
            sb.Append(kvp.Key + "," + kvp.Value + ";");
        }
        return sb.ToString();
    }
    public void LoadBestTimes()
    {
        string savedData = PlayerPrefs.GetString("bestTimesData");
        if (!string.IsNullOrEmpty(savedData))
        {
            bestMissionTimes = DeserializeBestTimes(savedData);
        }
    }
    void fadein(GameObject mission)
    {
        //turn up the alpha of the mission and then enable it
        mission.SetActive(true);
        LeanTween.alphaCanvas(mission.GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutSine);
    }
    void fadeOut(GameObject mission)
    {
        //turn down the alpha of the mission using canvas group and then disable it
        LeanTween.alphaCanvas(mission.GetComponent<CanvasGroup>(), 0, 1.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => mission.SetActive(false));


    }
    Dictionary<string, float> DeserializeBestTimes(string data)
    {
        Dictionary<string, float> result = new Dictionary<string, float>();
        string[] entries = data.Split(';');
        foreach (string entry in entries)
        {
            string[] parts = entry.Split(',');
            if (parts.Length == 2)
            {
                result[parts[0]] = float.Parse(parts[1]);
            }
        }
        return result;
    }
}
