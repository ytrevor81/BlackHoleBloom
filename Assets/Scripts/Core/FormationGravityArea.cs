using UnityEngine;

public class FormationGravityArea : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SplitClone splitClone;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BHBConstants.CELESTIAL_BODY))
        {
            if (playerController != null)
                playerController.AddObjectToOrbitingList(collision);

            else
                splitClone.AddObjectToOrbitingList(collision);
        }
        else if (collision.CompareTag(BHBConstants.ITEM))
        {
            if (collision.TryGetComponent(out IGravityInteract _gravityInteract))
            {
                if (playerController != null)
                    _gravityInteract.EnterOrbitOfPlayer(isRealPlayer: true);

                else
                    _gravityInteract.EnterOrbitOfPlayer(isRealPlayer: false);
            }
        }
    }
}
