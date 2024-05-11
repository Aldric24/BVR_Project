using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MenuController : MonoBehaviour
{

    public GameObject Panel;
    public List<GameObject> menuScreens;
    [SerializeField]SortieScreen sortieScreen;
    [SerializeField] AudioMixer Mixer;
    [SerializeField][Tooltip("Music volume slider")] Slider musicSlider;
    [SerializeField][Tooltip("Voice volume slider")] Slider SFXSlider;
    [SerializeField][Tooltip("Effects volume slider")] Slider MEnuSlider;
    private void Awake()
    {
        Time.timeScale = 1;
    }
    // Ensure this GameObject persists between scenes
    private void Start()
    {
       LoadVolume();
        //ShowScreen(0);
    }

    public void ShowScreen(int screenIndex)
    {
        for (int i = 0; i < menuScreens.Count; i++)
        {
            menuScreens[i].SetActive(i == screenIndex);
        }
    }
    private void FixedUpdate()
    {
        
    }
    public void SetMusicVolume()
    {
        Mixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
    }

    /// <summary>
    /// Set voice volume and save it
    /// </summary>
    public void SetSFXVolume()
    {
        Mixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
    }

    /// <summary>
    /// Set effects volume and save it
    /// </summary>
    public void SetMenuVolume()
    {
        
        Mixer.SetFloat("Menu", Mathf.Log10(MEnuSlider.value) * 20);
        PlayerPrefs.SetFloat("MenuVolume", MEnuSlider.value);
    }
    void LoadVolume()
    {
        float music = PlayerPrefs.GetFloat("musicVolume");
        float SFX = PlayerPrefs.GetFloat("SFXVolume");
        float MENU = PlayerPrefs.GetFloat("MenuVolume");

        if (music != 0)
        {
            musicSlider.value = music;
            SetMusicVolume();
        }

        if (SFX != 0)
        {
            SFXSlider.value = SFX;
            SetSFXVolume();
        }

        if (MENU != 0)
        {
            MEnuSlider.value = MENU;
            SetMenuVolume();
        }
    }
    public void StartGame()
    {
        FindAnyObjectByType<GameController>().StartGame();
    }
    public void confirmLoadout()
    {
        FindAnyObjectByType<GameController>().confirmLoadout();
    }
    public void OnExit()
    {                   
        Application.Quit();
      
    }
}
