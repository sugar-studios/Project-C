using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{

    public GameObject[] Players;

    // Update is called once per frame
    void Update()
    {
        MoveCameraPosition();
    }

    void MoveCameraPosition()
    {
        Vector3 currentPosition = transform.position;

        float newCameraPosition = 0;

        for (int i = 0; i < Players.Length; i++)
        {
            newCameraPosition += Players[i].transform.position.x;
        }

        newCameraPosition /= Players.Length;

        transform.position = new Vector3(newCameraPosition, transform.position.y, transform.position.z);
    }
}
