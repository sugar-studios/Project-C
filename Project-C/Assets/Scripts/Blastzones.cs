using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Blastzones : MonoBehaviour
{
    public LayerMask Player1;

    public GameManager gameManager;

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((Player1.value & (1 << other.gameObject.layer)) != 0)
        {
            other.gameObject.SetActive(false);
            gameManager.GameOver();
        }
    }
}
