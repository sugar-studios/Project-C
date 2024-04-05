using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public List<GameObject> Players;
    public float MaxZoom = -8f;
    public float MinZoom = -3f;
    public float ZoomSpeed = 5f;
    public float MoveSpeed = 5f;
    public float YoffSet = 0f;
    public bool DEBUGleaveBlastZones = false;

    private float ZoomLevel;
    private GameObject leftBracket;
    private GameObject rightBracket;
    private BoxCollider2D blastZoneCollider; // Changed to BoxCollider2D for 2D

    void Start()
    {
        leftBracket = this.gameObject.transform.GetChild(0).gameObject;
        rightBracket = this.gameObject.transform.GetChild(1).gameObject;

        // Changed to get BoxCollider2D component for 2D
        blastZoneCollider = GameObject.FindGameObjectWithTag("CameraBoarder").GetComponent<BoxCollider2D>();

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
        // Adjustments for 2D by using the bounds of the BoxCollider2D
        Vector2 minCorner = blastZoneCollider.bounds.min;
        Vector2 maxCorner = blastZoneCollider.bounds.max;

        float clampedX = Mathf.Clamp(newPosition.x, minCorner.x, maxCorner.x);
        float clampedY = Mathf.Clamp(newPosition.y, minCorner.y, maxCorner.y);

        return new Vector3(clampedX, clampedY, newPosition.z);
    }

    Vector3 CalculateTargetPosition()
    {
        if (Players == null || Players.Count == 0)
        {
            return new Vector3(0, 0, -8); // Default position for 2D
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
            return MaxZoom; // Default zoom for 2D
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
