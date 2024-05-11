using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    
    [SerializeField] GameObject pausescreen;
    [SerializeField] GameObject Tutorial;
    GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
       gameController= FindAnyObjectByType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void pause()
    {
        //GameObject UI = GameObject.Find("Player");
        //if(UI!=null)
        //{
        //    UI.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        //}
        Time.timeScale = 0;
        pausescreen.SetActive(true);
    }
    public void resume()
    {
        //GameObject UI = GameObject.Find("Player");
        //if (UI != null)
        //{
        //    UI.GetComponentInChildren<Canvas>().gameObject.SetActive(true);
        //}
        Time.timeScale = 1;
        pausescreen.SetActive(false);
    }
    public void loadMainMenu()
    {
        gameController.ingame = false;


        gameController.mainmenu();

        gameObject.SetActive(false);

    }
    public void Info()
    {
        Tutorial.SetActive(true);
        pausescreen.SetActive(false);
        
    }
}
