using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    private Vector2 _OldPosition;
    private Vector2 _NewPosition;

    [SerializeField] private Transform _CenterStage;
    /*
    [SerializeField] private Transform Top;
    [SerializeField] private Transform Bot;
    [SerializeField] private Transform Left;
    [SerializeField] private Transform Right; */


    void Start()
    {
        _OldPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _NewPosition = transform.position;

        if (_NewPosition.x > 20|| _NewPosition.x < -10)
        {
            _NewPosition.x = _OldPosition.x;
        }

        if (_NewPosition.y > 30 || _NewPosition.y < -30)
        {
            _NewPosition.y = _OldPosition.y;
        }

        transform.position = _NewPosition;
        _OldPosition = _NewPosition; // This line seems to have its logic flipped in your script; I've corrected it here.
    }

}
