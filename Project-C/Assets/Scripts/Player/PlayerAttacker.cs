using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStateManager))]
[RequireComponent(typeof(PlayerInputReceiver))]  // Ensure PlayerInputReceiver is a required component
public class PlayerAttacker : MonoBehaviour
{
    public PlayerStateManager PlayerState;
    public PlayerController Player;
    private PlayerInputReceiver inputReceiver;  // Reference to the PlayerInputReceiver

    public GameObject SquareHitbox;
    public GameObject CapsuleHitbox;

    void Start()
    {
        PlayerState = gameObject.GetComponent<PlayerStateManager>();
        Player = gameObject.GetComponent<PlayerController>();
        inputReceiver = gameObject.GetComponent<PlayerInputReceiver>();

        // Subscribe to input events
        inputReceiver.OnLightAttackEvent += (triggered) => StartAttack(triggered, 0.1f, 0.5f, 0.2f, SquareHitbox, new Vector3(1, 1, 1), Vector3.zero);
        inputReceiver.OnHeavyAttackEvent += (triggered) => StartAttack(triggered, 0.2f, 0.7f, 0.3f, SquareHitbox, new Vector3(1, 1, 1), new Vector3(0, 0, 45));
        inputReceiver.OnTrademarkAttackEvent += (triggered) => StartAttack(triggered, 0.3f, 1.0f, 0.4f, CapsuleHitbox, new Vector3(2, 2, 2), new Vector3(0, 0, 90));
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        inputReceiver.OnLightAttackEvent -= (triggered) => StartAttack(triggered, 0.1f, 0.5f, 0.2f, SquareHitbox, new Vector3(1, 1, 1), Vector3.zero);
        inputReceiver.OnHeavyAttackEvent -= (triggered) => StartAttack(triggered, 0.2f, 0.7f, 0.3f, SquareHitbox, new Vector3(1, 1, 1), new Vector3(0, 0, 45));
        inputReceiver.OnTrademarkAttackEvent -= (triggered) => StartAttack(triggered, 0.3f, 1.0f, 0.4f, CapsuleHitbox, new Vector3(2, 2, 2), new Vector3(0, 0, 90));
    }

    void StartAttack(bool triggered, float startupTime, float activeTime, float endLag, GameObject hitboxPrefab, Vector3 hitboxSize, Vector3 rotation)
    {
        if (triggered && PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("Starting attack");
            StartCoroutine(PerformAttack(startupTime, activeTime, endLag, hitboxPrefab, hitboxSize, rotation));
        }
    }

    IEnumerator PerformAttack(float startupTime, float activeTime, float endLag, GameObject prefab, Vector3 size, Vector3 eulerAngles)
    {
        PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
        yield return new WaitForSeconds(startupTime);

        PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
        GameObject activeHitbox = CreateHitbox(prefab, new Vector3(1f, 0, 0), eulerAngles, size);
        yield return new WaitForSeconds(activeTime);

        Destroy(activeHitbox);
        PlayerState.State = PlayerStateManager.PossibleStates.Recovering;
        yield return new WaitForSeconds(endLag);

        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }

    GameObject CreateHitbox(GameObject prefab, Vector3 position, Vector3 eulerAngles, Vector3 size)
    {
        int playerDir = Player.IsFacingRight ? 1 : -1;
        position = new Vector3(gameObject.transform.position.x + position.x * playerDir, gameObject.transform.position.y + position.y, gameObject.transform.position.z + position.z);
        GameObject instance = Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        instance.transform.localScale = size;
        instance.tag = gameObject.tag;
        instance.transform.parent = this.transform;
        return instance;
    }
}
