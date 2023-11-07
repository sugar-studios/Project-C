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

    private PlayerControls _controls;

    private int _CurrentDoubleJumps;

    private float _Height;

    private bool _IsGrounded;

    private Ray _Ray;

    private Vector2 _Move;

    private Vector2 _Direction;


    private void Start()
    {
        _Height = Renda.bounds.size.y;
        SetGravity(Gravity);
    }

    private void Awake()
    { 
        _controls = new PlayerControls();

        _controls.Gameplay.Jump.performed += ctx => Jump();

        _controls.Gameplay.Move.performed += ctx => _Move = _Direction = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Move.canceled += ctx => _Move = Vector2.zero;
    }

    private void Jump()
    {
        if (_CurrentDoubleJumps >= 1)
        {
            Vector2 newVelocity = rb.velocity;
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
            Vector2 m = new Vector2(_Move.x, 0) * Time.deltaTime * MoveSpeed;
            transform.Translate(m, Space.World);
        }
        else
        {
            Vector2 m = new Vector2(_Move.x, 0) * Time.deltaTime * AirSpeed;
            transform.Translate(m, Space.World);
        }

        
    }


}
