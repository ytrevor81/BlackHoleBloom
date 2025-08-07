using System;
using System.Collections;
using UnityEngine;

public class Comet : MonoBehaviour, IGravityInteract, IBarrierInteract
{
    private Animator animator;
    private Rigidbody2D rb;
    private OffscreenArrow offscreenArrow;
    [SerializeField]private TrailRenderer trail;
    
    //[SerializeField] private GameObject[] cometChunks;
    [SerializeField] private Transform explodeParticle;
    [SerializeField] private Transform cometCore;
    [SerializeField] private float coreRotationSpeed;
    [SerializeField] private float wholeRotationSpeed;
    [SerializeField] private float travelSpeed;
    [SerializeField] private float speedToPlayer;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private SpriteRenderer[] coreRenderers;
    [SerializeField] private ParticleSystem goToPlayerParticles;
    private PlayerController player;
    private bool inPlayerZone;
    private float coreRotation;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    public bool Used { get; private set; }

    [Space]
    
    [SerializeField] private float shrinkSpeedToPlayer;
    [SerializeField] private float burstSpeed;
    [SerializeField] private float fadeOutSpeed;


    
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnDisable()
    {
        if (offscreenArrow != null)
            offscreenArrow.RemoveTargetFromOffscreenArrow(transform);
    }

    void Start()
    {
        player = PlayerController.Instance;
        offscreenArrow = OffscreenArrow.Instance;
        offscreenArrow.InitializeTargetForOffscreenArrow(transform);

        Vector2 randomDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        rb.AddForce(randomDir * travelSpeed, ForceMode2D.Impulse);

        float targetRotation = Mathf.Atan2(randomDir.y, randomDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);

        Used = true;
    }
    void FixedUpdate()
    {
        if (inPlayerZone)
        {
            Vector2 travelDirection = (player.transform.position - transform.position).normalized;
            rb.AddForce(travelDirection * shrinkSpeed, ForceMode2D.Force);
        }
        
    }

    void Update()
    {
        if (!inPlayerZone)
        {
            coreRotation -= coreRotationSpeed * Time.deltaTime;
            cometCore.rotation = Quaternion.Euler(0, 0, coreRotation);
        }
    }
    
    public void GiveBoost()
    {
        player.EnterBoostMode();
        gameObject.SetActive(false);
    }

    public void EnterOrbit()
    {
        offscreenArrow.RemoveTargetFromOffscreenArrow(transform);
        inPlayerZone = true;
        gameObject.tag = BHBConstants.NULL;

        // for (int i=0; i < cometChunks.Length; i++)
        // {
        //     cometChunks[i].transform.SetParent(null);
        //     cometChunks[i].SetActive(true);
        // }

        //explodeParticle.position = cometCore.position;
        //explodeParticle.SetParent(null);
        //explodeParticle.gameObject.SetActive(true);

        // coreRenderers[0].enabled = false;
        // coreRenderers[1].enabled = false;
        //rb.isKinematic = true;
        //rb.velocity = Vector2.zero;

        rb.mass = 0.3f;
        rb.drag = 3.5f;
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
