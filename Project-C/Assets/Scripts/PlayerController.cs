using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _RB;
    private bool _IsFacingRight = true;
    private bool _IsDashing = false;
    private float _Horizontal;
    private float _DoubleJumpCount;
    private float _CoyoteTime = .2f;
    private float _CoyoteTimeCounter;

    private Vector2 _MovementInput = Vector2.zero;
    private bool _Jumped;
    private bool _FastFalling;

    [SerializeField]
    private float _JumpHeight = 16f;
    [SerializeField]
    private float _NGroundSpeed = 8f;
    [SerializeField]
    private float _DGroundSpeed = 11f;
    [SerializeField]
    private float _NAirSpeed = 9.5f;
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
        _Jumped = context.ReadValue<bool>();
        _Jumped = context.action.triggered;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        _IsDashing = context.ReadValue<bool>();
        _IsDashing = context.action.triggered;
    }

    public void OnFastFall(InputAction.CallbackContext context)
    {
        _IsFastFalling = context.ReadValue<bool>();
        _IsFastFalling = context.action.triggered;
    }

    void Update()
    {
        _Horizontal = StandardizeMoveValues(Input.GetAxisRaw("Horizontal"));

        if (Input.GetButtonDown("Jump") && (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0))
        {
            if (!(_CoyoteTimeCounter > 0)) { _RB.velocity = new Vector2(_RB.velocity.x, _MidAirJumpHeight); _DoubleJumpCount--; } else { _RB.velocity = new Vector2(_RB.velocity.x, _JumpHeight); }
            Flip();
            _CoyoteTimeCounter = 0;
        }

        if (IsGrounded())
        {
            _DoubleJumpCount = _MaxDoubleJump;
            Flip();
            _CoyoteTimeCounter = _CoyoteTime;
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
            _RB.velocity = new Vector2(_Horizontal * _NAirSpeed, _RB.velocity.y);
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

