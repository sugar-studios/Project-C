using UnityEngine;
using System.Collections;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    private PlayerController _Player;

    [SerializeField] private BoxCollider2D _PlayerCollider;

    private bool isCoroutineRunning = false;  // Flag to check if the coroutine is running

    private void Start()
    {
        _Player = transform.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_Player.PlayerInputVector.y == -1 && currentOneWayPlatform != null && !isCoroutineRunning)
        {
            Debug.Log("Attempting to fall through platform...");
            StartCoroutine(DisableCollision(currentOneWayPlatform.GetComponent<BoxCollider2D>()));
        }
    }

    private IEnumerator DisableCollision(BoxCollider2D platformCollider)
    {
        isCoroutineRunning = true;  // Set the flag true when coroutine starts
        Debug.Log("Disabling collision...");
        Physics2D.IgnoreCollision(_PlayerCollider, platformCollider, true);
        platformCollider.enabled = false;
        yield return new WaitForSeconds(0.25f);  // Wait for the specified time
        platformCollider.enabled = true;
        Physics2D.IgnoreCollision(_PlayerCollider, platformCollider, false);
        Debug.Log("Re-enabling collision...");
        isCoroutineRunning = false;  // Reset the flag when coroutine finishes
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }
}
