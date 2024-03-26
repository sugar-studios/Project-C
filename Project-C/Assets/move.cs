using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public bool useUnityInput; // Changed variable name to avoid conflict with UnityEngine.Input
    // Update is called once per frame
    void Update()
    {
        if (useUnityInput)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position += new Vector3(-1 * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position += new Vector3(1 * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += new Vector3(-1 * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += new Vector3(1 * Time.deltaTime, 0, 0);
            }
        }
    }
}
