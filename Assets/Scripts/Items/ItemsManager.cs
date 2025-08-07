using Cinemachine;
using System.Collections;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    private GameManager GM;

    [Header("Main References")]
    [Space]
    [SerializeField] private GameObject whiteHole;
    [SerializeField] private NewComet[] comets;
    [SerializeField] private PlayerController player;

    [Header("Spawning Settings")]
    [Space]

    [SerializeField] private int numOfCometsToSpawnPerLevelChange;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineVirtualCamera boostAnimationCamera;
    [SerializeField] private float minDistanceFromCamera;
    [SerializeField] private Transform westBound;
    [SerializeField] private Transform eastBound;
    [SerializeField] private Transform northBound;
    [SerializeField] private Transform southBound;

    private float cameraAspect;

    void Awake()
    {
        cameraAspect = mainCamera.aspect;
    }

    IEnumerator Start()
    {
        GM = GameManager.Instance;
        cameraAspect = mainCamera.aspect;

        GM.OnLevelChanged += AddCometToScene;
        GM.OnCutsceneEnded += IntroCutsceneEnded;

        if (player.gameObject.activeInHierarchy)
        {
            yield return null;

            whiteHole.transform.position = GetRandomValidPosition(true);
            whiteHole.SetActive(true);
            AddCometToScene();
        }
    }

    private void OnDisable()
    {
        if (GM != null)
        {
            GM.OnLevelChanged -= AddCometToScene;
            GM.OnCutsceneEnded -= IntroCutsceneEnded;
        }
    }

    private void IntroCutsceneEnded()
    {
        whiteHole.transform.position = GetRandomValidPosition(true);
        whiteHole.SetActive(true);
        AddCometToScene();
    }

    private GameObject GetComet() 
    {
        GameObject comet = null;

        for (int i = 0; i < comets.Length; i++)
        {
            if (!comets[i].gameObject.activeInHierarchy && !comets[i].Used)
            {
                comets[i].BoostAnimationCamera = boostAnimationCamera;
                comet = comets[i].gameObject;
                break;
            }
        }

        return comet;
    }

    private void AddCometToScene() 
    {
        for (int i = 0; i < numOfCometsToSpawnPerLevelChange; i++)
        {
            GameObject comet = GetComet();
            comet.transform.position = GetRandomValidPosition(false);
            comet.SetActive(true);
        }
    }

    private Vector3 GetRandomValidPosition(bool isWhiteHole)
    {
        Vector2 newPosition = new Vector2(Random.Range(westBound.position.x, eastBound.position.x), Random.Range(southBound.position.y, northBound.position.y));
        bool isInCameraView = IsPositionInCameraView(newPosition, isWhiteHole);

        if (!isInCameraView)
            return newPosition;

        else
        {
            while (isInCameraView)
            {
                newPosition = new Vector2(Random.Range(westBound.position.x, eastBound.position.x), Random.Range(southBound.position.y, northBound.position.y));
                isInCameraView = IsPositionInCameraView(newPosition, isWhiteHole);
            }

            return newPosition;
        }
    }

    private bool IsPositionInCameraView(Vector3 position, bool isWhiteHole)
    {
        Vector3 cameraPosition = virtualCamera.transform.position;

        // Calculate camera view bounds in world space
        float cameraHeight = virtualCamera.m_Lens.OrthographicSize * 2f;
        float cameraWidth = cameraHeight * cameraAspect;

        float cameraLeft = cameraPosition.x - (cameraWidth / 2f);
        float cameraRight = cameraPosition.x + (cameraWidth / 2f);
        float cameraBottom = cameraPosition.y - virtualCamera.m_Lens.OrthographicSize;
        float cameraTop = cameraPosition.y + virtualCamera.m_Lens.OrthographicSize;

        // Add padding for minimum distance
        if (isWhiteHole)
        {
            cameraLeft -= minDistanceFromCamera * 2;
            cameraRight += minDistanceFromCamera * 2;
            cameraBottom -= minDistanceFromCamera * 2;
            cameraTop += minDistanceFromCamera * 2;
        }
        else
        {
            cameraLeft -= minDistanceFromCamera;
            cameraRight += minDistanceFromCamera;
            cameraBottom -= minDistanceFromCamera;
            cameraTop += minDistanceFromCamera;
        }

        // Check if position is within extended camera bounds
        bool isInView = position.x >= cameraLeft && position.x <= cameraRight &&
                       position.y >= cameraBottom && position.y <= cameraTop;

        return isInView;
    }
}
