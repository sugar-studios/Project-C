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
    // Constants
    private const float _HighThreshold = 0.75f;
    private const float _MediumThreshold = 0.20f;
    private const float _LowThreshold = 0.25f;
    private const float _MoveMagnitudeThreshold = 0.1f;

    // Public Variables
    public CharacterController Controller;
    public float FastFallMultiplier;
    public float GroundSpeed;
    public float JumpHeight;
    public float Gravity;
    public float MomentumBuildUpSpeed = 8f; // Speed at which momentum builds up
    public float AirborneMomentumFactor = 1.5f; // Increase momentum when airborne

    // Private Variables
    private PlayerControls _Controls;
    private Vector2 _Move;
    private Vector3 _CurrentVelocity;
    private float _YVelocity;
    private bool _IsGrounded;
    private bool _FastfallCheck;
    private bool _FastFallBuffer;

    // Before the first frame the game starts
    private void Awake()
    {
        _Controls = new PlayerControls();
        _Controls.Gameplay.Jump.performed += ctx => Jump();
        _Controls.Gameplay.Move.performed += ctx => _Move = ctx.ReadValue<Vector2>();
        _Controls.Gameplay.Move.canceled += ctx => _Move = Vector2.zero;
        _FastfallCheck = false;
        _FastFallBuffer = false;
    }

    // Enable controls
    private void OnEnable()
    {
        _Controls.Gameplay.Enable();
    }

    // Disable Controls
    private void OnDisable()
    {
        _Controls.Gameplay.Disable();
    }

    // Jump
    private void Jump()
    {
        if (_IsGrounded)
        {
            Debug.Log("Jump!");
            _YVelocity = Mathf.Sqrt(2 * Gravity * JumpHeight);
        }
    }

    // Set inputs to either 1, 0.35, or 0
    private void StandardizeMoveValues()
    {
        _Move.x = Mathf.Abs(_Move.x) > _HighThreshold ? 1f * Mathf.Sign(_Move.x) :
                   _MediumThreshold < Mathf.Abs(_Move.x) && Mathf.Abs(_Move.x) <= _HighThreshold ? 0.35f * Mathf.Sign(_Move.x) :
                   Mathf.Abs(_Move.x) < _LowThreshold ? 0 : 0f;

        _Move.y = Mathf.Abs(_Move.y) > _HighThreshold ? 1f * Mathf.Sign(_Move.y) : 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Standardize Move Values
        StandardizeMoveValues();

        // Horizontal input
        float horizontal = _Move.x;

        // Desired moveVector
        Vector3 targetVelocity = new Vector3(horizontal * GroundSpeed, 0f, 0f);

        // Apply momentum
        float momentumMultiplier = _IsGrounded ? 1f : AirborneMomentumFactor + 1 * 2;
        _CurrentVelocity = Vector3.Lerp(_CurrentVelocity, targetVelocity, MomentumBuildUpSpeed * Time.deltaTime * momentumMultiplier);

        float appliedGravity = Gravity;
        if (!_IsGrounded && _Move.y == -1) { _FastfallCheck = true; _FastFallBuffer = false; }
        if (_FastfallCheck && _Move.y != -1) { _FastFallBuffer = true; Debug.Log("hdsjaf"); }
        if (_FastfallCheck && _Move.y == -1 && _FastFallBuffer) { appliedGravity *= FastFallMultiplier; }

        // Always apply gravity
        _YVelocity -= appliedGravity * Time.deltaTime;

        // Check if the character is grounded
        _IsGrounded = Controller.isGrounded;
        if (_IsGrounded && _YVelocity < 0)
        {
            _YVelocity = -0.5f; // Small negative value to stick to the ground
            _FastfallCheck = false;
            _FastFallBuffer = false;
        }

        // Integrate vertical movement (jump and gravity) with horizontal movement
        Vector3 finalMove = _CurrentVelocity + new Vector3(0f, _YVelocity, 0f);

        // Move character controller
        Controller.Move(finalMove * Time.deltaTime);
    }
}



