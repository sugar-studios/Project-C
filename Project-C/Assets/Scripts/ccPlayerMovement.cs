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
    public float FastFallMultiplier;
    public float GroundSpeed;
    public float JumpHeight;
    public float Gravity;
    public float MomentumBuildUpSpeed = 8f; // Speed at which momentum builds up
    public float AirborneMomentumFactor = 1.5f; // Increase momentum when airborne
    public int _MaxJumps;

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
        if (_JumpsRemaining > 0)
        {
            _YVelocity = Mathf.Sqrt(JumpHeight * 50);
            _JumpsRemaining--;
            Debug.Log("Jump! Jumps remaining: " + _JumpsRemaining);
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
        Vector3 boxSize = new Vector3(Controller.bounds.size.x, 0.1f, Controller.bounds.size.z); // Adjust the Y size to a small value for the ground check
        RaycastHit hit;
        Vector3 boxBottomCenter = Controller.bounds.center + Vector3.down * (Controller.bounds.extents.y + boxSize.y / 2); // Center of the bottom of the box

        bool hasHit = Physics.BoxCast(boxBottomCenter, boxSize / 2, Vector3.down, out hit, Quaternion.identity, 1f, stageLayerMask);

        // Draw box
        DebugDrawBox(boxBottomCenter, boxSize * 2, Quaternion.identity, hasHit ? Color.green : Color.red, 0f);

        return hasHit;
    }

    // Helper method to draw the box
    void DebugDrawBox(Vector3 center, Vector3 size, Quaternion orientation, Color color, float duration)
    {
        Vector3 halfExtents = size / 2;
        Vector3 frontTopLeft = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        Vector3 frontTopRight = center + orientation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        Vector3 frontBottomLeft = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        Vector3 frontBottomRight = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        Vector3 backTopLeft = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
        Vector3 backTopRight = center + orientation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
        Vector3 backBottomLeft = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
        Vector3 backBottomRight = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);

        Debug.DrawLine(frontTopLeft, frontTopRight, color, duration);
        Debug.DrawLine(frontTopRight, frontBottomRight, color, duration);
        Debug.DrawLine(frontBottomRight, frontBottomLeft, color, duration);
        Debug.DrawLine(frontBottomLeft, frontTopLeft, color, duration);

        Debug.DrawLine(backTopLeft, backTopRight, color, duration);
        Debug.DrawLine(backTopRight, backBottomRight, color, duration);
        Debug.DrawLine(backBottomRight, backBottomLeft, color, duration);
        Debug.DrawLine(backBottomLeft, backTopLeft, color, duration);

        Debug.DrawLine(frontTopLeft, backTopLeft, color, duration);
        Debug.DrawLine(frontTopRight, backTopRight, color, duration);
        Debug.DrawLine(frontBottomRight, backBottomRight, color, duration);
        Debug.DrawLine(frontBottomLeft, backBottomLeft, color, duration);
    }


    void Update()
    {
        StandardizeMoveValues();

        float horizontal = _Move.x;
        Vector3 targetVelocity = new Vector3(horizontal * GroundSpeed, 0f, 0f);
        float momentumMultiplier = _IsGrounded ? 1f : AirborneMomentumFactor + 1 * 2;
        _CurrentVelocity = Vector3.Lerp(_CurrentVelocity, targetVelocity, MomentumBuildUpSpeed * Time.deltaTime * momentumMultiplier);

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

        _YVelocity -= appliedGravity * Time.deltaTime;

        _IsGrounded = CheckGround();
        Debug.Log(_IsGrounded);
        if (_IsGrounded)
        {
            _YVelocity = -0.5f;
            _FastfallCheck = false;
            _FastFallBuffer = false;
            _DownInputDuration = 0f;
            _JumpsRemaining = _MaxJumps; // Reset jumps when grounded
        }

        Vector3 finalMove = _CurrentVelocity + new Vector3(0f, _YVelocity, 0f);
        Controller.Move(finalMove * Time.deltaTime);
    }
}