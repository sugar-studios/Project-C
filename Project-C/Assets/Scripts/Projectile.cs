using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float duration;
    private int direction;
    private float dropOffRate;
    private float projectileDir;

    public void Initialize(float speed, float duration, int direction, float dropOffRate, float projectileDir)
    {
        this.speed = speed;
        this.duration = duration;
        this.direction = direction;
        this.dropOffRate = dropOffRate;
        this.projectileDir = projectileDir;
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float dropOff = dropOffRate * Time.deltaTime;
        Vector3 movement = new Vector3(Mathf.Cos(projectileDir * Mathf.Deg2Rad) * speed * Time.deltaTime * direction,
                                       Mathf.Sin(projectileDir * Mathf.Deg2Rad) * speed * Time.deltaTime * direction,
                                       0);
        transform.Translate(movement, Space.World);
        transform.position += new Vector3(0, -dropOff, 0); // Apply drop-off effect
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stage"))
        {
            Destroy(gameObject);
        }
    }
}
