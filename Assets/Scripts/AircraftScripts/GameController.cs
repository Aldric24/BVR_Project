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
    public void spawnplayer()
    {
        
        SceneManager.LoadScene("LV1");
        //wait for scene to load
        StartCoroutine(waitforsecondsandlOad());
        

    }
    IEnumerator waitforsecondsandlOad()
    {
        yield return new WaitForSeconds(2);
        GameObject playerobj = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);
        SetPlayerLoadout(loadout, playerobj);
        playerobj.GetComponentInChildren<NewControl>().throttleSlider.value=50;
        
    }
}
