using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radarhandler : MonoBehaviour
{
    public GameObject SmolRadar;
    public GameObject BigRadar;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SmolRadar.GetComponentInChildren<Text>().text = BigRadar.GetComponentInChildren<Text>().text;
            
    }
    public void SRadarOn()
    {
        SmolRadar.SetActive(true);
        BigRadar.SetActive(false);

    }
    public void BRadarOn()
    {
        SmolRadar.SetActive(false);
        BigRadar.SetActive(true);
    }
}
