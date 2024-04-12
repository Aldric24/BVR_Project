using System.Collections;
using System.Collections.Generic;
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
    
    // Start is called before the first frame update
    public void fire()
    {
        if (cannonFiring && cannonFiringTimer == 0)
        {
            cannonFiringTimer = 60f / cannonFireRate;

            var spread = Random.insideUnitCircle * cannonSpread;

            var bulletGO = Instantiate(bulletPrefab, cannonSpawnPoint.position, cannonSpawnPoint.rotation * Quaternion.Euler(spread.x, spread.y, 0));
            
            
        }
    }
}
