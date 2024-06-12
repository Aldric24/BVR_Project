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
    public bool ingame = false;
    [SerializeField] bool Win = false;
    int showscreen;
    [SerializeField] TextMeshProUGUI stall;
    bool warning = false;
    [SerializeField] bool playeractive = false;
    void Awake()
    {
        Loadoutscreen = FindAnyObjectByType<Loadoutscreen>();
        loadout = new Dictionary<int, Weapon>();
       
        panel = GameObject.Find("Panel");
        DontDestroyOnLoad(gameObject);
        GameObject[] gameControllers = GameObject.FindGameObjectsWithTag("GameController"); // Or whatever tag you use
        if (gameControllers.Length > 1)
        {
            Destroy(gameObject);  // Destroy this instance if another already exists
        }
    }
    
    void Start()
    {
        
        LoadBestTimes();
       
        
    }
    private void FixedUpdate()
    {
        if (ingame) {
            playeractive = player.activeSelf;
            checkPlayeralive();
            
        
        }
       
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
            playerobj.GetComponent<WeaponsManager>().hardpoints[i].AttachMissile(missile);
        }

        
    }
    public void setmission(string mission)
    {
        selectedmission = mission;
    }
    public void StartGame()
    {
        timerRunning = true;
        StartCoroutine(stallwarning());
        StartCoroutine(TimerCoroutine());
        //wait for scene to load
        StartCoroutine(waitforsecondsandlOad());
      
    }
    void checkPlayeralive()
    {         
        if (player.activeSelf==false && ingame && gameover!=true )
        {
            GameOver(false);
            gameover = true;
        }
        //else
        //    checkstall(player);
    }
    void checkstall(GameObject player)
    {
      
        if(player != null && ingame && gameover != true)
        {
            
            Debug.Log("PLayer Velocity: " + player.GetComponent<Rigidbody2D>().velocity.magnitude);
            if(player.GetComponent<Rigidbody2D>().velocity.magnitude<30)
            {
                warning = true;
            }
            if(player.GetComponent<Rigidbody2D>().velocity.magnitude < 10)
            {
                  GameOver(false);
                gameover = true;
            }
        }
    }
    IEnumerator stallwarning()
    {
        //show the stall warning
        while(warning)
        {
            stall.alpha = 1;
            yield return new WaitForSeconds(0.5f);
            stall.alpha = 0;
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public IEnumerator waitforsecondsandlOad()
    {
        LeanTween.alphaCanvas(panel.GetComponent<CanvasGroup>(), 1, 2).setOnComplete(() => SceneManager.LoadScene(selectedmission));
        
        yield return new WaitForSeconds(2);
        Debug.Log("Loadout SIZE " + loadout.Count);
        Debug.Log("Scene Loaded");
        //GameObject playerobj = Instantiate(player, new Vector3(-7507, 525, 121), Quaternion.identity);
        //GameObject playerobj = FindObjectOfType<NewControl>().gameObject;   
        player= FindObjectOfType<NewControl>().gameObject;   
        SetPlayerLoadout(loadout, player);
        ingame = true;
        Debug.Log("Player Loadout set");
        Debug.Log("player "+ player.name); 
        yield return new WaitForSeconds(2);
        player.GetComponentInChildren<WeaponsManager>().Loadweaponson();


    }
    public void GameOver(bool win)
    {
        ingame = false;
        GetComponentInChildren<CanvasGroup>().blocksRaycasts=true;
        timerRunning = false;
        if (win)
        {
            Win = true;
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
        Debug.Log("Current Scene: " + currentSceneName + " time: " + timer);
        if (bestMissionTimes.ContainsKey(currentSceneName))
        {
            Debug.Log("Save contains Best Time: " + bestMissionTimes[currentSceneName]);
            if (timer < bestMissionTimes[currentSceneName] || bestMissionTimes[currentSceneName]==0)
            {
                bestMissionTimes[currentSceneName] = timer;
                SaveBestTime(currentSceneName,timer);
            }
        }
        else
        {
            Debug.Log("Save does not contain Best Time: " + currentSceneName);
            // Scene name not found, save the time
            bestMissionTimes[currentSceneName] = timer;
            SaveBestTime(currentSceneName,timer);
        }
    }
    public void Refly()
    {
        //reload the scene
       

        showscreen = 2;
        fadeOut(End);
        StartCoroutine(transitiontoMainMenue());
        
        //StartGame();
    }
    public void mainmenu()
    {
        showscreen = 0;
        Debug.Log("Main Menu");
        StartCoroutine(transitiontoMainMenue());
    }
    public IEnumerator transitiontoMainMenue()
    {
        ingame = false;
        Win = false;
        gameover = false;
        Time.timeScale = 1;
        fadeOut(End);
        yield return new WaitForSeconds(2);
        Success.SetActive(false);
        lose.SetActive(false);
        SceneManager.LoadScene("MainMenu");
        
        yield return new WaitForSeconds(0.5f);
        panel = GameObject.Find("Panel");
        FindAnyObjectByType<MenuController>().GetComponent<MenuController>().ShowScreen(showscreen);
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
                Debug.Log("Loaded Best Time: " + parts[0] + " - " + parts[1]);
            }
        }
        return result;
    }
}
