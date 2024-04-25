using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MenuController : MonoBehaviour
{

    public GameObject Panel;
    public List<GameObject> menuScreens;
    [SerializeField]SortieScreen sortieScreen;
    private void Awake()
    {
        Time.timeScale = 1;
    }
    // Ensure this GameObject persists between scenes
    private void Start()
    {
       
        ShowScreen(0);
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
}
