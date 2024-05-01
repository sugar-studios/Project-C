using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class moveCamera : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("List of all players to track with the camera.")]
    public List<GameObject> Players;

    [Header("Zoom Settings")]
    [Tooltip("Maximum zoom level.")]
    public float MaxZoom = -8f;
    [Tooltip("Minimum zoom level.")]
    public float MinZoom = -3f;
    [Tooltip("Speed of zoom adjustment.")]
    public float ZoomSpeed = 5f;

    [Header("Camera Pan Settings")]
    [Tooltip("Speed of camera movement.")]
    public float MoveSpeed = 5f;
    [Tooltip("Vertical offset for the camera position.")]
    public float YoffSet = 0f;
    [Tooltip("Padding for the X-axis within the camera bounds.")]
    public float xPadding = 1f;
    [Tooltip("Padding for the Y-axis within the camera bounds.")]
    public float yPadding = 1f;

    [Header("Debug Settings")]
    [Tooltip("If true, camera will ignore blast zone constraints.")]
    public bool DEBUGleaveBlastZones = false;
    [Tooltip("Backup maunal refrence of collider if the main camera border is not found.")]
    public Collider backupBlastZoneCollider;

    private float ZoomLevel;
    private GameObject leftBracket;
    private GameObject rightBracket;
    private Collider blastZoneCollider;

    void Start()
    {
        leftBracket = this.gameObject.transform.GetChild(0).gameObject;
        rightBracket = this.gameObject.transform.GetChild(1).gameObject;

        blastZoneCollider = GameObject.FindGameObjectWithTag("CameraBoarder")?.GetComponent<Collider>();
        if (blastZoneCollider == null)
        {
            blastZoneCollider = backupBlastZoneCollider;
        }

        ZoomLevel = this.transform.position.z;
    }

    void Update()
    {
        float targetZoom = CalculateTargetZoom();
        ZoomLevel = Mathf.Lerp(ZoomLevel, targetZoom, Time.deltaTime * ZoomSpeed);
        MoveCameraPosition();
    }

    void MoveCameraPosition()
    {
        Vector3 targetPosition = CalculateTargetPosition();
        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y + YoffSet, ZoomLevel);

        if (!DEBUGleaveBlastZones)
        {
            newPosition = AdjustPositionWithinBlastZone(newPosition);
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * MoveSpeed);
    }

    Vector3 AdjustPositionWithinBlastZone(Vector3 newPosition)
    {
        Vector2 minCorner = blastZoneCollider.bounds.min;
        Vector2 maxCorner = blastZoneCollider.bounds.max;

        float clampedX = Mathf.Clamp(newPosition.x, minCorner.x + xPadding, maxCorner.x - xPadding);
        float clampedY = Mathf.Clamp(newPosition.y, minCorner.y + yPadding, maxCorner.y - yPadding);

        return new Vector3(clampedX, clampedY, newPosition.z);
    }

    Vector3 CalculateTargetPosition()
    {
        if (Players == null || Players.Count == 0)
        {
            return new Vector3(0, 0, -8);
        }

        float totalX = 0f;
        float totalY = 0f;
        foreach (var player in Players)
        {
            totalX += player.transform.position.x;
            totalY += player.transform.position.y;
        }
        float averageX = Players.Count > 0 ? totalX / Players.Count : 0;
        float averageY = Players.Count > 0 ? totalY / Players.Count : 0;

        return new Vector3(averageX, averageY, transform.position.z);
    }

    float CalculateTargetZoom()
    {
        if (Players == null || Players.Count == 0)
        {
            return MaxZoom;
        }

        float maxDistance = 0f;
        foreach (var player in Players)
        {
            foreach (var otherPlayer in Players)
            {
                float distanceX = Mathf.Abs(player.transform.position.x - otherPlayer.transform.position.x);
                float distanceY = Mathf.Abs(player.transform.position.y - otherPlayer.transform.position.y);
                float distance = Mathf.Max(distanceX, distanceY);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }

        float targetZoom = MaxZoom - maxDistance;
        return Mathf.Clamp(targetZoom, MinZoom, MaxZoom);
    }
}