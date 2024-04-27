using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject enemy;
    [SerializeField] public GameObject WaypointSystem;
    [SerializeField] public GameObject[] Weapons;
    public Loadoutscreen Loadoutscreen;
    private Dictionary<int, Weapon> loadout;
    public GameObject panel;
    
    
    void Awake()
    {
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
    public void StartGame()
    {
        
        
        //wait for scene to load
        StartCoroutine(waitforsecondsandlOad());
        

    }
    public IEnumerator waitforsecondsandlOad()
    {
        LeanTween.alphaCanvas(panel.GetComponent<CanvasGroup>(), 1, 2).setOnComplete(() => SceneManager.LoadScene("LV1"));
        
        yield return new WaitForSeconds(2);
        GameObject playerobj = Instantiate(player, new Vector3(1661, 219, 60), Quaternion.identity);
        SetPlayerLoadout(loadout, playerobj);
        
        
    }
    public IEnumerator transitiontoMainMenue()
    {
        panel.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenu");
    }
}
