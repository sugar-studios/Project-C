using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Vector2 PlayerInputVector;

    private Rigidbody2D _RB;
    private bool _IsFacingRight = true;
    private bool _IsDashing = false;
    private float _Horizontal;
    private float _DoubleJumpCount;
    private float _CoyoteTime = .2f;
    private float _CoyoteTimeCounter;
    private bool _FastFalling = false;

    private Vector2 _MovementInput = Vector2.zero;
    private bool _Jumped = false;
    private bool _FastFall = false;

    [SerializeField]
    private float _JumpHeight = 16f;
    [SerializeField]
    private float _NGroundSpeed = 8f;
    [SerializeField]
    private float _DGroundSpeed = 11f;
    [SerializeField]
    private float _NAirSpeed = 2300f;
    [SerializeField]
    private float _FastFallSpeed = 16f;
    [SerializeField]
    private float _NAirSpeedCap = 9.5f;
    [SerializeField]
    private float _MidAirJumpHeight = 16f;
    [SerializeField]
    private Transform _GroundCheck;
    [SerializeField]
    private LayerMask _GroundLayer;
    [SerializeField]
    private float _MaxDoubleJump;

    private void Start()
    {
        _RB = gameObject.GetComponent<Rigidbody2D>();
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
        _FastFall = context.action.triggered;
    }

    void Update()
    {
        PlayerInputVector = new Vector2(StandardizeMoveValues(_MovementInput.x), StandardizeMoveValues(_MovementInput.y));
        _Horizontal = PlayerInputVector.x;



        if (_Jumped && (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0))
        {
            if (!(_CoyoteTimeCounter > 0)) { _RB.velocity = new Vector2(_RB.velocity.x, _MidAirJumpHeight); _DoubleJumpCount--; _Jumped = false; } else { _RB.velocity = new Vector2(_RB.velocity.x, _JumpHeight); _Jumped = false; }
            Flip();
            _CoyoteTimeCounter = 0;
        }

        if (IsGrounded())
        {
            _DoubleJumpCount = _MaxDoubleJump;
            Flip();
            _CoyoteTimeCounter = _CoyoteTime;
            _FastFalling = false;
            _FastFall = false;
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
        Debug.Log(_IsDashing);
        if (IsGrounded())
        {
            if (_IsDashing)
            {
                _RB.velocity = new Vector2(Mathf.Clamp(_Horizontal * _DGroundSpeed, -_NAirSpeedCap, _NAirSpeedCap), _RB.velocity.y);
            }
            else
            {
                _RB.velocity = new Vector2(Mathf.Clamp(_Horizontal * _NGroundSpeed, -_NAirSpeedCap, _NAirSpeedCap), _RB.velocity.y);
            }
        }
        else
        {
            float ff = _FastFall ? _FastFallSpeed : 0.0f;
            float horizontalForce = _Horizontal * _NAirSpeed * Time.deltaTime;

            if (!_FastFalling && _FastFall)
            {
                _RB.velocity = new Vector2(_RB.velocity.x, -ff * 2);
                _FastFalling = true;
            }
            else
            {
                _RB.velocity = new Vector2(_RB.velocity.x, _RB.velocity.y - ff);
            }

            Debug.Log(_FastFall);

            _RB.AddForce(new Vector2(horizontalForce, 0));

            if (Mathf.Abs(_RB.velocity.x) > _NAirSpeedCap)
            {
                _RB.velocity = new Vector2(_NAirSpeedCap * Mathf.Sign(_RB.velocity.x), _RB.velocity.y - ff);
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

