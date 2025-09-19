using UnityEngine;

public class FormationGravityArea : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SplitClone splitClone;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BHBConstants.CELESTIAL_BODY))
        {
            if (collision.TryGetComponent(out CelestialBody celestialBody))
            {
                if (playerController != null)
                    celestialBody.EnterOrbitOfPlayer(_targetOrbit: TargetOrbit());

                else
                    celestialBody.EnterOrbitOfClone(_targetOrbit: TargetOrbit());
            }
        }
        else if (collision.CompareTag(BHBConstants.ITEM))
        {
            if (collision.TryGetComponent(out IGravityInteract _gravityInteract))
            {
                if (playerController != null)
                    _gravityInteract.EnterOrbitOfPlayer(_targetOrbit: TargetOrbit());
                
                else
                    _gravityInteract.EnterOrbitOfClone(_targetOrbit: TargetOrbit());
            }
        }
    }

    private Transform TargetOrbit()
    {
        if (playerController != null)
            return playerController.transform;

        else
            return splitClone.transform;
    }
}
