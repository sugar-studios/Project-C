using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public int JumpHeight;
    public float Gravity;
    public float MoveSpeed;
    public int MaxDoubleJumps;
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

    private void OnEnable()
    {
        _controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    private void Update()
    {

        if (Mathf.Abs(_Move.x) > .5f)
        {
            _Move.x = 1f * (_Move.x/ Mathf.Abs(_Move.x));
        }
        else if (Mathf.Abs(_Move.x) <= .47f)
        {
            _Move.x = .5f * (_Move.x/ Mathf.Abs(_Move.x));
        }
        else if (Mathf.Abs(_Move.x) <= .20f)
        {
            _Move.x = 0;
        }

        if (Mathf.Abs(_Move.y) > .5f)
        {
            _Move.y = 1f * (_Move.y / Mathf.Abs(_Move.y));
        }
        else if (Mathf.Abs(_Move.y) <= .47f)
        {
            _Move.y = 0;
        }



        if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(0, 0))
        {
            Direction = "Neutral";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(1, 0))
        {
            Direction = "Right";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(1, 1))
        {
            Direction = "TopRight";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(0, 1))
        {
            Direction = "Top";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(-1, 1))
        {
            Direction = "TopLeft";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(-1, 0))
        {
            Direction = "Left";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(-1, -1))
        {
            Direction = "BotLeft";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(0, -1))
        {
            Direction = "Bot";
        }
        else if (new Vector2(Mathf.Abs(_Move.x) / _Move.x, Mathf.Abs(_Move.y) / _Move.y) == new Vector2(1, -1))
        {
            Direction = "BotRight";
        }
        else
        {
            Direction = "Neutral";
        }

        //Debug.Log(Direction);

        if (Physics.Raycast(transform.position, Vector3.down, _Height))
        {
            _IsGrounded = true;
        }
        else
        {
            _IsGrounded = false;
        }

        if (_IsGrounded)
        {
            _CurrentDoubleJumps = MaxDoubleJumps;

            // Here is the modified movement code to restrict the character to X and Y axes
            Vector3 newPosition = transform.position + new Vector3(_Move.x * MoveSpeed * Time.deltaTime, 0, _Move.y * MoveSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
        else
        {
            // Here is the modified movement code for air movement
            Vector3 newPosition = transform.position + new Vector3(_Move.x * AirSpeed * Time.deltaTime, 0, _Move.y * AirSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
    }
   
}
