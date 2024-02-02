using UnityEngine;

public class PlayerFighter : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer; // Layer mask to target the player layer

    private PlayerControls _Controls;

    // Struct to hold launch data
    private struct LaunchData
    {
        public Vector3 direction;
        public float force;
        public bool shouldLaunch;

        public LaunchData(Vector3 direction, float force)
        {
            this.direction = direction;
            this.force = force;
            this.shouldLaunch = true;
        }
    }

    private LaunchData _currentLaunchData;

    private void Awake()
    {
        _Controls = new PlayerControls();
        _Controls.Gameplay.NormalAttack.performed += ctx => Attack();
    }

    private void OnEnable()
    {
        _Controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _Controls.Gameplay.Disable();
    }

    public void Hit(float power, float angle, float xSize, float ySize)
    {
        Vector3 size = new Vector3(xSize, ySize, xSize); // Assuming a depth similar to xSize for 3D
        Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward; // Adjust for 3D
        RaycastHit hit;

        bool hasHit = Physics.BoxCast(transform.position, size / 2, direction, out hit, Quaternion.identity, Mathf.Infinity, playerLayer);

        if (hasHit)
        {
            CharacterController characterController = hit.collider.GetComponent<CharacterController>();
            if (characterController != null)
            {
                Vector3 launchDirection = direction.normalized * power + Vector3.up * (power / 2); // Simple upward force
                _currentLaunchData = new LaunchData(launchDirection, power);
            }
        }
    }

    void Attack()
    {
        Debug.Log("Attack!");
        float power = 10f; // Example power
        float angle = 45f; // Example angle
        float xSize = 2f; // Example x size of the boxcast
        float ySize = 1f; // Example y size of the boxcast
        Hit(power, angle, xSize, ySize);
    }

    void Update()
    {
        if (_currentLaunchData.shouldLaunch)
        {
            // Assuming you have access to the CharacterController you want to move
            CharacterController controller = GetComponent<CharacterController>();

            if (controller != null)
            {
                // Apply the launch force (simplified, consider using deltaTime and decreasing force over time)
                controller.Move(_currentLaunchData.direction * Time.deltaTime);
                _currentLaunchData.shouldLaunch = false; // Reset launch flag
            }
        }
    }
}
