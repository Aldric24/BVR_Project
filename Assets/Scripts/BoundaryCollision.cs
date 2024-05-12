using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCollision : MonoBehaviour
{
    GameObject player;
    [SerializeField] private Rigidbody2D playerRigidbody; // Assign the player's Rigidbody2D
    private void Start()
    {
       
    }
    private void Update()
    {
       
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = FindAnyObjectByType<NewControl>().gameObject;
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            Vector2 currentVelocity = playerRigidbody.velocity;
            playerRigidbody.velocity = new Vector2(-currentVelocity.x, -currentVelocity.y);
            playerRigidbody.transform.Rotate(0, 0, 180, Space.Self);
        }
    }
}
