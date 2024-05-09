using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardPoint : MonoBehaviour
{
    // Start is called before the first frame update
   
    [SerializeField]Transform hardpoint;
    public  bool MissileFired = false;
    public GameObject missile;
    public string missiletype;

    private void Start()
    {
        missiletype = missile.GetComponent<Weapon>().type;
    }
    public void AttachMissile(GameObject Missile)
    {
        missile = Instantiate(Missile, hardpoint.position, hardpoint.rotation);
        missile.transform.parent = hardpoint;
    }
   
    public void Fire(WeaponsManager wp)
    {
        if(missile.GetComponent<Weapon>().type== "Fox3")
        {
            MissileFired = true;
            missile.GetComponent<Fox3Script>().fire(wp); ;
          
        }
        else if(missile.GetComponent<Weapon>().type == "Fox1")
        {
            MissileFired = true;
            missile.GetComponent<Fox1Script>().fire(wp);

        }
        else if (missile.GetComponent<Weapon>().type == "Fox2")
        {
            MissileFired = true;
            missile.GetComponent<Fox2Script>().fire(wp);

        }


    }
    public void EnableMissile()
    {
        if (missile.GetComponent<Weapon>().type == "Fox3")
        {   
            missile.GetComponent<Fox3Script>().enabled = true;
        }
        else if (missile.GetComponent<Weapon>().type == "Fox1")
        {

            missile.GetComponent<Fox1Script>().enabled = true;

        }
        else if (missile.GetComponent<Weapon>().type == "Fox2")
        {
            missile.GetComponent<AudioSource>().enabled = true;
            missile.GetComponent<Fox2Script>().enabled=true;
        }
    }
    public void DisableMissile()
    {
        if (missile.GetComponent<Weapon>().type == "Fox3")
        {
            missile.GetComponent<Fox3Script>().enabled = false;
        }
        else if (missile.GetComponent<Weapon>().type == "Fox1")
        {
            missile.GetComponent<Fox1Script>().enabled = false;
        }
        else if (missile.GetComponent<Weapon>().type == "Fox2")
        {
            missile.GetComponent<AudioSource>().enabled = false;
            missile.GetComponent<Fox2Script>().enabled = false;
        }
    }   
    


}
