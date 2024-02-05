using System.Collections;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    private newPlayerMovement _Player;

    [SerializeField] private BoxCollider2D _PlayerCollider;
    [SerializeField] private AudioSource _PlatformFallSound;

    private void Start()
    {
        _Player = transform.GetComponent<newPlayerMovement>();
    }

    private void Update()
    {
        if (_Player.PlayerInputVector.y == -1)
        {
            if (currentOneWayPlatform != null)
            {
                _PlatformFallSound.Play();
                StartCoroutine(DisableCollision());
            }
        }
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

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(_PlayerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(_PlayerCollider, platformCollider, false);
    }
}