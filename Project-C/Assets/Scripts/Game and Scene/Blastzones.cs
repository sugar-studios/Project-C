using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blastzones : MonoBehaviour
{
    public LayerMask Player1;

    public GameManager gameManager;

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((Player1.value & (1 << other.gameObject.layer)) != 0)
        {
            gameManager.GameOver();
            other.gameObject.SetActive(false);
        }
    }
}
