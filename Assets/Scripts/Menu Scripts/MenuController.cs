using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
   

    public List<GameObject> menuScreens;

    // Ensure this GameObject persists between scenes
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ShowScreen(int screenIndex)
    {
        for (int i = 0; i < menuScreens.Count; i++)
        {
            menuScreens[i].SetActive(i == screenIndex);
        }
    }
    
}
