using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
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
    private Dictionary<int, Weapon> loadout;
    public GameObject panel;
    Timer GameTimer;
    private float timer = 0f;
    private bool timerRunning = false;
    Dictionary<string, float> bestMissionTimes = new Dictionary<string, float>();
    public string selectedmission;
    void Awake()
    {
        timerRunning = true;
        StartCoroutine(TimerCoroutine());
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        
        loadout = Loadoutscreen.GetSelectedWeapons();
    }
    public void confirmLoadout()
    {
       loadout = Loadoutscreen.GetSelectedWeapons();
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
        
        
        //wait for scene to load
        StartCoroutine(waitforsecondsandlOad());
        

    }
    void GameEndScreen()
    {

    }
    public IEnumerator waitforsecondsandlOad()
    {
        LeanTween.alphaCanvas(panel.GetComponent<CanvasGroup>(), 1, 2).setOnComplete(() => SceneManager.LoadScene("LV1"));
        
        yield return new WaitForSeconds(2);
        GameObject playerobj = Instantiate(player, new Vector3(1661, 219, 60), Quaternion.identity);
        SetPlayerLoadout(loadout, playerobj);
       
        
    }
    public void GameOver(bool win)
    {
       
        timerRunning = false;
        if (win)
        {
            fadein(Success);
            FindAnyObjectByType<NewControl>().gameObject.transform.parent.gameObject.SetActive(false);
            Success.SetActive(true);


            UpdateBestTime(); // Update the dictionary and save
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (bestMissionTimes.ContainsKey(currentSceneName))
            {
                float bestTime = bestMissionTimes[currentSceneName];
                GameObject.Find("Time").GetComponent<TextMeshProUGUI>().text = "Time To Target: " + timer + " Seconds";
                GameObject.Find("Best Mission Time:").GetComponent<TextMeshProUGUI>().text = "Best Time: " + bestTime +" Seconds";
            }
            else
            {
                GameObject.Find("Time").GetComponent<TextMeshProUGUI>().text = "Time To Target: " + timer; // No best time saved yet 
            }

            Debug.Log("You Win");

        }
        else
        {
            Debug.Log("You Lose");
        }
        timerRunning = false;

        StartCoroutine(transitiontoMainMenue());
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
    public IEnumerator transitiontoMainMenue()
    {
        panel.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenu");
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
    void LoadBestTimes()
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
