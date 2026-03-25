using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Ignore guns and other bullets
        if (collision.gameObject.layer == LayerMask.NameToLayer("Gun")) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet")) return;

        ShootingTarget target = collision.gameObject.GetComponent<ShootingTarget>();
        if (target != null) target.TakeHit();

        Destroy(gameObject);
    }
}