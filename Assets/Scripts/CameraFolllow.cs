using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFolllow : MonoBehaviour
{

    private Transform player;
    private Vector3 tempPos;
    public float minX = -20;
    public float maxX = 20;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        tempPos = transform.position;
        tempPos.x = player.position.x;
        tempPos.y = (player.position.y + 3);
        // Setting bounds for camera.
        if (tempPos.x < minX)
        {
            tempPos.x = minX;
        }
        if (tempPos.x > maxX)
        {
            tempPos.x = maxX;
        }
        transform.position = tempPos;
    }
} // class
