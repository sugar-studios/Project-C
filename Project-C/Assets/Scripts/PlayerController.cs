using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Vector2 PlayerInputVector;
    public Vector2 PlayerMovementVector;
    public string State;
    public bool StanderizeMovement = false;

    private Rigidbody2D _RB;
    private bool _IsFacingRight = true;
    private bool _IsDashing = false;
    private bool _FastFalling = false;
    private float _Horizontal;
    private float _DoubleJumpCount;
    private float _CoyoteTime = .2f;
    private float _CoyoteTimeCounter;

    private Vector2 _MovementInput = Vector2.zero;
    private bool _Jumped = false;

    [SerializeField]
    private float _JumpHeight = 16f;
    [SerializeField]
    private float _FastFallSpeed = 0.1f;
    [SerializeField]
    private float _NGroundSpeedCap = 7f;
    [SerializeField]
    private float _DGroundSpeedCap = 11f;
    [SerializeField]
    private float _NAirSpeedCap = 9.5f;
    [SerializeField]
    private float _NGroundSpeed = 1750f;
    [SerializeField]
    private float _DGroundSpeed = 2250f;
    [SerializeField]
    private float _NAirSpeed = 2000f;
    [SerializeField]
    private float _MidAirJumpHeight = 16f;
    [SerializeField]
    private Transform _GroundCheck;
    [SerializeField]
    private LayerMask _GroundLayer;
    [SerializeField]
    private LayerMask _PlatformLayer;
    [SerializeField]
    private float _MaxDoubleJump;

    private void Start()
    {
        _RB = gameObject.GetComponent<Rigidbody2D>();
        State = "Free";
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _MovementInput = context.ReadValue<Vector2>();  
    }

    public void OnJump(InputAction.CallbackContext context)
    { 
        _Jumped = context.action.triggered;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        _IsDashing = context.action.triggered;
    }

    public void OnFastFall(InputAction.CallbackContext context)
    {
        if (!IsGrounded())
        {
            _FastFalling = true;
        }
    }

    void Update()
    {
        Debug.Log(_RB.velocity);

        PlayerInputVector = new Vector2(StandardizeMoveValues(_MovementInput.x), StandardizeMoveValues(_MovementInput.y));
        PlayerMovementVector = PlayerInputVector = StanderizeMovement ? PlayerInputVector : _MovementInput;

        if (IsGrounded())
        {
            if (State == "Free")
            {
                _Horizontal = PlayerMovementVector.x;
            }
            else
            {
                _Horizontal = 0;
            }
        }
        else
        {
            if (State == "Free")
            {
                _Horizontal = PlayerMovementVector.x;
            }
            else
            {
                _Horizontal = PlayerMovementVector.x/3;
            }
        }



        if (_Jumped && (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0))
        {
            if (State == "Free")
            {
                if (!(_CoyoteTimeCounter > 0))
                {
                    _FastFalling = false;
                    _RB.velocity = new Vector2(_RB.velocity.x/3, _MidAirJumpHeight);
                    _DoubleJumpCount--; _Jumped = false;
                } 
                else
                {
                    _FastFalling = false;
                    _RB.velocity = new Vector2(_RB.velocity.x/3, _JumpHeight);
                    _Jumped = false;
                }
                _CoyoteTimeCounter = 0;
            }
        }

        if (IsGrounded())
        {
            _DoubleJumpCount = _MaxDoubleJump;
            Flip();
            _CoyoteTimeCounter = _CoyoteTime;
            _FastFalling = false;
        }
        else
        {
            if (_CoyoteTimeCounter > 0)
            {
                _CoyoteTimeCounter -= Time.deltaTime;
            }
        }

    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            if (_IsDashing)
            {
                Debug.Log(_Horizontal);
                Move(_Horizontal, _DGroundSpeed, _DGroundSpeedCap, true);
            }
            else
            {
                Debug.Log(_Horizontal);
                Move(_Horizontal, _NGroundSpeed, _NGroundSpeedCap, true);
            }
        }
        else
        {
            Move(_Horizontal, _NAirSpeed, _NAirSpeedCap, true);

            if (_FastFalling)
            {
                _RB.velocity = new Vector2(_RB.velocity.x, _RB.velocity.y - _FastFallSpeed);
            }
        }
    }


    /*Basically it takes in a horizantal, accelereation, max speed, force mode, and capping values. 
     * horizantal is an float that represent a postive or negitive number. Postive means right, negative means left. It aslso indaiacates the movement multiplier 1 = full speed, 0 = no speed.
     * acceleration is how fast the player will move forward (this is NOT max speed), it is how fast the player will reach max speed, higher number = more responisve.
     * max speed is the cap on how fast the player will move once full acceleration is reached. This is the actual player movement speed. If you don't put a max speed, it will not be capped
     * impulse is if you want instanious force or applied force. Impule makes going in the opposite direction faster and reponsive, but applied forece has more force behind it. 
     */
    private void Move(float horizantal, float acc, float max = 0, bool impulse = false)
    {
        if (impulse)
        {
            _RB.AddForce(new Vector2(horizantal * acc * Time.deltaTime, 0), ForceMode2D.Impulse);
        }
        else
        {
            _RB.AddForce(new Vector2(horizantal * acc * Time.deltaTime, 0));
        }
        if (max > 0)
        {
            _RB.velocity = new Vector2(Mathf.Clamp(_RB.velocity.x, -max, max), _RB.velocity.y);
        }
    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_GroundCheck.position, 0.2f, _GroundLayer);
    }


    private float StandardizeMoveValues(float num)
    {
        num = (Mathf.Abs(num) >= 0.34f) ? 1 * Mathf.Sign(num) : 0;

        return num;
    }

    private void Flip()
    {
        if (_IsFacingRight && _Horizontal < 0f || !_IsFacingRight && _Horizontal > 0f)
        {
            _IsFacingRight = !_IsFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}


