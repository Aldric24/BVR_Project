using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Button button;
    void Start()
    {
        Time.timeScale = 0;
    }
    // Start is called before the first frame update
    public void onSart()
    {
        Time.timeScale = 1;
       
        gameObject.SetActive(false);
    }
}
