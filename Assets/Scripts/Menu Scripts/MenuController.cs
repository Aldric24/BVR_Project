using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
   

    public List<GameObject> menuScreens;
    [SerializeField]SortieScreen sortieScreen;

    // Ensure this GameObject persists between scenes
    

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
