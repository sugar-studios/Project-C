/*
For the purpose of perserving readability.

ThisIsAPublicVarriable

_ThisIsAPrivateVarriable

_thisIsALocalOrInstanceVarriable

IthisIsAnInterface
*/

using UnityEngine;

public class ccPlayerMovement : MonoBehaviour
{
    [SerializeField] LayerMask stageLayerMask;

    // Constants
    private const float _HighThreshold = 0.75f;
    private const float _MediumThreshold = 0.20f;
    private const float _LowThreshold = 0.25f;
    private const float _MoveMagnitudeThreshold = 0.1f;

    // Public Variables
    public CharacterController Controller;
    public float Traction;
    public float FastFallMultiplier;
    public float GroundSpeed;
    public float JumpHeight;
    public float Gravity;
    public float MomentumBuildUpSpeed = 8f; // Speed at which momentum builds up
    public int _MaxJumps;
    public float MomentumMultiplier;
    public float AirborneSpeed;

    // Private Variables
    private PlayerControls _Controls;
    private Vector2 _Move;
    private Vector3 _CurrentVelocity;
    private float _YVelocity;
    private bool _IsGrounded;
    private bool _FastfallCheck;
    private bool _FastFallBuffer;
    private int _JumpsRemaining; // Tracks the number of jumps remaining (for double jumping)
    private float _DownInputDuration = 0f; // Tracks how long the down input has been held
    private RaycastHit _Hit;
    private float _GroundCheckDistance = 12f;
    private bool _HitDetect;
    private bool _JumpRequested = false;

    private void Awake()
    {
        _Controls = new PlayerControls();
        _Controls.Gameplay.Jump.performed += ctx => Jump();
        _Controls.Gameplay.Move.performed += ctx => _Move = ctx.ReadValue<Vector2>();
        _Controls.Gameplay.Move.canceled += ctx => _Move = Vector2.zero;
        _FastfallCheck = false;
        _FastFallBuffer = false;
        _JumpsRemaining = _MaxJumps; // Initialize with max jumps on awake
    }

    private void OnEnable()
    {
        _Controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _Controls.Gameplay.Disable();
    }

    private void Jump()
    {
        Debug.Log("JUMP");
        if (_IsGrounded)
        {
            Debug.Log("TRUE JUMP");
            _YVelocity = Mathf.Sqrt(JumpHeight * 2f * Gravity); // Use the kinematic equation for jump height
            _JumpRequested = true; // Set the flag to true when jump is requested
        }
    }

    private void StandardizeMoveValues()
    {
        _Move.x = Mathf.Abs(_Move.x) > _HighThreshold ? 1f * Mathf.Sign(_Move.x) :
                   (_MediumThreshold < Mathf.Abs(_Move.x) && Mathf.Abs(_Move.x) <= _HighThreshold) ? 0.35f * Mathf.Sign(_Move.x) :
                   (Mathf.Abs(_Move.x) < _LowThreshold ? 0 : 0f);

        _Move.y = Mathf.Abs(_Move.y) > _HighThreshold ? 1f * Mathf.Sign(_Move.y) : 0f;
    }

    private bool CheckGround()
    {
        _HitDetect = Physics.BoxCast(Controller.bounds.center, transform.localScale * 0.5f, Vector3.down, out _Hit, transform.rotation, _GroundCheckDistance);
        //if (_HitDetect) {Debug.Log("Hit : " + _Hit.collider.name);} else {Debug.Log(null);}
        return _HitDetect;
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (_HitDetect)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, Vector3.down * _Hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(Controller.bounds.center + Vector3.down * _Hit.distance, transform.localScale);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray down from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, Vector3.down * _GroundCheckDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(Controller.bounds.center + Vector3.down * _GroundCheckDistance, transform.localScale);
        }
    }*/



    void Update()
    {
        StandardizeMoveValues();

        float horizontal = _Move.x;

        // Use AirborneSpeed if not grounded, otherwise use GroundSpeed
        float currentSpeed = _IsGrounded ? GroundSpeed : AirborneSpeed;
        Vector3 targetVelocity = new Vector3(horizontal * currentSpeed, 0f, 0f);

        // Adjust the momentum build-up based on whether the character is airborne or grounded
        float currentMomentumBuildUpSpeed = _IsGrounded ? MomentumBuildUpSpeed : MomentumBuildUpSpeed * Traction;
        _CurrentVelocity = Vector3.Lerp(_CurrentVelocity, targetVelocity, currentMomentumBuildUpSpeed * Time.deltaTime);

        _IsGrounded = Controller.isGrounded; // Check if grounded before applying gravity

        float appliedGravity = Gravity;

        if (!_IsGrounded)
        {
            if (_Move.y == -1)
            {
                if (!_FastfallCheck)
                {
                    _FastfallCheck = true;
                    _DownInputDuration += Time.deltaTime;
                    if (_DownInputDuration >= 0.5f && !_FastFallBuffer)
                    {
                        appliedGravity *= FastFallMultiplier;
                        Debug.Log("FastFall by Hold");
                        _FastFallBuffer = true;
                    }
                }
                else if (_FastFallBuffer)
                {
                    appliedGravity *= FastFallMultiplier;
                    Debug.Log("FastFall by Double Tap");
                }
            }
            else
            {
                if (_FastfallCheck && !_FastFallBuffer)
                {
                    _FastFallBuffer = true;
                }
                _DownInputDuration = 0f;
            }
        }

        if (_IsGrounded && !_JumpRequested)
        {
            // Apply a small negative velocity when grounded to stick the player to the ground
            _YVelocity = Mathf.Max(_YVelocity, -0.5f);
        }
        else
        {
            // Apply gravity over time
            if (_YVelocity - (appliedGravity * Time.deltaTime) <= -appliedGravity)
            {
                _YVelocity = -appliedGravity;
            }
            else
            {
                _YVelocity -= appliedGravity * Time.deltaTime;
            }
        }

        Vector3 finalMove = _CurrentVelocity + new Vector3(0f, _YVelocity, 0f);
        Controller.Move(finalMove * Time.deltaTime);

        // After moving, check if character is grounded
        if (Controller.isGrounded && !_JumpRequested)
        {
            _YVelocity = -0.5f; // Reset YVelocity when grounded but not when jump is requested
            _JumpsRemaining = _MaxJumps; // Reset jumps
            _FastfallCheck = false;
            _FastFallBuffer = false;
            _DownInputDuration = 0f;
        }

        // Reset the jump requested flag after the character has left the ground
        if (_JumpRequested && !Controller.isGrounded)
        {
            _JumpRequested = false;
        }
    }
}