using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public moveCamera playerList;
    private static int playerCount = 0;  // Static counter to keep track of the number of players
    public GameObject Spawn1;
    public GameObject Spawn2;

    private void OnEnable()
    {
        // Find the moveCamera script attached to any active object in the scene
        playerList = FindObjectOfType<moveCamera>();

        if (playerList != null)
        {
            // Add this game object to the camera's Players list
            playerList.players.Add(this.gameObject);

            // Increment player count and assign tag
            playerCount++;
            if (playerCount <= 4)
            {
                AssignTagRecursively(this.gameObject, "Player" + playerCount);
                if (this.gameObject.tag == "Player1")
                {
                    Spawn1 = GameObject.FindWithTag("Player3");
                    transform.position = Spawn1.transform.position;
                }
                else
                {
                    Spawn2 = GameObject.FindWithTag("Player4");
                    transform.position = Spawn2.transform.position;
                }
            }
            else
            {
                Debug.LogError("Maximum player limit reached");
            }
        }
        else
        {
            Debug.LogError("moveCamera script not found in the scene.");
        }
    }


    private void OnDisable()
    {
        // Decrement player count when a player object is disabled
        playerCount--;
    }

    // Helper function to assign tag recursively to all child objects
    private void AssignTagRecursively(GameObject obj, string tag)
    {
        obj.tag = tag;
        foreach (Transform child in obj.transform)
        {
            AssignTagRecursively(child.gameObject, tag);
        }
    }
}
