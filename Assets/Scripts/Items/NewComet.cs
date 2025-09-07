using System.Collections;
using Cinemachine;
using UnityEngine;

public class NewComet : MonoBehaviour, IGravityInteract, IBarrierInteract
{
    private GameManager GM;
    private HUDController HUD;
    private Collider2D coll;
    private Animator animator;
    private Rigidbody2D rb;
    private OffscreenArrow offscreenArrow;
    private PlayerController player;
    public CinemachineVirtualCamera BoostAnimationCamera { get; set; }
    public bool Used { get; private set; }

    [Header("MAIN REFS")]
    [Space]
    [SerializeField] private CodexEntry codexEntry;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private GameObject explodeParticle;
    [SerializeField] private ParticleSystem goToPlayerParticle;
    [SerializeField] private SpriteRenderer[] coreRenderers;
    [SerializeField] private Transform cometCore;

    [Header("SPEEDS")]
    [Space]
    [SerializeField] private float coreRotationSpeed;
    [SerializeField] private float travelSpeed;
    [SerializeField] private float particleToPlayerLerpTime;
    [SerializeField] private float deactivateAfterParticleWentToPlayerTime;
    private float elapsedTime;
    private bool inPlayerZone;
    private float coreRotation;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private bool particleTravellingToPlayer;
    private WaitForSeconds delayAnimation = new WaitForSeconds(0.5f);
    private IEnumerator currentCoroutine;
    private Vector2 cachedVelocity;
    private bool paused;
    private bool triggeredFirstCometAnimation;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    void OnDisable()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (offscreenArrow != null)
            offscreenArrow.RemoveTargetFromOffscreenArrow(transform);
    }

    void Start()
    {
        GM = GameManager.Instance;
        HUD = HUDController.Instance;
        player = PlayerController.Instance;
        offscreenArrow = OffscreenArrow.Instance;
        offscreenArrow.InitializeTargetForOffscreenArrow(transform);

        Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
        rb.AddForce(dirToPlayer * travelSpeed, ForceMode2D.Impulse);

        float targetRotation = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    void Update()
    {
        if (!inPlayerZone)
        {
            coreRotation -= coreRotationSpeed * Time.deltaTime;
            cometCore.rotation = Quaternion.Euler(0, 0, coreRotation);
        }

        if (particleTravellingToPlayer)
        {
            elapsedTime += Time.deltaTime;
            goToPlayerParticle.transform.position = Vector2.Lerp(transform.position, player.transform.position, elapsedTime / particleToPlayerLerpTime);

            if (elapsedTime >= particleToPlayerLerpTime)
            {
                particleTravellingToPlayer = false;
                elapsedTime = 0;
                goToPlayerParticle.Stop();
                HUD.FadeInHUD();
                HUD.CheckCodexEntry(codexEntry);
                player.EnterBoostMode();

                currentCoroutine = DelayDeactivatingObject();
                StartCoroutine(currentCoroutine);
            }
        }

        HandlePausedStateDuringSpecialAnimations();
    }

    private void HandlePausedStateDuringSpecialAnimations()
    {
        if (!paused && !inPlayerZone && GM.CutscenePlaying)
        {
            cachedVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
            paused = true;
        }
        else if (paused && !inPlayerZone && !GM.CutscenePlaying)
        {
            rb.velocity = cachedVelocity;
            paused = false;
        }
    }

    public void EnterOrbitOfPlayer(bool isRealPlayer)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        inPlayerZone = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        Used = true;
        coll.enabled = false;

        offscreenArrow.RemoveTargetFromOffscreenArrow(transform);
        gameObject.tag = BHBConstants.NULL;
        trail.emitting = false;

        if (!GM.CometAnimationPlayedFirstTime)
        {
            triggeredFirstCometAnimation = true;
            HUD.FadeOutHUD();
            GM.CometAnimationPlayedFirstTime = true;
            GM.CutscenePlaying = true;
            
            float difference = player.transform.position.x - transform.position.x;

            if (difference > 0)
            {
                BoostAnimationCamera.m_Lens.Dutch = 50f;
            }
            else
            {
                BoostAnimationCamera.m_Lens.Dutch = -50f;
            }

            BoostAnimationCamera.Follow = transform;
            BoostAnimationCamera.gameObject.SetActive(true);

            currentCoroutine = DelayStartingAnimation();
            StartCoroutine(currentCoroutine);
        }
        else
        {
            animator.speed = 1.5f;
            animator.enabled = true;
        }
    }
    public void EnterOrbitOfOtherCelestialBody(CelestialBody celestialBody, Collider2D _collider)
    {
        //do nothing, this comet does not interact with other celestial bodies
    }

    private IEnumerator DelayStartingAnimation()
    {
        yield return delayAnimation;
        animator.enabled = true;
    }
    private IEnumerator DelayDeactivatingObject()
    {
        yield return delayAnimation;

        if (triggeredFirstCometAnimation)
            GM.CutscenePlaying = false;
            
        gameObject.SetActive(false);
    }

    public void BurstFinished_AnimationEvent()
    {
        animator.enabled = false;
        cometCore.gameObject.SetActive(false);
        explodeParticle.SetActive(true);
        goToPlayerParticle.gameObject.SetActive(true);
        particleTravellingToPlayer = true;
        BoostAnimationCamera.gameObject.SetActive(false);
    }

    public void HitBarrier(Vector2 contactPoint)
    {
        if (inPlayerZone)
            return;

        StartCoroutine(NewReflectedRotation());
    } 

    private IEnumerator NewReflectedRotation()
    {
        yield return waitForFixedUpdate;

        if (!inPlayerZone)
        {
            float targetRotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        }
    }
}
