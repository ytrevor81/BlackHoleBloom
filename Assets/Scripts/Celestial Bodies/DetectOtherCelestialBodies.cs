using UnityEngine;

public class DetectOtherCelestialBodies : MonoBehaviour
{
    [SerializeField] private CelestialBody controller;
    [SerializeField] private Collider2D selfCollider;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BHBConstants.CELESTIAL_BODY) && collision != selfCollider)
        {
            if (collision.TryGetComponent(out CelestialBody celestialBody))
            {
                celestialBody.EnterOrbitOfOtherCelestialBody(controller, collision);
            }
        }
    }
}
