using UnityEngine;

public class LedgeGrabber : MonoBehaviour
{
    public Transform ledgeCheckPoint; // An empty GameObject indicating where to check for ledges
    public LayerMask ledgeLayer; // Layer mask to identify ledges
    public float checkRadius = 0.5f; // Radius for checking
    public CharacterController controller; // Reference to the CharacterController

    private bool isGrabbingLedge = false;
    private Vector3 grabPosition;

    void Update()
    {
        CheckForLedge();

        if (isGrabbingLedge)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) // Example input for climbing up
            {
                ClimbUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) // Example input for dropping down
            {
                DropDown();
            }
        }
    }

    void CheckForLedge()
    {
        Collider[] hits = Physics.OverlapSphere(ledgeCheckPoint.position, checkRadius, ledgeLayer);
        if (hits.Length > 0)
        {
            isGrabbingLedge = true;
            grabPosition = hits[0].transform.position;
            // Adjust grabPosition based on the character's position and the ledge's properties
            controller.enabled = false; // Temporarily disable the controller to manually position the character
            transform.position = grabPosition;
            // Transition to ledge grab animation state here
        }
        else
        {
            isGrabbingLedge = false;
            controller.enabled = true; // Re-enable the controller for normal movement
        }
    }

    void ClimbUp()
    {
        // Logic to pull the character up from the ledge
        // Typically involves setting the character's position to the top of the ledge and resuming normal control
        isGrabbingLedge = false;
        controller.enabled = true;
    }

    void DropDown()
    {
        // Logic to let go of the ledge and resume falling or jumping
        isGrabbingLedge = false;
        controller.enabled = true;
    }
}
