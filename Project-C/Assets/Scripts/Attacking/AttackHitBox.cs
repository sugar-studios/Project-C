using UnityEngine;
using System.Collections;

public class AttackHitbox : MonoBehaviour
{
    private Hit hitData;
    private PlayerStateManager attackerState;
    private PlayerController attacker;

    public void Initialize(Hit hitData, PlayerStateManager attackerState, PlayerController attacker)
    {
        this.hitData = hitData;
        this.attackerState = attackerState;
        this.attacker = attacker;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Attack!!");
        if (collision.gameObject.tag != attacker.gameObject.tag && collision.gameObject.layer == LayerMask.NameToLayer("Hurtbox"))
        {
            Debug.Log("Attack Connected");
            PlayerStateManager targetState = collision.GetComponent<PlayerStateManager>();
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (targetState != null)
            {
                ApplyHitEffects(targetState, collision.transform.position);
            }

            Destroy(gameObject);
        }
    }

    private void ApplyHitEffects(PlayerStateManager targetState, Vector3 hitPosition, Rigidbody2D rb)
    {
        targetState.health -= hitData.damage;

        if (hitData.hitstun)
        {
            StartCoroutine(ApplyHitstunAndKnockback(targetState, hitPosition));
        }
        else
        {
            ApplyKnockback(targetState, hitPosition);
        }
    }

    private IEnumerator ApplyHitstunAndKnockback(PlayerStateManager targetState, Vector3 hitPosition)
    {
        targetState.State = PlayerStateManager.PossibleStates.HitStun;
        yield return new WaitForSeconds(hitData.hitstunDuration);
        ApplyKnockback(targetState, hitPosition);
        targetState.State = PlayerStateManager.PossibleStates.PsedeuFree;
        yield return new WaitForSeconds(1.0f);
        targetState.State = PlayerStateManager.PossibleStates.FreeAction;
    }

    private void ApplyKnockback(PlayerStateManager targetState, Vector3 hitPosition)
    {
        Vector2 direction = (hitPosition - attacker.transform.position).normalized;
        float knockbackForce = hitData.isSetKnockback ? hitData.knockback : Mathf.Max(hitData.knockbackScaling * hitData.knockback - targetState.playerWeight, hitData.knockback / 3);

        Rigidbody2D targetRB = targetState.GetComponent<Rigidbody2D>();
        if (targetRB != null)
        {
            targetRB.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
