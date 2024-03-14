using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed;
    
    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer() 
    {
        if(player != null) 
        {
            Vector3 targetPosition = new(player.transform.position.x, player.transform.position.y, player.transform.position.z - 10f);
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        }
    }

}
