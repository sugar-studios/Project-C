using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LedgeGrabber : MonoBehaviour
{
    [SerializeField] LayerMask _Ledge;
    [SerializeField] GameObject _Parent;
    [SerializeField] Rigidbody2D _RB;

    private GameObject _CurrentLedge;
    private PlayerControls _Controls;
    private bool LedgeGrab;
    private float prevGravity;
    private bool LedgeLock;
    private int direction;
    private Vector2 _RawInputVector;

    private void Awake()
    {
        _Controls = new PlayerControls();
        _Controls.Gameplay.Jump.performed += ctx => LedgeJump();
        _Controls.Gameplay.Move.performed += ctx => _RawInputVector = ctx.ReadValue<Vector2>();
        _Controls.Gameplay.Move.canceled += ctx => _RawInputVector = Vector2.zero;
        _CurrentLedge = null;
        LedgeLock = true;
        prevGravity = _RB.gravityScale;
    }

    private void OnEnable()
    {
        _Controls.Gameplay.Enable();
    }


    private void OnDisable()
    {
        _Controls.Gameplay.Disable();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_Ledge.value & (1 << collision.gameObject.layer)) != 0)
        {
            _CurrentLedge = collision.transform.parent.gameObject;
            Debug.Log(_CurrentLedge);
            Debug.Log(_CurrentLedge.transform.GetChild(1));
            GrabLedge();
        }
    }

    void LedgeJump()
    {
        if (LedgeGrab) { 
        _RB.isKinematic = false;
        _RB.velocity = new Vector2(_RB.velocity.x, 20);
        OffLedge();
        }
    }

    void LedgeClimb()
    {
        if (LedgeGrab)
        {
            Debug.Log(_Parent.transform.position);
            try
            {
                _Parent.transform.position = new Vector2(_CurrentLedge.transform.GetChild(1).gameObject.transform.position.x, _CurrentLedge.transform.GetChild(1).gameObject.transform.position.y + _Parent.GetComponent<Collider2D>().bounds.size.y / 2);
            }
            catch 
            {
                OffLedge();
            }
            Debug.Log(_Parent.transform.position);
            OffLedge();
        }
    }

    private Vector2 targetPosition; // Add this field to your class

    void GrabLedge()
    {
        direction = _Parent.GetComponent<newPlayerMovement>()._IsFacingRight ? -1 : 1;
        _RB.velocity = Vector2.zero;
        _RB.gravityScale = 0;
        // Calculate the target position without immediately setting _Parent.transform.position
        targetPosition = new Vector2(_CurrentLedge.transform.GetChild(2).transform.gameObject.transform.position.x + (_Parent.GetComponent<Collider2D>().bounds.size.x / 2 * direction), _CurrentLedge.transform.GetChild(2).gameObject.transform.position.y);
        _Parent.GetComponent<newPlayerMovement>().enabled = false;
        LedgeGrab = true;
    }


    private void OffLedge()
    {
        if (LedgeGrab)
        {
            _CurrentLedge = null;
            _RB.gravityScale = prevGravity;
            _Parent.GetComponent<newPlayerMovement>().enabled = true;
            LedgeLock = true;
            LedgeGrab = false;
        }
    }

    private void Update()
    {
        if (LedgeGrab)
        {
            if (LedgeLock)
            {
                if (Mathf.Abs(_RawInputVector.x) == 0)
                {
                    LedgeLock = false;
                }
            }
            else
            {
                if (Mathf.Abs(_RawInputVector.x) >= .34)
                {
                    LedgeClimb();
                }
            }

            // Smoothly move _Parent to the target position
            if (!_Parent.transform.position.Equals(targetPosition))
            {
                _Parent.transform.position = Vector2.Lerp(_Parent.transform.position, targetPosition, .1f); // Adjust the multiplier for speed
            }
        }
    }

}
