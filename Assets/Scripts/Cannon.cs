using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] int Ammo = 360;
    bool cannonFiring = true;
    float cannonFiringTimer = 0;    
    public float cannonFireRate = 1;
    public float cannonSpread = 5;
    public GameObject bulletPrefab;
    public Transform cannonSpawnPoint;
    [SerializeField] GameObject player;
    // Start is called before the first frame update
    public void fire()
    {
        var spread = Random.insideUnitCircle * cannonSpread;
        var bullet = Instantiate(bulletPrefab, cannonSpawnPoint.position, cannonSpawnPoint.rotation);
        //var bulletGO = Instantiate(bulletPrefab, cannonSpawnPoint.position, cannonSpawnPoint.rotation * Quaternion.Euler(spread.x, spread.y, 0));
        bullet.GetComponent<Bullet>().owner = player;




    }
}
