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
    private short _ManualFastFallLock = 0;
    private float _Horizontal;
    private float _DoubleJumpCount;
    private float _CoyoteTime = .2f;
    private float _CoyoteTimeCounter;
    private float _GravityMultiplier;
    public bool _IsFacingRight;
    private bool _IsDashing = false;
    private bool _ManualFastFall = false;


    [SerializeField] private LayerMask _GroundLayer;
    [SerializeField] private float _Gravity = 4;
    [SerializeField] private float _JumpHeight;
    [SerializeField] private float _MidAirJumpHeight;
    [SerializeField] private float _GroundSpeed;
    [SerializeField] private float _DashSpeed;
    [SerializeField] private float _AirSpeed;
    [SerializeField] private float _MaxSpeed;
    [SerializeField] private float _MaxDashSpeed;
    [SerializeField] private float _MaxAirSpeed;
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
    private void Jump()
    {
            if (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0)
            {
                if (!(_CoyoteTimeCounter > 0)) { _RB.velocity = new Vector2(_RB.velocity.x, _MidAirJumpHeight); _DoubleJumpCount--; } else { _RB.velocity = new Vector2(_RB.velocity.x, _JumpHeight); }
                _JumpSound.Play();
                Flip();
                _CoyoteTimeCounter = 0;
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
        if (_IsDashing)
        {
            _Footstep.GetComponent<AudioSource>().pitch = 3f;
            _Footstep.SetActive(true);
        }
        else
        {
            _Footstep.GetComponent<AudioSource>().pitch = 2.5f;
            _Footstep.SetActive(true);
        }
        
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
            _ManualFastFallLock = 0;
            _ManualFastFall = false;
            _GravityMultiplier = _Gravity;
        }
        else
        {
            if (_CoyoteTimeCounter > 0)
            {
                _CoyoteTimeCounter -= Time.deltaTime;
            }

            /*
            if (!_ManualFastFall && _ManualFastFallLock == 0 && PlayerInputVector.y == -1)
            {
                _ManualFastFallLock = 1;
            }

            if (!_ManualFastFall && _ManualFastFallLock == 1 && PlayerInputVector.y == 0)
            {
                _ManualFastFallLock = 2;
            }

            if (!_ManualFastFall && _ManualFastFallLock == 2 && PlayerInputVector.y == -1)
            {
                _ManualFastFall = true;
            }

            if ((_RB.velocity.y < 0 && PlayerInputVector.y == -1) || _ManualFastFall)
            { 
                _GravityMultiplier *= 1.1f;
            } */
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
            maxSpeed = _MaxAirSpeed;
        }

        float horizontalForce = _Horizontal * targetSpeed * Time.deltaTime;

        _RB.AddForce(new Vector2(horizontalForce, 0));

        if (Mathf.Abs(_RB.velocity.x) > maxSpeed)
        { 
            _RB.velocity = new Vector2(maxSpeed * Mathf.Sign(_RB.velocity.x), _RB.velocity.y);
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
