using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public bool isMoving;
    Vector3 initialPos, targetPos;
    public float timeToMove = .2f;


    void Start()
    {
        
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
            transform.position = Vector3.Lerp(initialPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }
}
