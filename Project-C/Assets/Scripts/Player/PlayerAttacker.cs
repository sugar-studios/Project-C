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
        PlayerState = GetComponent<PlayerStateManager>();
        Player = GetComponent<PlayerController>();
        inputReceiver = GetComponent<PlayerInputReceiver>();
        jsonReader = GetComponent<JSONReader>();

        currentMoveset = jsonReader.GetMovesetByName(characterName);

        if (currentMoveset == null)
        {
            Debug.LogError("Moveset not found for character: " + characterName);
            return;
        }

        inputReceiver.OnLightAttackEvent += HandleLightAttack;
        inputReceiver.OnHeavyAttackEvent += HandleHeavyAttack;
        inputReceiver.OnTrademarkAttackEvent += HandleTrademarkAttack;
    }

    void OnDestroy()
    {
        inputReceiver.OnLightAttackEvent -= HandleLightAttack;
        inputReceiver.OnHeavyAttackEvent -= HandleHeavyAttack;
        inputReceiver.OnTrademarkAttackEvent -= HandleTrademarkAttack;
    }

    private void HandleLightAttack(bool triggered)
    {
        if (triggered)
            StartAttack(GetAttackLabel("light"), currentMoveset.lightAttack);
    }

    private void HandleHeavyAttack(bool triggered)
    {
        if (triggered)
            StartAttack(GetAttackLabel("heavy"), currentMoveset.heavyAttack);
    }

    private void HandleTrademarkAttack(bool triggered)
    {
        if (triggered)
            StartAttack(GetAttackLabel("trademark"), currentMoveset.trademarkAttack);
    }

    private string GetAttackLabel(string attackType)
    {
        string stateLabel = Player.IsGrounded() ? "ground" : "aerial";
        string direction = "neutral";

        Vector2 inputVector = Player.PlayerInputVector;

        if (Mathf.Abs(inputVector.x) > 0) direction = (inputVector.x > 0) == Player.IsFacingRight ? "forward" : "backward";
        else if (inputVector.y > 0) direction = "upward";
        else if (inputVector.y < 0) direction = "downward";

        if (attackType == "trademark")
        {
            stateLabel = "";
        }

        Debug.Log($"{direction} {stateLabel} {attackType} attack");
        return $"{direction} {stateLabel} {attackType} attack";
    }

    private void StartAttack(string moveLabel, List<Attack> attacks)
    {
        Attack selectedAttack = FindAttackInMoveset(attacks, moveLabel);
        Debug.Log(selectedAttack.name);
        Debug.Log(moveLabel);
        if (selectedAttack != null && PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            if (selectedAttack.attackType == "projectile")
            {
                if (selectedAttack.chargeable)
                {
                    StartCoroutine(PerformChargeableProjectileAttack(selectedAttack));
                }
                else
                {
                    StartCoroutine(PerformProjectileAttack(selectedAttack));
                }
            }
            else
            {
                StartCoroutine(PerformMultiHitAttack(selectedAttack.hits, selectedAttack.hitboxType));
            }
        }
    }

    private Attack FindAttackInMoveset(List<Attack> attacks, string moveLabel)
    {
        return attacks.Find(attack => attack.moveLabel.Equals(moveLabel, System.StringComparison.OrdinalIgnoreCase));
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
        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
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
