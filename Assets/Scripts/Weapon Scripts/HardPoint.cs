using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardPoint : MonoBehaviour
{
    // Start is called before the first frame update
   
    [SerializeField]Transform hardpoint;
    public  bool MissileFired = false;
    public GameObject missile;
    AmraamScript missileScript;
    public void AttachMissile(GameObject Missile)
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
