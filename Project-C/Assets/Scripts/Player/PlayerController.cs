using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStateManager))]
public class PlayerController : MonoBehaviour
{

    #region Varible creation  

    public Vector2 PlayerInputVector;
    public Vector2 PlayerMovementVector;
    public PlayerStateManager PlayerState;
    public bool StanderizeMovement = false;

    private Rigidbody2D _RB;
    private bool _IsFacingRight = true;
    private bool _IsDashing = false;
    private bool _FastFalling = false;
    private float _Horizontal;
    private float _DoubleJumpCount;
    private float _CoyoteTime = .2f;
    private float _CoyoteTimeCounter;

    private Vector2 _MovementInput = Vector2.zero;
    private bool _Jumped = false;

    [SerializeField]
    private float _JumpHeight = 16f;

    [SerializeField]
    private float _FastFallSpeed = 0.1f;

    [SerializeField]
    private float _NGroundSpeedCap = 7f;

    [SerializeField]
    private float _DGroundSpeedCap = 11f;

    [SerializeField]
    private float _NAirSpeedCap = 9.5f;

    [SerializeField]
    private float _NGroundSpeed = 1750f;

    [SerializeField]
    private float _DGroundSpeed = 2250f;

    [SerializeField]
    private float _NAirSpeed = 2000f;

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

    #endregion

    private void Start()
    {
        //get rigidbody
        _RB = gameObject.GetComponent<Rigidbody2D>();

        PlayerState = gameObject.GetComponent<PlayerStateManager>();

        Debug.Log(PlayerState.State);
    }

    #region Input functions
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
        if (!IsGrounded())
        {
            _FastFalling = true;
        }
    }

    #endregion


    /*
     * Update is to manage states and inputs that need to happen indpendent of physiscs
     */
    void Update()
    {
        #region input vectors
        //Get the player input Vecotor
        PlayerInputVector = new Vector2(StandardizeValues(_MovementInput.x), StandardizeValues(_MovementInput.y));
        //Set movment insensity to either be the input vector or raw
        PlayerMovementVector = StanderizeMovement ? PlayerInputVector : _MovementInput;
        #endregion

        #region Jumping
        /*
         * Allow the player to jump if they get the jump flag, and they either have coyote time or double jump
         * If they don't have any coyote time, they'll consume a double jump
         */
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            if (_Jumped && (_CoyoteTimeCounter > 0 || _DoubleJumpCount > 0))
            {
                if (!(_CoyoteTimeCounter > 0))
                {
                    _FastFalling = false;
                    _RB.velocity = new Vector2(_RB.velocity.x / 3, _MidAirJumpHeight);
                    _DoubleJumpCount--; _Jumped = false;
                }
                else
                {
                    _FastFalling = false;
                    _RB.velocity = new Vector2(_RB.velocity.x / 3, _JumpHeight);
                    _Jumped = false;
                }
                _CoyoteTimeCounter = 0;
            }
        }
        #endregion

        #region Reset values on grounded and activate coyote time

        /*
         * If the player us grounded reset double jump, allow the player to change direction, reset coyote time, and turn of fast falling
         * if the player is not grounded start the coyote time counter. 
         */
        if (IsGrounded())
        {
            _DoubleJumpCount = _MaxDoubleJump;
            Flip();
            _CoyoteTimeCounter = _CoyoteTime;
            _FastFalling = false;
        }
        else
        {
            if (_CoyoteTimeCounter > 0)
            {
                _CoyoteTimeCounter -= Time.deltaTime;
            }
        }
        #endregion

    }

    /*
     * Fixed upadte is where all the physiscs based opeations happen like moving
     */
    private void FixedUpdate()
    {
        #region grounded move)
        if (IsGrounded())
        {
            if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
            {
                if (_IsDashing)
                {
                    Move(PlayerMovementVector.x, _DGroundSpeed, _DGroundSpeedCap, true);
                }
                else
                {
                    Move(PlayerMovementVector.x, _NGroundSpeed, _NGroundSpeedCap, true);
                }
            }
        }
        #endregion
        #region Arial move
        else
        {
            if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
            {
                Move(PlayerMovementVector.x, _NAirSpeed, _NAirSpeedCap);

                //If the player is fast falling add downward velocity to their descent
                if (_FastFalling)
                {
                    _RB.velocity = new Vector2(_RB.velocity.x, _RB.velocity.y - _FastFallSpeed);
                }
            }
            else
            {
                Move(PlayerMovementVector.x, _NAirSpeed/2, _NAirSpeedCap / 3, false);
            }
        }
        #endregion
    }


    #region Player Move Function
    /*Basically it takes in a horizantal, accelereation, max speed, force mode, and capping values. 
     * horizantal is an float that represent a postive or negitive number. Postive means right, negative means left. It aslso indaiacates the movement multiplier 1 = full speed, 0 = no speed.
     * acceleration is how fast the player will move forward (this is NOT max speed), it is how fast the player will reach max speed, higher number = more responisve.
     * max speed is the cap on how fast the player will move once full acceleration is reached. This is the actual player movement speed. If you don't put a max speed, it will not be capped
     * impulse is if you want instanious force or applied force. Impule makes going in the opposite direction faster and reponsive, but applied forece has more force behind it. 
     */
    private void Move(float horizantal, float acc, float max = 0, bool impulse = false)
    {
        if (impulse)
        {
            _RB.AddForce(new Vector2(horizantal * acc * Time.deltaTime, 0), ForceMode2D.Impulse);
        }
        else
        {
            _RB.AddForce(new Vector2(horizantal * acc * Time.deltaTime, 0));
        }
        if (max > 0)
        {
            _RB.velocity = new Vector2(Mathf.Clamp(_RB.velocity.x, -max, max), _RB.velocity.y);
        }
    }
    #endregion


    #region Ground Check
    /*
     * Checks if the user is grounded by doing a sphere cast at the ground check location (found in editor) 
     */
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_GroundCheck.position, 0.2f, _GroundLayer);
    }
    #endregion

    #region Standerize Inputs
    /*
     * This function takes in a number from 0-.99 and turns it into either a 1 or 0
     */
    private float StandardizeValues(float num)
    {
        num = (Mathf.Abs(num) >= 0.34f) ? 1 * Mathf.Sign(num) : 0;

        return num;
    }
    #endregion

    #region Flip player
    /*
     * This flips the orientaion of the player right/left
     */
    private void Flip()
    {
        if (_IsFacingRight && PlayerMovementVector.x < 0f || !_IsFacingRight && PlayerMovementVector.x > 0f)
        {
            _IsFacingRight = !_IsFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    #endregion
}


