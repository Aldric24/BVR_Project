using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTruck : WeaponsManager
{
    [SerializeField] private GameObject playerTarget;
    [SerializeField] private List<GameObject> missilePrefabs;
    [SerializeField] private Transform missileSpawnPoint;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float firingCooldown = 2f;
    [SerializeField] private LayerMask playerLayer;

    private float timeSinceLastShot = 0f;

    void Update()
    {
        if (!playerTarget)
        {
            playerTarget = FindPlayer();
        }

        if (playerTarget && CanSeePlayer())
        {
            timeSinceLastShot += Time.deltaTime;
            target = playerTarget;
            if (timeSinceLastShot >= firingCooldown)
            {
                fire();
                timeSinceLastShot = 0f;
            }
        }
        else
        {
            playerTarget = null;
            target = playerTarget;
        }
    }

    GameObject FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        return playerCollider ? playerCollider.gameObject : null;
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (playerTarget.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, playerLayer);
        return hit.collider != null;
    }

    void fire()
    {
        int randomIndex = Random.Range(0, missilePrefabs.Count);
        GameObject missileprefab = missilePrefabs[randomIndex];
        GameObject missile = Instantiate(missileprefab, missileSpawnPoint.position, missileSpawnPoint.rotation);
        if (missile.GetComponent<Weapon>().type == "Fox3")
        {
           
            missile.GetComponent<Fox3Script>().enabled=true; 
            missile.GetComponent<Fox3Script>().fire(this); 

        }
        else if (missile.GetComponent<Weapon>().type == "Fox1")
        {
            missile.GetComponent<Fox3Script>().enabled = true;
            missile.GetComponent<Fox1Script>().fire(this);

        }
        else if (missile.GetComponent<Weapon>().type == "Fox2")
        {
            missile.GetComponent<Fox3Script>().enabled = true;
            missile.GetComponent<Fox2Script>().fire(this);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}