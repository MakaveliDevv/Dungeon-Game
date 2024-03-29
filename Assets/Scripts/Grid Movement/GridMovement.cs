using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public bool isMoving;
    Vector3 initialPos, targetPos;
    private Vector2 curPosition, lastPosition;
    public float timeToMove = .2f;
    private Rigidbody2D rb;

    [SerializeField] private RegionData region;
    // [SerializeField] private GameManager gameManag;


    void Awake()
    {
        // gameManag = GameObject.FindFirstObjectByType<GameManager>();
    } 

    void Start()
    {
        // if(GameManager.instance.NextSpawnPoint != "") 
        // {
        //     GameObject spawnPoint = GameObject.Find(GameManager.instance.NextSpawnPoint);
        //     transform.position = spawnPoint.transform.position;

        //     GameManager.instance.NextSpawnPoint = "";

        // } else 
        
        rb = GetComponent<Rigidbody2D>();
        if(GameManager.instance.lastHeroPosition != Vector2.zero) 
        {
            transform.position = GameManager.instance.lastHeroPosition;
            GameManager.instance.lastHeroPosition = Vector2.zero;
        }
    }

    void FixedUpdate() 
    {
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

    void Update()
    {
        if(Input.GetKey(KeyCode.W) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.up));
        }

        if(Input.GetKey(KeyCode.A) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.left));
        }

        if(Input.GetKey(KeyCode.S) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.down));
        }

        if(Input.GetKey(KeyCode.D) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.right));
        }
    }

    private IEnumerator MovePlayer(Vector3 _direction) 
    {
        isMoving = true;

        float elapsedTime = 0f;

        initialPos = transform.position;
        targetPos = initialPos + _direction;

        while(elapsedTime < timeToMove) 
        {
            rb.MovePosition(Vector3.Lerp(initialPos, targetPos, (elapsedTime / timeToMove)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);

        isMoving = false;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("EncounterZone")) 
        {
            Debug.Log("Zone Encounterd!");
            region = other.gameObject.GetComponent<RegionData>();
            GameManager.instance.curRegion = region;

            // Add the current enemy to the encounterdEnemy list in GamaManager
            // gameManag.encounterdEnemy.Add(other.gameObject);
            GameManager.instance.encounterdEnemy.Add(other.gameObject.name);

            if(GameManager.instance.battleWon) 
            {
                foreach(string enemyName in GameManager.instance.encounterdEnemy)
                {
                    // Find the GameObject with the specified name
                    GameObject enemyObject = GameObject.Find(enemyName);
                    
                    // Check if the enemyObject exists and then destroy it
                    if(enemyObject != null)
                    {
                        Destroy(enemyObject);
                        Debug.Log("Destroyed enemy: " + enemyName);
                    }
                }
            }   
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
