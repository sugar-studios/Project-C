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
    private const float Gravity = 9.8f; // Adjust as needed

    // Public Variables
    public CharacterController Controller;
    public float GroundSpeed;
    public float JumpHeight; // Adjust as needed

    // Private Variables
    private PlayerControls _Controls;
    private Vector2 _Move;
    private Vector3 _NVelocity;
    private Vector3 _OVelocity;
    private float _YVelocity; // Vertical velocity for jumping and gravity
    private bool _IsGrounded; // Add a grounded check

    // Before the first frame the game starts
    private void Awake()
    {
        _Controls = new PlayerControls();

        // Jump function
        _Controls.Gameplay.Jump.performed += ctx => Jump();

        // Get move value
        _Controls.Gameplay.Move.performed += ctx => _Move = ctx.ReadValue<Vector2>();
        _Controls.Gameplay.Move.canceled += ctx => _Move = Vector2.zero;
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
    // Jump
    private void Jump()
    {
        if (_IsGrounded)
        {
            _YVelocity = Mathf.Sqrt(-1 * JumpHeight * Gravity);
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
        // Create moveVector
        StandardizeMoveValues();

        float horizontal = _Move.x;
        float vertical = _Move.y;

        Vector3 moveVector = new Vector3(horizontal, 0f, 0f);

        if (moveVector.magnitude > _MoveMagnitudeThreshold)
        {
            moveVector *= GroundSpeed * Time.deltaTime;
        }

        // Apply gravity
        if (!_IsGrounded)
        {
            _YVelocity -= Gravity * Time.deltaTime;
        }
        else
        {
            _YVelocity = -Gravity * Time.deltaTime;
        }

        // Lerp between old and new velocity
        Vector3 lerpedVelocity = Vector3.Lerp(_OVelocity, _NVelocity, 0.5f); // Adjust the lerp factor as needed

        // Move character
        Controller.Move((lerpedVelocity + new Vector3(0f, _YVelocity, 0f)) * GroundSpeed * Time.deltaTime);

        // Get old velocity
        _OVelocity = moveVector;

        // Grounded check
        _IsGrounded = Controller.isGrounded;
        if (_IsGrounded)
        {
            _YVelocity = -Gravity * Time.deltaTime; // Reset vertical velocity when grounded
        }
    }
}
