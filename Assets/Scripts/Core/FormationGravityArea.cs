using UnityEngine;

public class FormationGravityArea : MonoBehaviour
{
    private PlayerController playerController;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BHBConstants.CELESTIAL_BODY))
        {
            if (playerController == null)
                playerController = PlayerController.Instance;

            playerController.AddObjectToOrbitingList(collision);
        }
        else if (collision.CompareTag(BHBConstants.BOOST) || collision.CompareTag(BHBConstants.WHITE_HOLE))
        {
            if (collision.TryGetComponent(out IGravityInteract _gravityInteract))
                _gravityInteract.EnterOrbit();
        }
    }
}
