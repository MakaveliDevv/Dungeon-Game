using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Vector2 curPosition, lastPosition;


    void Start()
    {
        if(GameManager.instance.NextSpawnPoint != "") 
        {
            GameObject spawnPoint = GameObject.Find(GameManager.instance.NextSpawnPoint);
            transform.position = spawnPoint.transform.position;

            GameManager.instance.NextSpawnPoint = "";

        } else if(GameManager.instance.lastHeroPosition != Vector2.zero) 
        {
            transform.position = GameManager.instance.lastHeroPosition;
            GameManager.instance.lastHeroPosition = Vector2.zero;
        }
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
        if(other.CompareTag("Teleporter")) 
        {
            CollisionHandler colHandler = other.GetComponent<CollisionHandler>();
            GameManager.instance.NextSpawnPoint = colHandler.SpawnPointName; 
            GameManager.instance.SceneToLoad = colHandler.SceneName;
            GameManager.instance.LoadNextScene();
        }

        if(other.CompareTag("EncounterZone")) 
        {
            RegionData region = other.gameObject.GetComponent<RegionData>();
            GameManager.instance.curRegion = region;
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.CompareTag("EncounterZone")) 
        {
            GameManager.instance.canGetEncounter = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("EncounterZone")) 
        {
            GameManager.instance.canGetEncounter = false;
        }
    }
}
