using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStateManager))]
[RequireComponent(typeof(PlayerInputReceiver))]
public class PlayerAttacker : MonoBehaviour
{
    public PlayerStateManager PlayerState;
    public PlayerController Player;
    private PlayerInputReceiver inputReceiver;

    public static GameObject SquareHitbox;
    public static GameObject CapsuleHitbox;

    private Dictionary<string, Attack> attacks = new Dictionary<string, Attack>();

    void Start()
    {
        PlayerState = GetComponent<PlayerStateManager>();
        Player = GetComponent<PlayerController>();
        inputReceiver = GetComponent<PlayerInputReceiver>();

        // Load attacks from JSON using the base character name
        attacks = JSONUtilityHelper.LoadAttacksForCharacter(gameObject.name);

        // Subscribe to input events
        inputReceiver.OnLightAttackEvent += (triggered) => StartAttack("light", triggered);
        inputReceiver.OnHeavyAttackEvent += (triggered) => StartAttack("heavy", triggered);
        inputReceiver.OnTrademarkAttackEvent += (triggered) => StartAttack("trademark", triggered);
    }


    void OnDestroy()
    {
        inputReceiver.OnLightAttackEvent -= (triggered) => StartAttack("light", triggered);
        inputReceiver.OnHeavyAttackEvent -= (triggered) => StartAttack("heavy", triggered);
        inputReceiver.OnTrademarkAttackEvent -= (triggered) => StartAttack("trademark", triggered);
    }

    void StartAttack(string attackId, bool triggered)
    {
        if (triggered && PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("Starting attack with ID: " + attackId);
            var attack = attacks[attackId];
            StartCoroutine(PerformAttack(attack));
        }
    }

    IEnumerator PerformAttack(Attack attack)
    {
        PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
        yield return new WaitForSeconds(attack.Startup);

        PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
        GameObject activeHitbox = CreateHitbox(attack);
        yield return new WaitForSeconds(attack.Active);

        Destroy(activeHitbox);
        PlayerState.State = PlayerStateManager.PossibleStates.Recovering;
        yield return new WaitForSeconds(attack.Endlag);

        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }

    GameObject CreateHitbox(Attack attack)
    {
        int playerDir = Player.IsFacingRight ? 1 : -1;
        Vector3 position = new Vector3(gameObject.transform.position.x + playerDir, gameObject.transform.position.y, gameObject.transform.position.z);
        GameObject instance = Instantiate(attack.HitboxPrefab, position, Quaternion.Euler(attack.Rotation));
        instance.transform.localScale = attack.HitboxSize;
        instance.tag = gameObject.tag;
        instance.transform.parent = this.transform;
        return instance;
    }
}
