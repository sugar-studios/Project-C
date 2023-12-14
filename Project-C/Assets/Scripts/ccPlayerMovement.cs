using UnityEngine;


/*
For the purpose of perserving readability.

ThisIsAPublicVarriable

_ThisIsAPrivateVarriable

_thisIsALocalOrInstanceVarriable

IthisIsAnInterface
*/


public class ccPlayerMovement : MonoBehaviour
{
    public CharacterController Controller;
    public float GroundSpeed;
    public float Gravity;
    public GameObject GroundColider;
    public float JumpHeight;

    [SerializeField] Vector3 _MoveVector;
    [SerializeField] LayerMask _GroundPlayerMask;

    private PlayerControls _Controls;
    private Vector2 _Move;
    private bool _Jump;
    private float _Velocity;
    private bool _IsGrounded;
    private bool _Sprint;

    //Before the first frame the game starts
    private void Awake()
    {
        _Controls = new PlayerControls();

        _Controls.Gameplay.Jump.performed += ctx => Jump();

        _Controls.Gameplay.Sprint.started += ctx => StartSprint();
        _Controls.Gameplay.Sprint.canceled += ctx => StopSprint();

        _Controls.Gameplay.Move.performed += ctx => _Move = ctx.ReadValue<Vector2>();
        _Controls.Gameplay.Move.canceled += ctx => _Move = Vector2.zero;

    }

    //Enable controls
    private void OnEnable()
    {
        _Controls.Gameplay.Enable();
    }

    //Disable Controls
    private void OnDisable()
    {
        _Controls.Gameplay.Disable();
    }

    //Set inputs to either a 1 .5 or 0
    private void StandardizeMoveValues()
    {
        if (Mathf.Abs(_Move.x) > .2f)
        {
            _Move.x = 1f * Mathf.Sign(_Move.x);
        }
        else
        {
            _Move.x = 0f;
        }


        if (Mathf.Abs(_Move.y) > .75f)
        {
            _Move.y = 1f * (_Move.y / Mathf.Abs(_Move.y));
        }
        else
        {
            _Move.y = 0f;
        };
    }

    public void ApplyGravity()
    {
        if (_IsGrounded)
        {
            _MoveVector.y = -1f;
        }
        else
        {
            _MoveVector.y -= Gravity;
        }
    }

    public void Jump()
    {
        if (_IsGrounded)
        {
            _Jump = true;
        }
    }

    public void ApplyMovement()
    {
        if (_MoveVector.magnitude > 0.1f)
        {
            float GroundMoveSpeed = GroundSpeed;
            if (_Sprint)
            {
                GroundMoveSpeed *= 2;
            }
            _MoveVector.x = _MoveVector.x * GroundMoveSpeed;
            //_MoveVector.y = _MoveVector.y * GroundSpeed; // Assuming your forward/backward movement is along the z-axis

            // Optionally, you can remove the line above and use the following line for a more unified approach:
            // _MoveVector = _MoveVector * GroundSpeed * Time.deltaTime;
        }
    }

    public void StartSprint()
    {
        _Sprint = true;
    }

    public void StopSprint()
    {
        _Sprint = false;
    }

    public void CheckForJump()
    {
        if (_Jump)
        {
            _Jump = false;
            _MoveVector.y = Mathf.Sqrt(JumpHeight * 2f); // Calculate the initial jump velocity
        }
    }
    public void CheckGrounded()
    {
        if (Physics.CheckSphere(GroundColider.transform.position, 0.1f, _GroundPlayerMask))
        {
            _IsGrounded = true;
        }
        else
        {
            _IsGrounded = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        StandardizeMoveValues();

        float _horizontal = _Move.x;

        _MoveVector = new Vector3(_horizontal, 0, 0f);

        CheckGrounded();
        ApplyMovement();
        ApplyGravity();
        CheckForJump();
        //Debug.Log(_MoveVector);


        Controller.Move(_MoveVector * Time.deltaTime);    
    }
}
