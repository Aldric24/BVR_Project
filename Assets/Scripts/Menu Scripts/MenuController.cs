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

    // Ensure this GameObject persists between scenes
    private void Start()
    {
        LeanTween.alphaCanvas(Panel.GetComponent<CanvasGroup>(), 0, 2);
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
