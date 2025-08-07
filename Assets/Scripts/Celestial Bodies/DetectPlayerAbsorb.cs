using UnityEngine;

public class DetectPlayerAbsorb : MonoBehaviour
{
    [SerializeField] private CelestialBody controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BHBConstants.PLAYER)) 
        {
            controller.DiveIntoEventHorizon();
        }
    }
}
