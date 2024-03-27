using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public GameObject[] Players;
    public float MaxZoom = -8f;
    public float MinZoom = -3f;
    public float ZoomSpeed = 5f;
    public float MoveSpeed = 5f;
    public float YoffSet = 0f;
    public bool DEBUGleaveBlastZones = false; 

    private float ZoomLevel;
    private GameObject leftBracket;
    private GameObject rightBracket;
    private BoxCollider blastZoneCollider; 

    void Start()
    {
        leftBracket = this.gameObject.transform.GetChild(0).gameObject;
        rightBracket = this.gameObject.transform.GetChild(1).gameObject;

        blastZoneCollider = GameObject.FindGameObjectWithTag("Blastzones").GetComponent<BoxCollider>();

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
        Vector3 minCorner = blastZoneCollider.bounds.min;
        Vector3 maxCorner = blastZoneCollider.bounds.max;

        float clampedX = Mathf.Clamp(newPosition.x, minCorner.x, maxCorner.x);
        float clampedY = Mathf.Clamp(newPosition.y, minCorner.y, maxCorner.y);

        return new Vector3(clampedX, clampedY, newPosition.z);
    }

    Vector3 CalculateTargetPosition()
    {
        float totalX = 0f;
        float totalY = 0f;
        foreach (var player in Players)
        {
            totalX += player.transform.position.x;
            totalY += player.transform.position.y;
        }
        float averageX = totalX / Players.Length;
        float averageY = totalY / Players.Length;

        return new Vector3(averageX, averageY, transform.position.z);
    }

    float CalculateTargetZoom()
    {
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
