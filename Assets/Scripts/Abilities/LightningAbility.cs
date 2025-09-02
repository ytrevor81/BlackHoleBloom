using UnityEngine;

public class LightningAbility : MonoBehaviour
{
    private enum CurrentState
    {
        Cooldown,
        ChargeUp,
        Active,
        ChargeDown
    }
    private CurrentState currentState;
    private LineRenderer lineRenderer;

    [Header("MAIN REFS")]
    [Space]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CelestialBodyManager celestialBodyManager;
    [SerializeField] private Transform lightningVFXExplosion;
    [SerializeField] private Transform lightningVFXTravelToPlayer;
    [SerializeField] private ParticleSystem lightningParticle;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private VCHelper vcHelper;

    [Header("SETTINGS")]
    [Space]
    [SerializeField] private float travelToPlayerTime;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float visualChargeUpTime;
    [SerializeField] private float visualChargeDownTime;
    [SerializeField] private float detectRadius;
    private bool particleReachedPlayer;
    private ParticleSystem lightningVFXTravelToPlayerPS;
    private Collider2D[] objectsDetected = new Collider2D[10];
    private Material lineMaterial;
    private Transform lightningParticleTrans;
    private ParticleSystem.MainModule lightningParticleMain;
    private Color originalStartColor;
    private Color targetStartColor;
    private Vector3 targetScale;
    private Vector3 startingScale;
    private float timer;
    private float elaspedTime;
    private float particleTravelElaspedTime;
    private static int BOLT_ALPHA = Shader.PropertyToID("_Alpha");
    private CelestialBody targetLogic;
    private Vector3 targetPos;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lightningVFXTravelToPlayerPS = lightningVFXTravelToPlayer.GetComponent<ParticleSystem>();
        lineMaterial = lineRenderer.material;

        lightningParticleMain = lightningParticle.main;
        lightningParticleTrans = lightningParticle.transform;

        targetStartColor = lightningParticleMain.startColor.color;
        originalStartColor = new Color(targetStartColor.r, targetStartColor.g, targetStartColor.b, 0f);
        targetScale = lightningParticleTrans.localScale;
        startingScale = targetScale * 0.1f; // Start at 10% size
        lightningParticleTrans.localScale = startingScale;
    }

    void OnEnable()
    {
        lightningParticleTrans.localScale = startingScale;
        lightningParticleMain.startColor = originalStartColor;
        currentState = CurrentState.Cooldown;
        timer = cooldownTime;
    }

    private void ManageCooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            currentState = CurrentState.ChargeUp;
            elaspedTime = 0;
        }
    }
    private void ManageChargeUp()
    {
        elaspedTime += Time.deltaTime;

        float lerpProgress = elaspedTime / visualChargeUpTime;
        lightningParticleTrans.localScale = Vector3.Lerp(startingScale, targetScale, lerpProgress);
        lightningParticleMain.startColor = Color.Lerp(originalStartColor, targetStartColor, lerpProgress);

        if (lerpProgress >= 1f)
        {
            currentState = CurrentState.Active;
            lineRenderer.positionCount = 0;
            lineMaterial.SetFloat(BOLT_ALPHA, 1f);
            lightningParticleTrans.localScale = targetScale;
            lightningParticleMain.startColor = targetStartColor;
        }
    }
    private void ManageLightningStrike()
    {
        int numDetected = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, objectsDetected, layerMask);

        if (numDetected < 1)
            return;

        targetLogic = null;

        for (int i = 0; i < numDetected; i++)
        {
            if (IsPositionInCameraView(objectsDetected[i].transform.position))
            {
                targetLogic = objectsDetected[i].transform.GetComponent<CelestialBody>();
                break;
            }
        }

        if (targetLogic != null)
        {
            currentState = CurrentState.ChargeDown;
            targetPos = targetLogic.transform.position;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, targetPos);

            elaspedTime = 0f;

            lightningVFXExplosion.position = targetPos;
            lightningVFXExplosion.gameObject.SetActive(true);
            lightningVFXTravelToPlayer.position = targetPos;
            lightningVFXTravelToPlayer.gameObject.SetActive(true);

            targetLogic.HitByLightning();
            particleTravelElaspedTime = 0f;
            particleReachedPlayer = false;
        }   
    }

    private void ParticleTravelToPlayer()
    {
        if (particleReachedPlayer)
            return;

        particleTravelElaspedTime += Time.deltaTime;
        lightningVFXTravelToPlayer.position = Vector3.Lerp(targetPos, transform.position, particleTravelElaspedTime / travelToPlayerTime);

        if (particleTravelElaspedTime >= travelToPlayerTime)
        {
            lightningVFXTravelToPlayerPS.Stop();
            playerController.AddMass(celestialBodyManager.GetMass(targetLogic.Type));
            celestialBodyManager.PerformAbsorbBehavior(targetLogic.Type, null, playSFX: false);
            particleReachedPlayer = true;
            targetLogic = null;
        }
    }
    private void ManageChargeDown()
    {
        elaspedTime += Time.deltaTime;

        float lerpProgress = elaspedTime / visualChargeDownTime;
        lightningParticleTrans.localScale = Vector3.Lerp(targetScale, startingScale, lerpProgress);
        lightningParticleMain.startColor = Color.Lerp(targetStartColor, originalStartColor, lerpProgress);

        float alpha = Mathf.Lerp(1f, 0f, lerpProgress);
        lineMaterial.SetFloat(BOLT_ALPHA, alpha);

        lineRenderer.SetPosition(0, transform.position);

        if (lerpProgress >= 1f)
        {
            currentState = CurrentState.Cooldown;
            lightningParticleTrans.localScale = startingScale;
            lightningParticleMain.startColor = originalStartColor;
            lineMaterial.SetFloat(BOLT_ALPHA, 0);
        }
    }

    void Update()
    {
        if (targetLogic != null)
            ParticleTravelToPlayer();

        if (currentState == CurrentState.Cooldown)
            ManageCooldown();

        else if (currentState == CurrentState.ChargeUp)
            ManageChargeUp();

        else if (currentState == CurrentState.Active)
            ManageLightningStrike();

        else if (currentState == CurrentState.ChargeDown)
            ManageChargeDown();
    }

    private bool IsPositionInCameraView(Vector3 position)
    {
        if (vcHelper == null)
            return false;

        VCHelperVectors vcVectors = vcHelper.GetVCVectorsAtRuntime();

        // Calculate camera view bounds using VCHelperVectors
        float cameraLeft = vcVectors.center.x - (vcVectors.bounds.x / 2f);
        float cameraRight = vcVectors.center.x + (vcVectors.bounds.x / 2f);
        float cameraBottom = vcVectors.center.y - (vcVectors.bounds.y / 2f);
        float cameraTop = vcVectors.center.y + (vcVectors.bounds.y / 2f);

        // Add padding for minimum distance
        float minDistanceFromCamera = 20f;
        cameraLeft -= minDistanceFromCamera;
        cameraRight += minDistanceFromCamera;
        cameraBottom -= minDistanceFromCamera;
        cameraTop += minDistanceFromCamera;

        // Check if position is within extended camera bounds
        bool isInView = position.x >= cameraLeft && position.x <= cameraRight &&
                       position.y >= cameraBottom && position.y <= cameraTop;

        return isInView;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
