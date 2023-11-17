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

    private PlayerControls _Controls;
    private Vector2 _Move;

    //Before the first frame the game starts
    private void Awake()
    {
        _Controls = new PlayerControls();

        //_controls.Gameplay.Jump.performed += ctx => Jump();

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
    }

    // Update is called once per frame
    void Update()
    {
        StandardizeMoveValues();

        float _horizontal = _Move.x;

        float _vertical = _Move.y;

        Vector3 _moveVector = new Vector3(_horizontal, _vertical, 0f).normalized;

        //apply movement
        if (_moveVector.magnitude > 0.1f)
        {
            _moveVector.x = _moveVector.x * GroundSpeed * Time.deltaTime;
        }

        //apply gravity
        Controller.Move(_moveVector * GroundSpeed * Time.deltaTime);
    }
}
