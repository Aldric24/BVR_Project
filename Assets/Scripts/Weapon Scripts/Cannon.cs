using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : Weapon
{
     
    [SerializeField] int Ammo = 360;
    bool cannonFiring = true;
    float cannonFiringTimer = 0;    
    public float cannonFireRate = 1;
    public float cannonSpread = 5;
    public GameObject bulletPrefab;
    public Transform cannonSpawnPoint;
    [SerializeField] GameObject player;
    [SerializeField] Button fireButton;
    public bool cannonequipped = false;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] AudioSource gun;
    // Start is called before the first frame update
    void Start() // Or Awake()
    {
        ammoText.text = Ammo.ToString();
        weaponName = "Cannon";
        type  = "Cannon";
    }
    

    public void fire()
    {
        var spread = Random.insideUnitCircle * cannonSpread;
        var bullet = Instantiate(bulletPrefab, cannonSpawnPoint.position, cannonSpawnPoint.rotation);
        //var bulletGO = Instantiate(bulletPrefab, cannonSpawnPoint.position, cannonSpawnPoint.rotation * Quaternion.Euler(spread.x, spread.y, 0));
        bullet.GetComponent<Bullet>().owner = player;




    }
    private IEnumerator FireBurstCoroutine()
    {
        while (cannonFiring && Ammo>0 && cannonequipped==true) // While the fire button is held down
        {
            fire(); // Fire a single shot
            Ammo--; // Decrement ammo count
            ammoText.text = Ammo.ToString();
            yield return new WaitForSeconds(1f / cannonFireRate); // Wait based on fire rate     
        }
    }
    public void StartFiring()
    {
        if(cannonequipped)
        {
            cannonFiring = true;
            StartCoroutine(FireBurstCoroutine());
        }
        
    }

    public void StopFiring()
    {
        cannonFiring = false;
    }
    
}
