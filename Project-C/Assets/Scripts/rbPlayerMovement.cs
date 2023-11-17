using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public int JumpHeight;
    public float Gravity;
    public float GroundSeed;
    public int MaxDoubleJumps;
    public float MaxSpeed;
    public int AirSpeed;
    public MeshRenderer Renda;
    public Rigidbody rb;
    public string Direction;

    private PlayerControls _controls;
    private int _CurrentDoubleJumps;
    private float _Height;
    private bool _IsGrounded;
    private Vector2 _Move;

    private void Start()
    {
        _Height = Renda.bounds.size.y;
        SetGravity(Gravity);
    }

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Gameplay.Jump.performed += ctx => Jump();

        _controls.Gameplay.Move.performed += ctx => _Move = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Move.canceled += ctx => _Move = Vector2.zero;
    }

    private void Jump()
    {
        if (_CurrentDoubleJumps >= 1)
        {
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = JumpHeight;
            rb.velocity = newVelocity;
            _CurrentDoubleJumps--;
        }
    }

    private void SetGravity(float newGravityStrength)
    {
        Gravity = newGravityStrength;
        Physics.gravity = Vector3.down * Gravity;
    }

    private void CheckSpeed(float _MaxSpeed)
    {
        _MaxSpeed *= (_Move.x / Mathf.Abs(_Move.x));
        if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(_MaxSpeed))
        {
            Vector3 newVelocity = new Vector3(_MaxSpeed, rb.velocity.y, rb.velocity.z);
            rb.velocity = newVelocity;
        }
    }

    private void OnEnable()
    {
        _controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    private void NormalizeInput()
    {
    }

    private void GetDirecton()
    { 
    }

    private void Update()
    {
        //Set inputs to either a 1 .5 or 0
        if (Mathf.Abs(_Move.x) > .75f)
        {
            _Move.x = 1f * (_Move.x / Mathf.Abs(_Move.x));
        }
        else if (.20f < Mathf.Abs(_Move.x) && Mathf.Abs(_Move.x) <= .75f)
        {
            _Move.x = .35f * (_Move.x / Mathf.Abs(_Move.x));
        }
        else if (Mathf.Abs(_Move.x) < .25f)
        {
            _Move.x = 0;
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

        //Get Directional input as string
        Vector2 _TempMove = new Vector2(0, 0);
        if (_Move.x == 0)
        {
            if (_Move.y == 0)
            {
                _TempMove = new Vector2(0, 0);
            }
            else
            {
                _TempMove = new Vector2(0, Mathf.Abs(_Move.y) / _Move.y);
            }
        }
        else
        {
            if (_Move.y == 0)
            {
                _TempMove = new Vector2(Mathf.Abs(_Move.x) / _Move.x, 0);
            }
            else
            {

                _TempMove = new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y);
            }
        }

        if (_TempMove == new Vector2(0, 0))
        {
            Direction = "Neutral";
        }
        else if (_TempMove == new Vector2(1, 0))
        {
            Direction = "Right";
        }
        else if (_TempMove == new Vector2(1, 1))
        {
            Direction = "TopRight";
        }
        else if (_TempMove == new Vector2(0, 1))
        {
            Direction = "Top";
        }
        else if (_TempMove == new Vector2(-1, 1))
        {
            Direction = "TopLeft";
        }
        else if (_TempMove == new Vector2(-1, 0))
        {
            Direction = "Left";
        }
        else if (_TempMove == new Vector2(-1, -1))
        {
            Direction = "BotLeft";
        }
        else if (_TempMove == new Vector2(0, -1))
        {
            Direction = "Bot";
        }
        else if (_TempMove == new Vector2(1, -1))
        {
            Direction = "BotRight";
        }
        else
        {
            Direction = "Neutral";
        }

        //Check if grounded
        if (Physics.Raycast(transform.position, Vector3.down, _Height))
        {
            _IsGrounded = true;
        }
        else
        {
            _IsGrounded = false;
        }


        //What to do depending on if the player is grounded or not
        if (_IsGrounded)
        {
            //Reset Double Jumps
            _CurrentDoubleJumps = MaxDoubleJumps;

            rb.AddForce(GroundSeed * _Move.x * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(AirSpeed * _Move.x * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        CheckSpeed(MaxSpeed);


    }

}
