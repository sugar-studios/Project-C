using UnityEngine;
using System.Collections;

public class Hitbox : MonoBehaviour
{
    public int priority;
    public Vector3 size;
    public float scale;
    public Vector3 rotation;
    public Vector3 position;
    public float startup;
    public float active;
    public float endlag;
    public int damage;
    public float knockback;
    public bool isSetKnockback;
    public float knockbackScaling;
    public float speed;
    public float duration;
    public float dropOffRate;
    public float maxChargeTime;
    public bool hitstun;
    public float hitstunDuration;
    public string hitFunction;

    private GameObject attacker;

    public void Initialize(Hit hit, GameObject attacker)
    {
        priority = hit.priority;
        size = hit.size;
        scale = hit.scale;
        rotation = hit.rotation;
        position = hit.position;
        startup = hit.startup;
        active = hit.active;
        endlag = hit.endlag;
        damage = hit.damage;
        knockback = hit.knockback;
        isSetKnockback = hit.isSetKnockback;
        knockbackScaling = hit.knockbackScaling;
        speed = hit.speed;
        duration = hit.duration;
        dropOffRate = hit.dropOffRate;
        maxChargeTime = hit.maxChargeTime;
        hitstun = hit.hitstun;
        hitstunDuration = hit.hitstunDuration;
        hitFunction = hit.hitFunction;

        this.attacker = attacker;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != attacker.tag && collision.gameObject.layer == LayerMask.NameToLayer("Hurtbox"))
        {
            PlayerStateManager opponentState = collision.transform.parent.GetComponent<PlayerStateManager>();
            Rigidbody2D opponentRb = collision.transform.parent.GetComponent<Rigidbody2D>();

            if (opponentState != null && opponentRb != null)
            {
                Debug.Log("step 1");
                Debug.Log($"op state: {opponentState}");
                Debug.Log($"op state: {opponentRb.velocity}");
                ApplyHitEffects(opponentState, opponentRb);
                Destroy(gameObject);
            }
        }
    }

    private void ApplyHitEffects(PlayerStateManager opponentState, Rigidbody2D opponentRb)
    {
        // Apply damage
        opponentState.health -= damage;

        // Apply hitstun
        if (hitstun)
        {
            Debug.Log("step 2a");
            //StartCoroutine(HitstunCoroutine(opponentState, opponentRb));
            ApplyKnockback(opponentState, opponentRb);
        }
        else
        {
            Debug.Log("step 2b");
            ApplyKnockback(opponentState, opponentRb);
        }
    }

    private IEnumerator HitstunCoroutine(PlayerStateManager opponentState, Rigidbody2D opponentRb)
    {
        Debug.Log("step 2.5a");
        Debug.Log(hitstunDuration);
        opponentState.State = PlayerStateManager.PossibleStates.HitStun;
        yield return new WaitForSeconds(hitstunDuration);
        ApplyKnockback(opponentState, opponentRb);
    }

    private void ApplyKnockback(PlayerStateManager opponentState, Rigidbody2D opponentRb)
    {
        Debug.Log("step 3");
        float totalKnockback = isSetKnockback ? knockback : knockback * (opponentState.KOScale / 100f)*10000;
        Vector2 knockbackDirection = new Vector2(attacker.transform.localScale.x * Mathf.Cos(rotation.z * Mathf.Deg2Rad), Mathf.Sin(rotation.z * Mathf.Deg2Rad));
        opponentRb.AddForce(knockbackDirection * totalKnockback, ForceMode2D.Impulse);

        // Apply KO scaling
        opponentState.KOScale += knockbackScaling;

        // Transition to pseudo-free state
        opponentState.State = PlayerStateManager.PossibleStates.PsedeuFree;
        StartCoroutine(PseudoFreeCoroutine(opponentState));
    }

    private IEnumerator PseudoFreeCoroutine(PlayerStateManager opponentState)
    {
        Debug.Log("step 4");
        yield return new WaitForSeconds(1f);
        Debug.Log("step 5");
        opponentState.State = PlayerStateManager.PossibleStates.FreeAction;
    }
}
