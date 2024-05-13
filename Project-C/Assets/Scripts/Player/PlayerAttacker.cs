using UnityEngine;
using System.Collections;

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
            StartCoroutine(PerformAttack(attack));
        }
    }

    IEnumerator PerformAttack(Attack attack)
    {
        PlayerState.State = PlayerStateManager.PossibleStates.PreparingAttack;
        yield return new WaitForSeconds(attack.startup);

        PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
        GameObject activeHitbox = CreateHitbox(
            attack.hitboxType == "square" ? SquareHitbox : CapsuleHitbox,
            attack.position,
            attack.rotation,
            attack.size * attack.scale
        );
        yield return new WaitForSeconds(attack.active);

        Destroy(activeHitbox);
        PlayerState.State = PlayerStateManager.PossibleStates.Recovering;
        yield return new WaitForSeconds(attack.endlag);

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
