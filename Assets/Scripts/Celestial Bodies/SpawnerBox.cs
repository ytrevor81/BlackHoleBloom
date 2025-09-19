using UnityEngine;

public class SpawnerBox : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(BHBConstants.CELESTIAL_BODY))
        {
            if (collision.TryGetComponent(out CelestialBody celestialBody))
            {
                celestialBody.ReturnToPool(despawnedByBoundary: true);
            }
        }
    }
}
