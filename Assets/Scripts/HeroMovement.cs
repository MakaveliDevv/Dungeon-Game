using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Vector2 curPosition, lastPosition;


    void Start()
    {
        transform.position = GameManager.instance.nextHeroPosition;
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new(moveX, moveY);
        GetComponent<Rigidbody2D>().velocity = moveSpeed * movement; 

        curPosition = transform.position;

        if(curPosition == lastPosition) 
        {
            GameManager.instance.isWalking = false;

        } else 
        {
            GameManager.instance.isWalking = true;
        }

        lastPosition = curPosition;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("EnterTown") || other.CompareTag("LeaveTown")) 
        {
            CollisionHandler colHandler = other.GetComponent<CollisionHandler>();
            GameManager.instance.nextHeroPosition = colHandler.spawnPoint.transform.position; // In new scene this info gets passed into the start
            GameManager.instance.SceneToLoad = colHandler.SceneName;
            GameManager.instance.LoadNextScene();
        }

        if(other.CompareTag("Region1")) 
        {
            GameManager.instance.curRegion = 0;
        }

        if(other.CompareTag("Region2")) 
        {
            GameManager.instance.curRegion = 1;
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.CompareTag("Region1") || other.CompareTag("Region2")) 
        {
            GameManager.instance.canGetEncounter = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Region1") || other.CompareTag("Region2")) 
        {
            GameManager.instance.canGetEncounter = false;
        }
    }
}
