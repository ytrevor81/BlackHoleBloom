using UnityEngine;

public class Barrier : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IBarrierInteract _barrierInteract))
            _barrierInteract.HitBarrier(collision.contacts[0].point);
    }
}
