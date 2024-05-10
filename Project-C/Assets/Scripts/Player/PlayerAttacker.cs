using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStateManager))]
[RequireComponent(typeof(PlayerInputReceiver))]
public class PlayerAttacker : MonoBehaviour
{
    public PlayerStateManager PlayerState;
    public PlayerController Player;
    private PlayerInputReceiver inputReceiver;
    private MoveSet moveSet;

    public GameObject SquareHitbox;
    public GameObject CapsuleHitbox;

    void Start()
    {
        PlayerState = GetComponent<PlayerStateManager>();
        Player = GetComponent<PlayerController>();
        inputReceiver = GetComponent<PlayerInputReceiver>();
        moveSet = new MoveSet();

        // Subscribe to input events
        inputReceiver.OnLightAttackEvent += (triggered) => TriggerMove(triggered, "GroundedLeftRightNormal");
        inputReceiver.OnHeavyAttackEvent += (triggered) => TriggerMove(triggered, "GroundedLeftRightHeavy");
        inputReceiver.OnTrademarkAttackEvent += (triggered) => TriggerMove(triggered, "LeftRightTrademark");
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        inputReceiver.OnLightAttackEvent -= (triggered) => TriggerMove(triggered, "GroundedLeftRightNormal");
        inputReceiver.OnHeavyAttackEvent -= (triggered) => TriggerMove(triggered, "GroundedLeftRightHeavy");
        inputReceiver.OnTrademarkAttackEvent -= (triggered) => TriggerMove(triggered, "LeftRightTrademark");
    }

    void TriggerMove(bool triggered, string moveName)
    {
        if (triggered && moveSet.moves.ContainsKey(moveName) && PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            MoveData move = moveSet.moves[moveName];
            StartCoroutine(PerformAttack(move.startup, move.active, move.endLag, DetermineHitbox(move.hitboxType), move.coordinates, move.rotation));
        }
    }

    GameObject DetermineHitbox(string type)
    {
        return (type == "Square") ? SquareHitbox : CapsuleHitbox;
    }

    IEnumerator PerformAttack(float startupTime, float activeTime, float endLag, GameObject hitboxPrefab, Vector3 hitboxSize, Vector3 rotation)
    {
        PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
        yield return new WaitForSeconds(startupTime);

        PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
        GameObject activeHitbox = CreateHitbox(hitboxPrefab, hitboxSize, rotation);
        yield return new WaitForSeconds(activeTime);

        Destroy(activeHitbox);
        PlayerState.State = PlayerStateManager.PossibleStates.Recovering;
        yield return new WaitForSeconds(endLag);

        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }

    GameObject CreateHitbox(GameObject prefab, Vector3 size, Vector3 eulerAngles)
    {
        int playerDir = Player.IsFacingRight ? 1 : -1;
        Vector3 position = new Vector3(transform.position.x + size.x * playerDir, transform.position.y + size.y, transform.position.z + size.z);
        GameObject instance = Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        instance.transform.localScale = size;
        instance.tag = gameObject.tag;
        instance.transform.parent = this.transform;
        return instance;
    }
}
