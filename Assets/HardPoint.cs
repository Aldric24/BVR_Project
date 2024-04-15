using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Missile;
    [SerializeField]Transform hardpoint;
    public  bool MissileFired = false;
    GameObject missile;
    AmraamScript missileScript;
    void Start()
    {
        
        
            missile = Instantiate(Missile, hardpoint.position, hardpoint.rotation);
            missile.transform.parent = hardpoint;
        
        
        
           
        
        
    }
    public void Fire(WeaponsManager wp)
    {
        MissileFired = true;
        missileScript = missile.GetComponent<AmraamScript>();
        missileScript.fire(wp);
    }

}
