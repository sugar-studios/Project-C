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
            Vector3 targetpos = new Vector3(1f,
                                            0,
                                            0);
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

    IEnumerator AttackTimeOut(GameObject activeHitbox, float time, Vector3 targetPosition)
    {
        float endTime = Time.time + time;

        int playerDir = Player.IsFacingRight ? 1 : -1;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time + time - endTime;

            activeHitbox.transform.position = new Vector3(gameObject.transform.position.x + (targetPosition.x * playerDir),
                                                          gameObject.transform.position.y + targetPosition.y,
                                                          gameObject.transform.position.z + targetPosition.z);

            yield return null;
        }

        RestartState(activeHitbox);
    }


    void RestartState(GameObject activeHitbox)
    {
        Destroy(activeHitbox);
        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }


    GameObject CreateHitbox(GameObject prefab, Vector3 position, Vector3 eulerAngles, Vector3 size)
    {
        int playerDir = Player.IsFacingRight ? 1 : -1;
        position = new Vector3(gameObject.transform.position.x + position.x * playerDir, gameObject.transform.position.y + position.y, gameObject.transform.position.z + position.z);
        GameObject instance = Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        instance.transform.localScale = size;
        return instance;
    }
}
