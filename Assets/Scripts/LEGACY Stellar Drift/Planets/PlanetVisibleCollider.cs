using Cinemachine;
using UnityEngine;

public class PlanetVisibleCollider : MonoBehaviour
{
    // private BoxCollider2D boxCollider;
    // private int planetLayer;
    // [SerializeField] private Camera mainCamera;
    // [SerializeField] private CinemachineVirtualCamera virtualCamera;

    // private Vector2 center;
    // private Vector2 bounds;

    // void Awake()
    // {
    //     boxCollider = GetComponent<BoxCollider2D>();
    // }
    // void Start()
    // {
    //     planetLayer = LayerMask.NameToLayer(StellarDriftConstants.PLANET);
    // }

    // void Update()
    // {
    //     GetVCVectorsAtRuntime();
    //     transform.position = center;
    //     boxCollider.size = bounds;
    // }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.layer == planetLayer)
    //     {
    //         if (collision.TryGetComponent(out Planet planet))
    //         {
    //             planet.VisibleToPlayer = true;
    //         }
    //     }
    // }

    // void OnTriggerExit2D(Collider2D collision)
    // {
    //     if (collision.gameObject.layer == planetLayer)
    //     {
    //         if (collision.TryGetComponent(out Planet planet))
    //         {
    //             planet.VisibleToPlayer = false;
    //         }
    //     }
    // }

    // private void GetVCVectorsAtRuntime()
    // {
    //     float cameraViewHeight = 2.0f * mainCamera.orthographicSize;
    //     float cameraViewWidth = cameraViewHeight * mainCamera.aspect;

    //     center = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
    //     bounds = new Vector3(cameraViewWidth, cameraViewHeight, 1);
    // }
}
