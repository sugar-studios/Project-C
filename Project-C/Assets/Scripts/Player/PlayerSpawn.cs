using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public moveCamera playerList;
    private void OnEnable()
    {
        // Find the moveCamera script attached to any active object in the scene
        moveCamera playerList = FindObjectOfType<moveCamera>();

        if (playerList != null)
        {
            // Add this game object to the camera's Players list
            playerList.Players.Add(this.gameObject);
        }
        else
        {
            Debug.LogError("moveCamera script not found in the scene.");
        }
    }
}
