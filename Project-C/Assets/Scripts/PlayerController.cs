using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Vector2 PlayerInputVector;
    public string State;

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
        _FastFall = context.action.triggered;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            if (State == "Free")
            {
                State = "Attacking";
                Debug.Log("Attack");
                StartCoroutine(AttackEndLag(1));
            }
        }
    }

    void Update()
    {
        PlayerInputVector = new Vector2(StandardizeMoveValues(_MovementInput.x), StandardizeMoveValues(_MovementInput.y));

        if (IsGrounded())
        {
            if (State == "Free")
            {
                _Horizontal = PlayerInputVector.x;
            }
            else
            {
                _Horizontal = 0;
            }
        }
        else
        {
            _Horizontal = PlayerInputVector.x;
        }



        if (_Jumped && (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0))
        {
            if (State == "Free")
            {
                if (!(_CoyoteTimeCounter > 0)) { _RB.velocity = new Vector2(_RB.velocity.x, _MidAirJumpHeight); _DoubleJumpCount--; _Jumped = false; } else { _RB.velocity = new Vector2(_RB.velocity.x, _JumpHeight); _Jumped = false; }
                _CoyoteTimeCounter = 0;
            }
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

    private IEnumerator AttackEndLag(float EndLag)
    {   
        yield return new WaitForSeconds(EndLag);
        State = "Free";
    }
}


