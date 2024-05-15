using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStateManager))]
[RequireComponent(typeof(PlayerInputReceiver))]
public class PlayerAttacker : MonoBehaviour
{
    public PlayerStateManager PlayerState;
    public PlayerController Player;
    private PlayerInputReceiver inputReceiver;
    public GameObject SquareHitbox;
    public GameObject CapsuleHitbox;

    public string characterName;
    private JSONReader jsonReader;
    private JSONReader.Moveset currentMoveset;

    void Start()
    {
        PlayerState = gameObject.GetComponent<PlayerStateManager>();
        Player = gameObject.GetComponent<PlayerController>();
        inputReceiver = gameObject.GetComponent<PlayerInputReceiver>();
        jsonReader = gameObject.GetComponent<JSONReader>();

        // Load the moveset for the specified character
        currentMoveset = jsonReader.GetMovesetByName(characterName);

        if (currentMoveset == null)
        {
            Debug.LogError("Moveset not found for character: " + characterName);
            return;
        }

        // Subscribe to input events
        inputReceiver.OnLightAttackEvent += (triggered) => StartAttack(triggered, currentMoveset.lightAttack);
        inputReceiver.OnHeavyAttackEvent += (triggered) => StartAttack(triggered, currentMoveset.heavyAttack);
        inputReceiver.OnTrademarkAttackEvent += (triggered) => StartAttack(triggered, currentMoveset.trademarkAttack);
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        inputReceiver.OnLightAttackEvent -= (triggered) => StartAttack(triggered, currentMoveset.lightAttack);
        inputReceiver.OnHeavyAttackEvent -= (triggered) => StartAttack(triggered, currentMoveset.heavyAttack);
        inputReceiver.OnTrademarkAttackEvent -= (triggered) => StartAttack(triggered, currentMoveset.trademarkAttack);
    }

    void StartAttack(bool triggered, Attack attack)
    {
        if (triggered && PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("Starting attack: " + attack.name);
            if (attack.attackType == "projectile")
            {
                if (attack.chargeable)
                {
                    StartCoroutine(PerformChargeableProjectileAttack(attack));
                }
                else
                {
                    StartCoroutine(PerformProjectileAttack(attack));
                }
            }
            else
            {
                StartCoroutine(PerformMultiHitAttack(attack.hits, attack.hitboxType));
            }

            // Handle maintaining momentum
            if (attack.maintainMomentum)
            {
                PlayerState.State = PlayerStateManager.PossibleStates.MaintainMomentum;
            }
        }
    }

    IEnumerator PerformMultiHitAttack(List<Hit> hits, string hitboxType)
    {
        foreach (var hit in hits)
        {
            PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
            yield return new WaitForSeconds(hit.startup);

            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            GameObject activeHitbox = CreateHitbox(
                hitboxType == "square" ? SquareHitbox : CapsuleHitbox,
                hit.position,
                hit.rotation,
                hit.size * hit.scale
            );
            yield return new WaitForSeconds(hit.active);

            Destroy(activeHitbox);
            PlayerState.State = PlayerStateManager.PossibleStates.Recovering;
            yield return new WaitForSeconds(hit.endlag);
        }

        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }

    IEnumerator PerformProjectileAttack(Attack attack)
    {
        PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
        yield return new WaitForSeconds(attack.hits[0].startup);

        PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
        GameObject projectile = CreateProjectile(
            attack.hitboxType == "square" ? SquareHitbox : CapsuleHitbox,
            attack.hits[0].position,
            attack.hits[0].rotation,
            attack.hits[0].size * attack.hits[0].scale,
            attack.hits[0].speed,
            attack.hits[0].duration,
            attack.hits[0].dropOffRate,
            attack.projectileDir
        );

        PlayerState.State = PlayerStateManager.PossibleStates.Recovering;
        yield return new WaitForSeconds(attack.hits[0].endlag);

        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }

    IEnumerator PerformChargeableProjectileAttack(Attack attack)
    {
        PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
        float chargeTime = 0f;
        while (inputReceiver.IsCharging)
        {
            chargeTime += Time.deltaTime;
            yield return null;
        }

        float maxChargeTime = attack.hits[0].maxChargeTime;
        chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);

        PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
        GameObject projectile = CreateProjectile(
            attack.hitboxType == "square" ? SquareHitbox : CapsuleHitbox,
            attack.hits[0].position,
            attack.hits[0].rotation,
            attack.hits[0].size * attack.hits[0].scale * (1 + chargeTime / maxChargeTime),
            attack.hits[0].speed,
            attack.hits[0].duration,
            attack.hits[0].dropOffRate,
            attack.projectileDir
        );

        PlayerState.State = PlayerStateManager.PossibleStates.Recovering;
        yield return new WaitForSeconds(attack.hits[0].endlag);

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

    GameObject CreateProjectile(GameObject prefab, Vector3 position, Vector3 eulerAngles, Vector3 size, float speed, float duration, float dropOffRate, float projectileDir)
    {
        int playerDir = Player.IsFacingRight ? 1 : -1;
        position = new Vector3(gameObject.transform.position.x + position.x * playerDir, gameObject.transform.position.y + position.y, gameObject.transform.position.z + position.z);
        GameObject instance = Instantiate(prefab, position, Quaternion.Euler(eulerAngles)); // Keep the hitbox rotation independent
        instance.transform.localScale = size;
        instance.tag = gameObject.tag;

        Projectile projectileScript = instance.AddComponent<Projectile>();
        projectileScript.Initialize(speed, duration, playerDir, dropOffRate, projectileDir);

        return instance;
    }
}
