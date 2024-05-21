using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    // List to store hardpoint references
    public List<HardPoint> hardpoints = new List<HardPoint>();

    // Currently equipped cannon object (if applicable)
    public Cannon equippedCannon;
    // Index for the currently selected weapon (0-based)
    [SerializeField] private int currentWeaponIndex = 0;
    public GameObject target;
    [SerializeField] private SweepRotation S;
    [SerializeField] TextMeshProUGUI Weapon;
    int missilecount = 0;
    [SerializeField] List<string> availableWeaponTypes = new List<string>(); // To track found weapon types
    private int currentWeaponTypeIndex = 0;
    // All available missiles
    public string currentMissileType = null; // Currently equipped missile type (or null)
    private int currentMissileIndex = 0; // Index within the current missile type

    private int currentListIndex = 0; // Index of the currently equipped list
    private List<HardPoint> currentMissileList; // Reference to the active list
    [SerializeField] private int flareCount;
    [SerializeField] private float lastFlareDeployTime;
    [SerializeField] private int cooldown;
    [SerializeField] private int flaresPerDeployment;
    [SerializeField] GameObject flarePrefab;
    [SerializeField] private int ChaffCount;
    [SerializeField] private float lastchaffdeploytime;
    [SerializeField] private int ccooldown;
    [SerializeField] private int chaffperdeployment;
    [SerializeField] GameObject chaffprefab;
    private Vector2 dispersion;
    [SerializeField] private bool AIControl;
    [SerializeField] bool isAIMissiletruck = false;
    [SerializeField] TMPro.TextMeshProUGUI FlareCount;
    [SerializeField] TMPro.TextMeshProUGUI ChaffCountText;
    [SerializeField]List<HardPoint> Fox1=new List<HardPoint>();
    [SerializeField]List<HardPoint> Fox2=new List<HardPoint>();
    [SerializeField]List<HardPoint> Fox3=new List<HardPoint>();
    void Start()
    {
       
        if (AIControl)
        {
            Tempmattach();
        }
        if(FlareCount!=null && ChaffCountText!=null)
        {
            FlareCount.text = flareCount.ToString();
            ChaffCountText.text = ChaffCount.ToString();
        }
           
        //Loadweaponson();
        //Tempmattach();
       
        
        //SwitchMissile();

    }
    void FixedUpdate()
    {


        if (isAIMissiletruck == false)
        {
            //if (availableWeaponTypes.Count > 0 && AIControl == false)
            //{
            //    // Get the currently selected weapon type (if any)
            //    string weaponTypeToDisplay = currentWeaponTypeIndex < availableWeaponTypes.Count
            //                                    ? availableWeaponTypes[currentWeaponTypeIndex]
            //                                    : "None";
            //    Weapon.text = weaponTypeToDisplay + ": " + missilecount;
            //}
            UpdateWeaponUI();

            target = S.LocekdTarget;
        }


    }
    public void Loadweaponson()
    {
        foreach (HardPoint hardPoint in hardpoints)
        {
            //if (hardPoint.missile != null)
            //{
                missilecount++;
                if (hardPoint.missiletype == "Fox1")
                {
                    Fox1.Add(hardPoint);
                    if (!availableWeaponTypes.Contains("Fox1"))
                    {
                        availableWeaponTypes.Add("Fox1");
                    }
                }
                else if (hardPoint.missiletype == "Fox2")
                {
                    Fox2.Add(hardPoint);
                    if (!availableWeaponTypes.Contains("Fox2"))
                    {
                        availableWeaponTypes.Add("Fox2");
                    }
                }
                else if (hardPoint.missiletype == "Fox3")
                {
                    Fox3.Add(hardPoint);
                    if (!availableWeaponTypes.Contains("Fox3"))
                    {
                        availableWeaponTypes.Add("Fox3");
                    }
                }
                
            //}
        }
        currentMissileType = availableWeaponTypes[0];
    }
    public void Tempmattach()
    {
        //availableWeaponTypes.Clear();
        foreach (HardPoint hardpoint in hardpoints)
        {

            hardpoint.AttachMissile(hardpoint.missile);
           

        }
    }
    public void FireMissile()
    {
        if (currentMissileType != null)
        {
            FireCurrentMissile();
        }
    }
    int i = 0;
    public void SwitchMissile() // Renamed for clarity
    {
       
        
        if(availableWeaponTypes.Count!=0)
        {
            i = (i + 1)% availableWeaponTypes.Count;
            Debug.Log("index " + i);
            currentMissileType = availableWeaponTypes[i];
        }
       
        DisableAllMissileScripts();
        //if()
        if(currentMissileType=="Fox1" && Fox1.Count>0)
        {
            currentMissileType = "Fox1";
            currentMissileList = Fox1;
        }
        else if(currentMissileType== "Fox2" && Fox2.Count>0)
        {
            Fox2[0].EnableMissile();
            currentMissileType = "Fox2";
            currentMissileList = Fox2;
        }
        else if(currentMissileType == "Fox3" && Fox3.Count>0)
        {
            currentMissileType = "Fox3";
            currentMissileList = Fox3;
        }
        if (AIControl == false)
        {
            UpdateWeaponUI();
        }

    }

    private void FireCurrentMissile()
    {
        if(currentMissileType=="Fox1" && Fox1.Count>0)
        {
            Fox1[0].EnableMissile();
            Fox1[0].Fire(this);
            Fox1.RemoveAt(0);
            
        }
        else if(currentMissileType=="Fox2" && Fox2.Count > 0)
        {
            
            Fox2[0].Fire(this);
            Fox2.RemoveAt(0);
            if(Fox2.Count>0)
            {
                Fox2[0].EnableMissile();
            }

        }
        else if(currentMissileType=="Fox3" && Fox3.Count > 0)
        {
            Fox3[0].EnableMissile();
            Fox3[0].Fire(this);
            Fox3.RemoveAt(0);
        }
        
        else
        {
            FindAnyObjectByType<HUD_Text>().Notif($"No more {currentMissileType} missiles available!");
        }
        
    }

    private void DisableAllMissileScripts()
    {
        // (Implement based on your missile script setup)
        foreach (HardPoint hardpoint in hardpoints)
        {
            if (hardpoint.missiletype != currentMissileType)
            {
                hardpoint.DisableMissile();
            }
        }
    }
    public void DeployFlares()
    {
        if (flareCount > 0 && Time.time > lastFlareDeployTime + cooldown)   
        {
            for (int i = 0; i < flaresPerDeployment; i++)
            {
                Vector3 offset = Random.insideUnitCircle * dispersion;
                GameObject flare = Instantiate(flarePrefab, transform.position + offset, transform.rotation);
                flare.gameObject.tag = gameObject.tag;
            }

            flareCount--;
            FlareCount.text = flareCount.ToString();
            lastFlareDeployTime = Time.time;
        }
    }
    public void DeployChaff()
    {
        if (ChaffCount > 0 && Time.time > lastchaffdeploytime + cooldown)
        {
            for (int i = 0; i < chaffperdeployment; i++)
            {
                Vector3 ofset = Random.insideUnitCircle * dispersion;
                GameObject chaff = Instantiate(chaffprefab, transform.position + ofset, transform.rotation);
                chaff.gameObject.tag=gameObject.tag;
            }

            ChaffCount--;
            ChaffCountText.text = ChaffCount.ToString();
            lastchaffdeploytime = Time.time;
        }
    }
    private void UpdateWeaponUI()
    {
        if (currentMissileType == "Fox1" )
        {
            
            Weapon.text = currentMissileType + ": "+ Fox1.Count;
        }
        else if (currentMissileType == "Fox2" )
        {
            Weapon.text = currentMissileType + ": " + Fox2.Count;
        }
        else if (currentMissileType == "Fox3")
        {
            Weapon.text = currentMissileType + ": " + Fox3.Count;
        }

        
    }

}
