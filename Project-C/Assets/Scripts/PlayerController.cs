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
    private float _NGroundSpeed = 8f;
    [SerializeField]
    private float _DGroundSpeed = 11f;
    [SerializeField]
    private float _NAirSpeed = 2300f;
    [SerializeField]
    private float _NAirSpeedCap = 9.5f;
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
                _RB.velocity = new Vector2(_Horizontal * _DGroundSpeed, _RB.velocity.y);
            }
            else
            {
                _RB.velocity = new Vector2(_Horizontal * _NGroundSpeed, _RB.velocity.y);
            }
        }
        else
        {
            _RB.velocity = new Vector2(_Horizontal * _NGroundSpeed, _RB.velocity.y);
            if (_FastFalling)
            {
                _RB.velocity = new Vector2(_RB.velocity.x, _RB.velocity.y - _FastFallSpeed);
            }
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


