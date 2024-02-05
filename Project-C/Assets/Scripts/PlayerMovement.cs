using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class newPlayerMovement : MonoBehaviour
{
    public bool Grounded;
    public Vector2 PlayerInputVector;

    private PlayerControls _Controls;
    private Rigidbody2D _RB;
    private GameObject _Footstep;
    private Vector2 _RawInputVector;
    private float _Horizontal;
    private float _DoubleJumpCount;
    private float _CurrentSpeed;
    private float _CoyoteTime = 5f;
    private float _CoyoteTimeCounter;
    private bool _IsFacingRight;
    private bool _IsDashing = false;


    [SerializeField] private LayerMask _GroundLayer;
    [SerializeField] private float _Gravity = 4;
    [SerializeField] private float _JumpHeight;
    [SerializeField] private float _GroundSpeed;
    [SerializeField] private float _DashSpeed;
    [SerializeField] private float _AirSpeed;
    [SerializeField] private float _MaxSpeed;
    [SerializeField] private float _MaxDashSpeed;
    [SerializeField] private float MaxDoubleJump;
    [SerializeField] private float SpeedLerpFactor = 0.1f; // Adjust this value for smoother speed transitions

    //Audio shit
    [SerializeField] private AudioSource _JumpSound;


    private void Awake()
    {
        _Controls = new PlayerControls();
        _Controls.Gameplay.Jump.performed += ctx => Jump();
        _Controls.Gameplay.Dash.performed += ctx => StartDash();
        _Controls.Gameplay.Dash.canceled += ctx => StopDash();
        _Controls.Gameplay.Move.performed += ctx => _RawInputVector = ctx.ReadValue<Vector2>();
        _Controls.Gameplay.Move.canceled += ctx => _RawInputVector = Vector2.zero;
    }

    private void OnEnable()
    {
        Debug.Log("Controls Enabled");
        _Controls.Gameplay.Enable();
    }


    private void OnDisable()
    {
        Debug.Log("Controls Disabled");
        _Controls.Gameplay.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _RB = GetComponent<Rigidbody2D>();
        }
        catch 
        {
            Debug.LogError("No RigidBody2D detected");
            this.AddComponent<Rigidbody2D>();
            _RB = GetComponent<Rigidbody2D>();
            _RB.gravityScale = _Gravity;
            _RB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _RB.sleepMode = RigidbodySleepMode2D.NeverSleep;
            _RB.interpolation = RigidbodyInterpolation2D.Interpolate;
            Debug.Log("Added RigidBody");
        }

        _DoubleJumpCount = MaxDoubleJump;
        _Footstep = transform.GetChild(2).gameObject;
    }

    //Jump function
    void Jump()
    {
        if (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0)
        {
            _JumpSound.Play();
            _RB.velocity = new Vector2(_RB.velocity.x, _JumpHeight);
            Flip();
            _CoyoteTimeCounter = 0;
            if (!Grounded ) { _DoubleJumpCount--; }
        }
        
    }

    void StartDash()
    { 
        _IsDashing = true;
    }

    void StopDash()
    {
        _IsDashing = false;
    }

    void Flip()
    {
        if (_IsFacingRight && _Horizontal < 0f || !_IsFacingRight && _Horizontal > 0f)
        {
            _IsFacingRight = !_IsFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void Footsteps()
    {
        _Footstep.SetActive(true);
    }

    void StopFootsteps()
    {
        _Footstep.SetActive(false);
    }

    private Vector2 StandardizeMoveValues(Vector2 _Vector)
    {
        _Vector.x = (Mathf.Abs(_Vector.x) >= 0.34f) ? 1 * Mathf.Sign(_Vector.x) : 0;
        _Vector.y = (Mathf.Abs(_Vector.y) >= 0.34f) ? 1 * Mathf.Sign(_Vector.y) : 0;

        return _Vector;
    }

    void Update()
    {
        PlayerInputVector = StandardizeMoveValues(_RawInputVector);
        _Horizontal = PlayerInputVector.x;

        if (Grounded)
        {
            _DoubleJumpCount = MaxDoubleJump;
            Flip();
            _CoyoteTimeCounter = _CoyoteTime;
        }
        else
        {
            _CoyoteTime -= Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {
        float targetSpeed;
        float maxSpeed;

        if (Grounded)
        {
            targetSpeed = _IsDashing ? _DashSpeed : _GroundSpeed;
            maxSpeed = _IsDashing ? _MaxDashSpeed : _MaxSpeed;
        }
        else
        {
            targetSpeed = _AirSpeed;
            maxSpeed = _MaxSpeed;
        }

        float horizontalForce = _Horizontal * targetSpeed * Time.deltaTime;

        _RB.AddForce(new Vector2(horizontalForce, 0));

        if (_RB.velocity.x > maxSpeed)
        { 
            _RB.velocity = new Vector2(maxSpeed, _RB.velocity.y);
        }


        if (Mathf.Abs(_RB.velocity.x) > 0 && Grounded)
        {
            Footsteps();
        } else
        {
            StopFootsteps();
        }

    }
}
