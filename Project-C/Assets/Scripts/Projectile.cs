using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float duration;
    private int direction;
    private float dropOffRate;

    public void Initialize(float speed, float duration, int direction, float dropOffRate)
    {
        this.speed = speed;
        this.duration = duration;
        this.direction = direction;
        this.dropOffRate = dropOffRate;
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float dropOff = dropOffRate * Time.deltaTime;
        transform.Translate(new Vector3(speed * direction * Time.deltaTime, 0, 0)); // Move horizontally
        transform.position += new Vector3(0, -dropOff, 0); // Apply drop-off effect
    }
}
