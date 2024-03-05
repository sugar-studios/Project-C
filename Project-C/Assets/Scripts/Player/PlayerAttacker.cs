using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStateManager))]
public class PlayerAttacker : MonoBehaviour
{
    public PlayerStateManager PlayerState;
    public PlayerController Player;

    public GameObject SquareHitbox;
    public GameObject CapsuleHitbox;

    void Start()
    {
        PlayerState = gameObject.GetComponent<PlayerStateManager>();
        Player = gameObject.GetComponent<PlayerController>();
    }
    public void NormalAttackL()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("A");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            Vector3 targetpos = gameObject.transform.position;
            GameObject ActiveHitbox = CreateHitbox(SquareHitbox, targetpos, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            StartCoroutine(AttackTimeOut(ActiveHitbox, .5f, targetpos));
        }
    }
    public void NormalAttackM()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("B");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            Vector3 targetpos = gameObject.transform.position;
            GameObject ActiveHitbox = CreateHitbox(SquareHitbox, targetpos, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            StartCoroutine(AttackTimeOut(ActiveHitbox, .5f, targetpos));
        }
    }
    public void NormalAttackH()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("C");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            Vector3 targetpos = gameObject.transform.position;
            GameObject ActiveHitbox = CreateHitbox(SquareHitbox, targetpos, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            StartCoroutine(AttackTimeOut(ActiveHitbox, .5f, targetpos));
        }
    }
    public void SpecialAttack()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("D");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            Vector3 targetpos = gameObject.transform.position;
            GameObject ActiveHitbox = CreateHitbox(SquareHitbox, targetpos, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            StartCoroutine(AttackTimeOut(ActiveHitbox, .5f, targetpos));
        }
    }

    // Coroutine for handling the attack timeout
    IEnumerator AttackTimeOut(GameObject activeHitbox, float time, Vector3 targetPosition)
    {
        // Set the activeHitbox position to the targetPosition
        activeHitbox.transform.position = targetPosition;

        // Log message for debugging
        Debug.Log("Coroutine started - activeHitbox moved to target position.");

        // Wait for the specified amount of time
        yield return new WaitForSeconds(time);

        // Log message for debugging
        Debug.Log("Waiting time is over.");

        // Call RestartState and pass activeHitbox
        RestartState(activeHitbox);
    }

    void RestartState(GameObject activeHitbox)
    {
        Destroy(activeHitbox);

        // Change player state
        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }


    GameObject CreateHitbox(GameObject prefab, Vector3 position, Vector3 eulerAngles, Vector3 size)
    {
        // Assuming Player.IsFacingRight is a boolean indicating the player's facing direction
        int playerDir = Player.IsFacingRight ? 1 : -1;

        // Adjust the position based on the player's direction
        position = new Vector3(position.x * playerDir, position.y, position.z);

        // Instantiate the prefab with the given rotation and position adjustments
        GameObject instance = Instantiate(prefab, position, Quaternion.Euler(eulerAngles));

        // Set the local scale of the instance to the specified size
        instance.transform.localScale = size;

        // Return the newly created instance
        return instance;
    }
}
