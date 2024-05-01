using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("List of all players to track with the camera.")]
    public List<GameObject> players;

    [Header("Zoom Settings")]
    [Tooltip("Maximum zoom level.")]
    public float maxZoom = -8f;
    [Tooltip("Minimum zoom level.")]
    public float minZoom = -3f;
    [Tooltip("Speed of zoom adjustment.")]
    public float zoomSpeed = 5f;

    [Header("Camera Pan Settings")]
    [Tooltip("Speed of camera movement.")]
    public float moveSpeed = 5f;
    [Tooltip("Vertical offset for the camera position.")]
    public float yOffset = 0f;
    [Tooltip("Padding for the X-axis within the camera bounds.")]
    public float xPadding = 1f;
    [Tooltip("Padding for the Y-axis within the camera bounds.")]
    public float yPadding = 1f;

    [Header("Default Camera Position")]
    [Tooltip("Default position of the camera.")]
    public Vector3 defaultPosition = new Vector3(0f, 0f, -10f);

    [Header("Debug Settings")]
    [Tooltip("If true, camera will ignore blast zone constraints.")]
    public bool debugIgnoreBlastZones = false;
    [Tooltip("Backup manual reference of collider if the main camera border is not found.")]
    public Collider backupBlastZoneCollider;

    private float zoomLevel;
    private Bounds blastZoneBounds;

    private void Start()
    {
        InitializeBlastZone();
        zoomLevel = transform.position.z;
    }

    private void InitializeBlastZone()
    {
        Collider collider = GameObject.FindGameObjectWithTag("CameraBorder")?.GetComponent<Collider>();
        if (collider != null)
        {
            blastZoneBounds = collider.bounds;
        }
        else if (backupBlastZoneCollider != null)
        {
            blastZoneBounds = backupBlastZoneCollider.bounds;
            Debug.LogWarning("Using backup blast zone collider.");
        }
        else
        {
            Debug.LogError("No valid camera border found. Please attach a Collider.");
        }
    }

    private void Update()
    {
        float targetZoom = CalculateTargetZoom();
        zoomLevel = Mathf.Lerp(zoomLevel, targetZoom, Time.deltaTime * zoomSpeed);
        MoveCameraPosition();
    }

    private void MoveCameraPosition()
    {
        Vector3 targetPosition = CalculateTargetPosition() + new Vector3(0, yOffset, 0);
        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y, zoomLevel);

        if (!debugIgnoreBlastZones)
        {
            newPosition = AdjustPositionWithinBlastZone(newPosition);
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * moveSpeed);
    }

    private Vector3 AdjustPositionWithinBlastZone(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, blastZoneBounds.min.x + xPadding, blastZoneBounds.max.x - xPadding);
        float clampedY = Mathf.Clamp(position.y, blastZoneBounds.min.y + yPadding, blastZoneBounds.max.y - yPadding);
        return new Vector3(clampedX, clampedY, position.z);
    }

    private Vector3 CalculateTargetPosition()
    {
        if (players == null || players.Count == 0)
        {
            return defaultPosition;
        }

        Vector3 totalPosition = Vector3.zero;
        foreach (GameObject player in players)
        {
            totalPosition += player.transform.position;
        }
        return totalPosition / players.Count;
    }

    private float CalculateTargetZoom()
    {
        if (players == null || players.Count == 0)
        {
            return defaultPosition.z;
        }

        float maxDistance = 0f;
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = i + 1; j < players.Count; j++)
            {
                float distance = Vector3.Distance(players[i].transform.position, players[j].transform.position);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }

        float targetZoom = maxZoom - maxDistance;
        return Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }
}
