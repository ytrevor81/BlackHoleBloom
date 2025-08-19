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

    [Space]
    [SerializeField] private Transform lesserLevelWestBound;
    [SerializeField] private Transform lesserLevelEastBound;
    [SerializeField] private Transform lesserLevelNorthBound;
    [SerializeField] private Transform lesserLevelSouthBound;

    [Space]

    [SerializeField] private Transform midLevelWestBound;
    [SerializeField] private Transform midLevelEastBound;
    [SerializeField] private Transform midLevelNorthBound;
    [SerializeField] private Transform midLevelSouthBound;

    [Space]

    [SerializeField] private Transform maxLevelWestBound;
    [SerializeField] private Transform maxLevelEastBound;
    [SerializeField] private Transform maxLevelNorthBound;
    [SerializeField] private Transform maxLevelSouthBound;
    

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
        if (GM.CurrentLevel == GameManager.Level.Level4)
            numOfCometsToSpawnPerLevelChange += 1;
            
        for (int i = 0; i < numOfCometsToSpawnPerLevelChange; i++)
        {
            GameObject comet = GetComet();
            comet.transform.position = GetRandomValidPosition(false);
            comet.SetActive(true);
        }
    }

    private bool PlayerIsAtLesserLevel()
    {
        return GM.CurrentLevel == GameManager.Level.Level1 ||
               GM.CurrentLevel == GameManager.Level.Level2 ||
               GM.CurrentLevel == GameManager.Level.Level3;
    }
    private bool PlayerIsAtMidLevel()
    {
        return GM.CurrentLevel == GameManager.Level.Level4 ||
               GM.CurrentLevel == GameManager.Level.Level5 ||
               GM.CurrentLevel == GameManager.Level.Level6;
    }

    private Transform GetDirectionalBoundary(int _dir) //0 = west, 1 = east, 2 = north, 3 = south
    {
        if (PlayerIsAtLesserLevel())
        {
            if (_dir == 0)
                return lesserLevelWestBound;

            else if (_dir == 1)
                return lesserLevelEastBound;

            else if (_dir == 2)
                return lesserLevelNorthBound;

            else
                return lesserLevelSouthBound;
        }
        else if (PlayerIsAtMidLevel())
        {
            if (_dir == 0)
                return midLevelWestBound;

            else if (_dir == 1)
                return midLevelEastBound;

            else if (_dir == 2)
                return midLevelNorthBound;

            else
                return midLevelSouthBound;
        }
        else
        {

            if (_dir == 0)
                return maxLevelWestBound;

            else if (_dir == 1)
                return maxLevelEastBound;

            else if (_dir == 2)
                return maxLevelNorthBound;

            else
                return maxLevelSouthBound;
        }
    }

    private Vector3 GetRandomValidPosition(bool isWhiteHole)
    {
        Transform westBound;
        Transform eastBound;
        Transform northBound;
        Transform southBound;

        if (isWhiteHole)
        {
            westBound = maxLevelWestBound;
            eastBound = maxLevelEastBound;
            northBound = maxLevelNorthBound;
            southBound = maxLevelSouthBound;
        }
        else
        {
            westBound = GetDirectionalBoundary(0);
            eastBound = GetDirectionalBoundary(1);
            northBound = GetDirectionalBoundary(2);
            southBound = GetDirectionalBoundary(3);
        }

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
