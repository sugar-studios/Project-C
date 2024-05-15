using System.Collections;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    private PlayerController _Player;

    [SerializeField] private BoxCollider2D _PlayerCollider;
    [SerializeField] private AudioSource _PlatformFallSound;

    private void Start()
    {
        _Player = transform.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_Player.PlayerInputVector.y == -1)
        {
            Debug.Log("Fall");
            Debug.Log(currentOneWayPlatform);
            if (currentOneWayPlatform != null)
            {
                Debug.Log("GO GO GO");
                //_PlatformFallSound.Play();
                StartCoroutine(DisableCollision(currentOneWayPlatform.GetComponent<BoxCollider2D>()));
              
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

    private IEnumerator DisableCollision(BoxCollider2D platformCollider)
    {

        Physics2D.IgnoreCollision(_PlayerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(_PlayerCollider, platformCollider, false);
    }
}