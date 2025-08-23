using UnityEngine;

public class FormationGravityArea : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BHBConstants.CELESTIAL_BODY))
        {
            playerController.AddObjectToOrbitingList(collision);
        }
        else if (collision.CompareTag(BHBConstants.ITEM))
        {
            if (collision.TryGetComponent(out IGravityInteract _gravityInteract))
                _gravityInteract.EnterOrbitOfPlayer();
        }
    }
}
