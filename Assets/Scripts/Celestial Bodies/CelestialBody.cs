using System;
using UnityEngine;
using static CelestialBodySettings;

public class CelestialBody : MonoBehaviour, IGravityInteract, IBarrierInteract
{
    protected Rigidbody2D rb;
    private Collider2D coll;
    [field: SerializeField] public CelestialBodyType Type { get; private set; }
    [SerializeField] private CodexEntry entry;
    [SerializeField] private GameObject playerDetectionCollider;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private bool omitAlphaDecrease;

    [Space]

    [field: SerializeField] public GameObject CelestialBodyFinderCollider;

    private SpriteRenderer spriteRenderer;
    public CelestialBodyManager Manager { get; set; }
    private Vector3[] simulatedPoints;
    protected Vector2 targetPos;
    protected Transform targetTransform;
    private bool inTargetOrbit;
    private Vector2 currentVelocity;
    private Vector2 gravitationalForce;
    private float orbitDistanceMultiplier;
    private float simulatedOrbitDistanceMultiplier;
    private float increaseSpeedThreshold;
    private float orbitDeclineRate;
    private float speedToPlayer;
    private CelestialBody targetCelestialBodyLogic;
    private Transform targetCelestialBodyTrans;
    private ArraySegment<Vector3> simulatedPointsSlice;
    private int simCounter;
    private static float GRAVITATIONAL_CONSTANT = 10000f;
    private float pathProgress;
    private Vector3[] renderPoints;
    private bool shrinking;
    protected Vector3 originalScale;
    protected Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Color originalColor;
    private bool isInOrbit;
    protected float elaspedTime;
    private Color targetColor;
    protected Vector3 startingPos;
    public bool GoingToClone { get; private set;  }
    private float elaspedTimeToClone;
    private bool overrideOrbitBehavior;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        if (Type != CelestialBodyType.Tier1)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;
            originalColor = spriteRenderer.color;
            targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
    }

    protected virtual void OnEnable()
    {
        if (Type != CelestialBodyType.Tier1)
        {
            transform.localScale = originalScale;
            spriteRenderer.color = originalColor;
        }

        rb.isKinematic = false;
    }

    void OnDisable()
    {
        if (coll != null)
            coll.enabled = false;

        isInOrbit = false;
        inTargetOrbit = false;
        targetCelestialBodyLogic = null;
        targetCelestialBodyTrans = null;
        GoingToClone = false;
    }

    public void BlockAndResetMovementVars()
    {
        overrideOrbitBehavior = true;

        isInOrbit = false;
        inTargetOrbit = false;
        targetCelestialBodyLogic = null;
        targetCelestialBodyTrans = null;
        GoingToClone = false;
        targetCelestialBodyLogic = null;
        targetCelestialBodyTrans = null;
        targetTransform = null;

        elaspedTime = 0f;
        elaspedTimeToClone = 0f;
        pathProgress = 0f;
        shrinking = false;
        inTargetOrbit = false;
        lineRenderer.gameObject.SetActive(false);
        currentVelocity = Vector2.zero;
        coll.enabled = false;
    }

    public void UnblockMovementVars()
    {
        gameObject.tag = BHBConstants.CELESTIAL_BODY;
        playerDetectionCollider.SetActive(true);
        coll.enabled = true;
        overrideOrbitBehavior = false;
    }

    public void InitialBoost(CelestialBodySettings settings)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = Vector2.zero;

        float randomSpeed = UnityEngine.Random.Range(settings.InitialSpeedRange.x, settings.InitialSpeedRange.y);
        Vector2 randomDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        rb.AddForce(randomDir.normalized * randomSpeed, ForceMode2D.Impulse);
        rb.AddTorque(UnityEngine.Random.Range(settings.InitialRotationRange.x, settings.InitialRotationRange.y), ForceMode2D.Impulse);
    }

    private Vector2 CalculateGravitationalForce(Vector2 dirToPlayer)
    {
        float distance = dirToPlayer.magnitude;
        float forceMagnitude = GRAVITATIONAL_CONSTANT / (distance * distance);
        return dirToPlayer.normalized * forceMagnitude;
    }

    private Vector2 NudgeTowardsOrbit(Vector2 _velocity, Vector2 _dirToPlayer, float _radialGain, float _tangentGain, bool simulated)
    {
        float r = _dirToPlayer.magnitude;
        Vector2 radialDir = _dirToPlayer / r;

        Vector2 tCCW = new Vector2(-radialDir.y, radialDir.x);
        Vector2 tCW = -tCCW;

        // find how much currentVelocity projects onto each
        float vCCW = Vector2.Dot(_velocity, tCCW);
        float vCW = Vector2.Dot(_velocity, tCW);

        // choose the one that matches the existing direction
        Vector2 tangentDir = (Mathf.Abs(vCCW) >= Mathf.Abs(vCW)) ? tCCW : tCW;
        float vTangent = Vector2.Dot(_velocity, tangentDir);

        // 2) Compute the "standard" orbital speed: v = sqrt(mu / r)
        // Divide by the multiplier to make higher values = wider orbits
        float effectiveDistance;

        if (simulated)
        {
            effectiveDistance = r / simulatedOrbitDistanceMultiplier;
            simulatedOrbitDistanceMultiplier -= Time.fixedDeltaTime * orbitDeclineRate;

            if (simulatedOrbitDistanceMultiplier < 0)
                simulatedOrbitDistanceMultiplier = 0;
        }
        else
        {
            effectiveDistance = r / orbitDistanceMultiplier;

            if (inTargetOrbit)
            {
                orbitDistanceMultiplier -= Time.fixedDeltaTime * orbitDeclineRate;

                if (orbitDistanceMultiplier < 0)
                    orbitDistanceMultiplier = 0;
            }
        }

        float desiredSpeed = Mathf.Sqrt(GRAVITATIONAL_CONSTANT / effectiveDistance);

        // 3) Split _velocity
        float errorRadial = -Vector2.Dot(_velocity, radialDir);
        float errorTangent = Mathf.Sign(vTangent) * desiredSpeed - vTangent;

        // 5) Apply a gentle PD-style correction

        Vector2 correction = radialDir * (errorRadial * _radialGain) + tangentDir * (errorTangent * _tangentGain);

        // 6) Integrate into your velocity
        _velocity += correction * Time.fixedDeltaTime;

        return _velocity;
    }

    private void MoveToPlayer(CelestialBodySettings settings)
    {
        rb.velocity = Vector2.zero;
        targetPos = targetTransform.position;

        if (shrinking)
        {
            Shrink();
            return;
        }

        Vector2 directionToTarget = targetPos - (Vector2)transform.position;
        gravitationalForce = CalculateGravitationalForce(directionToTarget);
        currentVelocity += (gravitationalForce + directionToTarget) * Time.fixedDeltaTime;
        currentVelocity = NudgeTowardsOrbit(currentVelocity, directionToTarget, settings.RadialGain, settings.TangentGain, simulated: false);

        if (orbitDistanceMultiplier < increaseSpeedThreshold)
        {
            speedToPlayer += settings.SpeedIncreaseRate * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + (currentVelocity * speedToPlayer) * Time.fixedDeltaTime);
        }
        else
        {
            speedToPlayer = settings.SpeedToPlayer;
            rb.MovePosition(rb.position + (currentVelocity * settings.SpeedToPlayer) * Time.fixedDeltaTime);
        }
    }

    public void MoveToClone()
    {
        if (!GoingToClone || overrideOrbitBehavior)
            return;

        rb.velocity = Vector2.zero;
        targetPos = targetTransform.position;

        if (shrinking)
        {
            Shrink();
            return;
        }

        elaspedTimeToClone += Time.deltaTime;
        float lerpedProgress = elaspedTimeToClone / 0.75f;
        transform.position = Vector3.Lerp(startingPos, targetPos, lerpedProgress);
    }

    public void MoveToTarget(CelestialBodySettings settings)
    {
        if (GoingToClone || overrideOrbitBehavior)
            return;
            
        if (inTargetOrbit)
            MoveToPlayer(settings);

        else
        {
            if (targetCelestialBodyLogic == null || targetCelestialBodyLogic.inTargetOrbit)
            {
                PlayerController _player = PlayerController.Instance;

                if (targetCelestialBodyLogic != null && targetCelestialBodyLogic.GoingToClone)
                    EnterOrbitOfClone(_player.SplitController.GetCloneTransform());

                else
                    EnterOrbitOfPlayer(_targetOrbit: _player.transform);

                return;
            }

            rb.velocity = Vector2.zero;
            Vector2 directionToTarget = (Vector2)targetCelestialBodyTrans.position - (Vector2)transform.position;

            gravitationalForce = CalculateGravitationalForce(directionToTarget);
            currentVelocity += (gravitationalForce + directionToTarget) * Time.fixedDeltaTime;
            currentVelocity = NudgeTowardsOrbit(currentVelocity, directionToTarget, settings.RadialGain, settings.TangentGain, simulated: false);

            speedToPlayer = settings.SpeedToPlayer;
            rb.MovePosition(rb.position + currentVelocity * settings.SpeedToPlayer * Time.fixedDeltaTime);
        }
    }

    protected virtual void Shrink()
    {
        elaspedTime += Time.fixedDeltaTime;
        float lerpedProgress = elaspedTime / BHBConstants.SHRINK_TO_SINGULARITY_TIME;

        if (!omitAlphaDecrease)
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, lerpedProgress);

        transform.localScale = Vector3.Lerp(originalScale, targetScale, lerpedProgress);
        rb.position = Vector3.Lerp(startingPos, targetPos, lerpedProgress);

        if (elaspedTime >= BHBConstants.SHRINK_TO_SINGULARITY_TIME)
            AbsorbIntoPlayer();
    }


    public void AnimateLine(CelestialBodySettings settings)
    {
        if ((!inTargetOrbit && !shrinking) || GoingToClone || overrideOrbitBehavior)
            return;

        if (simCounter <= 1)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // 1) Advance pathProgress by pathSpeed (units of array‐indices) per second
        pathProgress += settings.PathSpeed * Time.fixedDeltaTime;

        // Clamp so that headIndex never goes beyond simCounter‐1
        float maxProgress = (float)(simCounter - 1);

        if (pathProgress > maxProgress)
            pathProgress = maxProgress;

        // 2) Compute which segment we're on and the fractional inside that segment:
        int headIndex    = Mathf.FloorToInt(pathProgress);              // integer part
        float fracAlong  = pathProgress - (float)headIndex;             // fractional part (0..1)

        // 3) How many points should the renderer draw?
        //    - If we're still between index 0 and 1, draw 2 points: simulatedPoints[0] and an interpolated tip.
        //    - If headIndex = 3 (meaning we've passed simulatedPoints[0..3] completely),
        //      then we want to draw 4 fixed points (0..3) plus one interpolated tip between [3] and [4].
        //    The count is always headIndex+2, except when headIndex == simCounter-1, in which case
        //    there is no "next" point to interpolate with—we just draw simCounter points.
        bool atLastIndex = (headIndex >= simCounter - 1);

        int pointCount = atLastIndex
                       ? simCounter
                       : headIndex + 2;  // (# of fully fixed points) + (# of interpolated tip == 1)

        // 4) Build the positions array
        renderPoints = new Vector3[pointCount];

        // 4a) First copy all entirely "passed" simulatedPoints[0..headIndex-1] into renderPoints[0..headIndex-1].
        //     If headIndex==0, this loop does nothing (there are no fully‐passed points yet).
        for (int i = 0; i < headIndex; i++)
        {
            if (simCounter != settings.TrajectorySteps)
                renderPoints[i] = simulatedPointsSlice.Array[i];

            else
                renderPoints[i] = simulatedPoints[i];
        }

        // 4b) If headIndex < simCounter-1, we have an upcoming segment to interpolate:
        if (!atLastIndex)
        {
            // The "tip" at renderPoints[headIndex] is just the exact simulatedPoints[headIndex],
            // since that point is fully "covered."

            if (simCounter != settings.TrajectorySteps)
            {
                 renderPoints[headIndex] = simulatedPointsSlice.Array[headIndex];
                 // The next slot, renderPoints[headIndex+1], is a smooth lerp between [headIndex]→[headIndex+1]
                Vector3 A = simulatedPointsSlice.Array[headIndex];
                Vector3 B = simulatedPointsSlice.Array[headIndex + 1];
                renderPoints[headIndex + 1] = Vector3.Lerp(A, B, fracAlong);
            }
            else
            {
                renderPoints[headIndex] = simulatedPoints[headIndex];
                // The next slot, renderPoints[headIndex+1], is a smooth lerp between [headIndex]→[headIndex+1]
                Vector3 A = simulatedPoints[headIndex];
                Vector3 B = simulatedPoints[headIndex + 1];
                renderPoints[headIndex + 1] = Vector3.Lerp(A, B, fracAlong);
            }
        }
        else
        {
            // We're at the very last index, so we just copy all points and stop.
            // e.g. headIndex == simCounter-1 => draw all simCounter points exactly.
            for (int i = 0; i < simCounter; i++)
            {
                if (simCounter != settings.TrajectorySteps)
                    renderPoints[i] = simulatedPointsSlice.Array[i];
                    
                else
                    renderPoints[i] = simulatedPoints[i];
            }
        }

        // 5) Finally, push to the LineRenderer
        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(renderPoints);
    }

    public void PlotEventHorizonPath(CelestialBodySettings settings)
    {
        if ((!inTargetOrbit && !shrinking) || GoingToClone || overrideOrbitBehavior)
            return;

        Vector2 simulatedPos = rb.position;
        Vector2 simulatedVelocity = currentVelocity;

        simCounter = 0;

        if (simulatedPoints == null || simulatedPoints.Length != settings.TrajectorySteps)
            simulatedPoints = new Vector3[settings.TrajectorySteps];

        simulatedOrbitDistanceMultiplier = orbitDistanceMultiplier;

        for (int i = 0; i < settings.TrajectorySteps; i++)
        {
            simCounter++;

            if (i == 0)
            {
                simulatedPoints[i] = new Vector3(rb.position.x, rb.position.y, 0f);
                continue;
            }

            Vector2 simulatedDirectionToTarget = targetPos - simulatedPos;

            simulatedVelocity += (CalculateGravitationalForce(simulatedDirectionToTarget) + simulatedDirectionToTarget) * settings.TrajectoryDeltaTime;
            simulatedVelocity = NudgeTowardsOrbit(simulatedVelocity, simulatedDirectionToTarget, settings.RadialGain, settings.TangentGain, simulated: true);
            
            simulatedPos += simulatedVelocity * settings.TrajectoryDeltaTime;            
            simulatedPoints[i] = new Vector3(simulatedPos.x, simulatedPos.y, 0f);

            float distance = Vector2.Distance(targetPos, simulatedPos);
            
            if (distance < 7f)
                break;
        }
        
        if (simCounter != settings.TrajectorySteps)
            simulatedPointsSlice = new ArraySegment<Vector3>(simulatedPoints, 0, simCounter);
    }

    public void DiveIntoEventHorizon()
    {
        if (overrideOrbitBehavior)
            return;

        playerDetectionCollider.SetActive(false);
        lineRenderer.gameObject.SetActive(false);
        startingPos = rb.position;
        shrinking = true;
    }

    public virtual void AbsorbIntoPlayer()
    {
        if (overrideOrbitBehavior)
            return;

        Manager.RemoveCelestialBodyFromActiveSet(this);
        Manager.PerformAbsorbBehavior(Type, entry, playSFX: true);
        targetTransform = null;
        inTargetOrbit = false;
        isInOrbit = false;
        shrinking = false;
        lineRenderer.positionCount = 0;
        lineRenderer.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
    public void HitByLightning()
    {
        if (overrideOrbitBehavior)
            return;

        Manager.RemoveCelestialBodyFromActiveSet(this);
        targetTransform = null;
        inTargetOrbit = false;
        isInOrbit = false;
        shrinking = false;
        lineRenderer.positionCount = 0;
        lineRenderer.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        Manager.RemoveCelestialBodyFromActiveSet(this);
        lineRenderer.positionCount = 0;
        lineRenderer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public virtual void EnterOrbitOfPlayer(Transform _targetOrbit)
    {
        if (inTargetOrbit || overrideOrbitBehavior)
            return;
            
        if (CelestialBodyFinderCollider != null)
            CelestialBodyFinderCollider.SetActive(false);

        targetTransform = _targetOrbit;
        rb.isKinematic = true;
        orbitDistanceMultiplier = Manager.NewOrbitMultiplier(Type);

        orbitDeclineRate = Manager.OrbitDeclineRate(Type);
        increaseSpeedThreshold = Manager.GetMinOrbitMultiplier(Type);
        gameObject.tag = BHBConstants.NULL;
        coll.enabled = false;
        inTargetOrbit = true;
        isInOrbit = true;
        Manager.MakeCelestialBodyMoveable(this);
        rb.velocity = Vector2.zero;
        lineRenderer.gameObject.SetActive(true);
        targetCelestialBodyLogic = null;
        targetCelestialBodyTrans = null;
    }
    public virtual void EnterOrbitOfClone(Transform _targetOrbit)
    {
         if (inTargetOrbit || overrideOrbitBehavior)
            return;

        GoingToClone = true;
        startingPos = rb.position;
        currentVelocity = rb.velocity;
        EnterOrbitOfPlayer(_targetOrbit);
    }

    private bool OmitOrbitingOtherCelestialBody(CelestialBody _celestialBody)
    {
        return Type == CelestialBodyType.Tier1
            || Type == CelestialBodyType.Tier4
            || (Type == CelestialBodyType.Tier2 && _celestialBody.Type == CelestialBodyType.Tier4)
            || (Type == CelestialBodyType.Tier3 && _celestialBody.Type == CelestialBodyType.Tier3)
            || isInOrbit
            || overrideOrbitBehavior;
    }
    public void EnterOrbitOfOtherCelestialBody(CelestialBody celestialBody, Collider2D _collider)
    {
        if (OmitOrbitingOtherCelestialBody(celestialBody))
            return;

        if (CelestialBodyFinderCollider != null)
            CelestialBodyFinderCollider.SetActive(false);

        targetCelestialBodyTrans = celestialBody.transform;
        targetCelestialBodyLogic = celestialBody;
        isInOrbit = true;

        rb.isKinematic = true;
        orbitDistanceMultiplier = Manager.GetOrbitRadiusForOtherCelestialBodies(Type);

        increaseSpeedThreshold = Manager.GetMinOrbitMultiplier(Type);
        Manager.MakeCelestialBodyMoveable(this);
        rb.velocity = Vector2.zero;
    }

    public void HitBarrier(Vector2 contactPoint)
    {
        if (isInOrbit)
            return;
    }
}
