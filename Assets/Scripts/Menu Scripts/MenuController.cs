using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
   

    public List<GameObject> menuScreens;
    [SerializeField]SortieScreen sortieScreen;
    [SerializeField] GameObject loadout;
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
    private void FixedUpdate()
    {
        if(sortieScreen.SelectedMission!=null)
        {
            loadout.gameObject.GetComponent<Button>().interactable=true;
        }
        else
        {
            loadout.gameObject.GetComponent<Button>().interactable=false;
        }
    }
}
