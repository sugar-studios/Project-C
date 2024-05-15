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
        string stateLabel = Player.IsGrounded() ? "ground" : "airial";
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
            StartCoroutine(PerformAttack(selectedAttack));
    }

    private Attack FindAttackInMoveset(List<Attack> attacks, string moveLabel)
    {
        return attacks.Find(attack => attack.moveLabel.Equals(moveLabel, System.StringComparison.OrdinalIgnoreCase));
    }

    IEnumerator PerformAttack(Attack attack)
    {
        PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
        yield return new WaitForSeconds(attack.hits[0].startup);

        PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
        foreach (var hit in attack.hits)
        {
            GameObject activeHitbox = CreateHitbox(
                attack.hitboxType == "square" ? SquareHitbox : CapsuleHitbox,
                hit.position,
                hit.rotation,
                hit.size * hit.scale
            );
            yield return new WaitForSeconds(hit.active);

            Destroy(activeHitbox);
        }

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
}
