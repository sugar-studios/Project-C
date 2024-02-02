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
    public float MomentumBuildUpSpeed = 8f;
    public int MaxJumps = 2; // Total jumps available
    public float MomentumMultiplier;
    public float AirborneSpeed;
    public AudioSource jump;

    // Private Variables
    private PlayerControls _Controls;
    private Vector2 _Move;
    private Vector3 _CurrentVelocity;
    private float _YVelocity;
    private bool _IsGrounded;
    private bool _FastfallCheck;
    private bool _FastFallBuffer;
    private int _JumpsRemaining;
    private float _DownInputDuration = 0f;
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
        _JumpsRemaining = MaxJumps; // Initialize with max jumps on awake
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
        if (_JumpsRemaining > 0)
        {
            _YVelocity = Mathf.Sqrt(JumpHeight * 2f * Gravity);
            _JumpsRemaining--; // Decrement the jump counter
            _JumpRequested = true;
            jump.Play();
            
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
        float currentSpeed = _IsGrounded ? GroundSpeed : AirborneSpeed;
        Vector3 targetVelocity = new Vector3(horizontal * currentSpeed, 0f, 0f);
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
            _YVelocity = Mathf.Max(_YVelocity, -0.5f);
        }
        else
        {
            _YVelocity -= appliedGravity * Time.deltaTime;
        }

        Vector3 finalMove = _CurrentVelocity + new Vector3(0f, _YVelocity, 0f);
        Controller.Move(finalMove * Time.deltaTime);

        if (Controller.isGrounded)
        {
            _YVelocity = -0.5f; // Reset YVelocity when grounded
            _JumpsRemaining = MaxJumps; // Reset jumps when touching the ground
            _FastfallCheck = false;
            _FastFallBuffer = false;
            _DownInputDuration = 0f;
        }

        if (_JumpRequested && !Controller.isGrounded)
        {
            _JumpRequested = false;
        }
    }
}
