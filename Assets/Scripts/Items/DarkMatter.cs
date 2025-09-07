using System.Collections;
using UnityEngine;

public class DarkMatter : MonoBehaviour, IGravityInteract
{
    private GameManager GM;
    private Collider2D coll;

    [Header("Main Refs")]
    [Space]

    [SerializeField] private Transform cloudParticles;
    [SerializeField] private Transform coreParticles;


    [Header("Level Scaling")]
    [Space]

    [SerializeField] private Vector2 level2ScaleVectors; //x = main and core particles, y = cloud particles
    [SerializeField] private Vector2 level3ScaleVectors;
    [SerializeField] private Vector2 level4ScaleVectors;
    [SerializeField] private Vector2 level5ScaleVectors;
    [SerializeField] private Vector2 level6ScaleVectors;
    [SerializeField] private Vector2 level7ScaleVectors;
    [SerializeField] private Vector2 level8ScaleVectors;
    [SerializeField] private Vector2 level9ScaleVectors;

    private Vector2 targetScaleVectors;

    [Space]

    [SerializeField] private float scaleLerpTime;
    [SerializeField] private float timeToPlayer;
    [SerializeField] private float shrinkTime;

    private Coroutine currentCoroutine;
    private bool isMovingToPlayer;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        isMovingToPlayer = false;
        GM = GameManager.Instance;
        GM.OnLevelChanged += ScaleDarkMatter;
        ScaleDarkMatterImmeditely();
        coll.enabled = true;
    }

    void OnDisable()
    {
        StopCurrentCoroutine();

        if (GM != null)
            GM.OnLevelChanged -= ScaleDarkMatter;
    }

    private void ScaleDarkMatter()
    {
        if (isMovingToPlayer)
            return;

        StopCurrentCoroutine();
        targetScaleVectors = GetCurrentScaleVectors();
        currentCoroutine = StartCoroutine(ScaleCoroutine());
    }

    private Vector2 GetCurrentScaleVectors()
    {
        if (GM.CurrentLevel == GameManager.Level.Level2)
            return level2ScaleVectors;

        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return level3ScaleVectors;

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return level4ScaleVectors;

        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return level5ScaleVectors;

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return level6ScaleVectors;

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return level7ScaleVectors;

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return level8ScaleVectors;

        else if (GM.CurrentLevel == GameManager.Level.Level9)
            return level9ScaleVectors;

        else
            return Vector2.zero;
    }
    private void ScaleDarkMatterImmeditely()
    {
        if (GM.CurrentLevel == GameManager.Level.Level1)
            return;

        targetScaleVectors = GetCurrentScaleVectors();
        transform.localScale = new Vector3(targetScaleVectors.x, targetScaleVectors.x, 1f);
        coreParticles.localScale = new Vector3(targetScaleVectors.x, targetScaleVectors.x, 1f);
        cloudParticles.localScale = new Vector3(targetScaleVectors.y, targetScaleVectors.y, 1f);
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = null;
    }

    private IEnumerator ScaleCoroutine()
    {
        float elaspedTime = 0f;
        Vector3 startingScaleMainAndCoreParticles = transform.localScale;
        Vector3 startingScaleCloudParticles = cloudParticles.localScale;

        Vector3 targetScaleMainAndCoreParticles = new Vector3(targetScaleVectors.x, targetScaleVectors.x, 1f);
        Vector3 targetScaleCloudParticles = new Vector3(targetScaleVectors.y, targetScaleVectors.y, 1f);

        while (elaspedTime < scaleLerpTime)
        {
            elaspedTime += Time.deltaTime;
            float t = elaspedTime / scaleLerpTime;

            transform.localScale = Vector3.Lerp(startingScaleMainAndCoreParticles, targetScaleMainAndCoreParticles, t);
            coreParticles.localScale = Vector3.Lerp(startingScaleMainAndCoreParticles, targetScaleMainAndCoreParticles, t);
            cloudParticles.localScale = Vector3.Lerp(startingScaleCloudParticles, targetScaleCloudParticles, t);
            yield return null;
        }
    }

    private IEnumerator MoveToPlayerCoroutine()
    {
        float elaspedTime = 0f;
        Transform playerPos = PlayerController.Instance.transform;
        Vector3 startingPos = transform.position;

        while (elaspedTime < timeToPlayer)
        {
            elaspedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(startingPos, playerPos.position, elaspedTime / timeToPlayer);
            yield return null;
        }

        elaspedTime = 0;
        Vector3 startingScaleMainAndCoreParticles = transform.localScale;
        Vector3 startingScaleCloudParticles = cloudParticles.localScale;
        Vector3 targetShrinkScaleForEverything = new Vector3(0.1f, 0.1f, 1f);

        while (elaspedTime < shrinkTime)
        {
            elaspedTime += Time.deltaTime;
            float t = elaspedTime / shrinkTime;

            transform.localScale = Vector3.Lerp(startingScaleMainAndCoreParticles, targetShrinkScaleForEverything, t);
            coreParticles.localScale = Vector3.Lerp(startingScaleMainAndCoreParticles, targetShrinkScaleForEverything, t);
            cloudParticles.localScale = Vector3.Lerp(startingScaleCloudParticles, targetShrinkScaleForEverything, t);

            transform.position = playerPos.position;

            yield return null;
        }

        HUDController.Instance.AddTime();
        gameObject.SetActive(false);
    }

    public void EnterOrbitOfPlayer(bool isRealPlayer)
    {
        isMovingToPlayer = true;
        coll.enabled = false;
        StopCurrentCoroutine();
        currentCoroutine = StartCoroutine(MoveToPlayerCoroutine());
    }

    public void EnterOrbitOfOtherCelestialBody(CelestialBody celestialBody, Collider2D _collider)
    {
        //do nothing
    }
}
