using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private bool isFacingRight = true;
    private float _DoubleJumpCount;
    private float _CoyoteTime = .2f;
    private float _CoyoteTimeCounter;

    [SerializeField] private float _JumpHeight = 16f;
    [SerializeField] private float _MidAirJumpHeight = 16f;
    [SerializeField] private Rigidbody2D _RB;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float MaxDoubleJump;

    private void Start()
    {
        _RB = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontal = StandardizeMoveValues(Input.GetAxisRaw("Horizontal"));

        if (Input.GetButtonDown("Jump") && (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0))
        {
            if (!(_CoyoteTimeCounter > 0)) { _RB.velocity = new Vector2(_RB.velocity.x, _MidAirJumpHeight); _DoubleJumpCount--; } else { _RB.velocity = new Vector2(_RB.velocity.x, _JumpHeight); }
            Flip();
            _CoyoteTimeCounter = 0;
        }

        if (IsGrounded())
        {
            _DoubleJumpCount = MaxDoubleJump;
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

        Flip();
    }

    private void FixedUpdate()
    {
        _RB.velocity = new Vector2(horizontal * speed, _RB.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private float StandardizeMoveValues(float num)
    {
        num = (Mathf.Abs(num) >= 0.34f) ? 1 * Mathf.Sign(num) : 0;

        return num;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}

