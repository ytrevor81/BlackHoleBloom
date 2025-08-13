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
    private SpriteRenderer spriteRenderer;
    
    public CelestialBodyManager Manager { get; set; }
    private Vector3[] simulatedPoints;
    private PlayerController player;
    private Vector2 playerPos;
    private bool inPlayerZone;
    private Vector2 directionToPlayer;
    private Vector2 currentVelocity;
    private Vector2 gravitationalForce;
    private Vector2 simulatedPos;
    private Vector2 simulatedVelocity;
    private Vector2 simulatedDirectionToPlayer;
    private float playerGravitationalForce;
    private float simulatedPlayerGravitationalForce;
    private float orbitDistanceMultiplier;
    private float simulatedOrbitDistanceMultiplier;
    private float increaseSpeedThreshold;
    private float orbitDeclineRate;
    private float speedToPlayer;
    private ArraySegment<Vector3> simulatedPointsSlice;
    private int simCounter;

    // Tracks how far (in array‐indices) we've advanced along simulatedPoints. 
    // e.g. 0.0 means at simulatedPoints[0], 1.0 means at simulatedPoints[1], etc.
    private float pathProgress;

    // A flag to know when to re‐start the animation (so pathProgress resets when simCounter changes).
    private Vector3[] renderPoints;
    private bool shrinking;
    protected Vector3 originalScale;
    protected Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);
    protected float alpha;
    private Color originalColor;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        if (Type != CelestialBodyType.Gas)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;
            originalColor = spriteRenderer.color;
        }
    }

    protected virtual void OnEnable()
    {
        if (Type != CelestialBodyType.Gas)
        {
            transform.localScale = originalScale;
            spriteRenderer.color = originalColor;
        }

        pathProgress = 0f;
        alpha = 1f;
        shrinking = false;
        rb.isKinematic = false;
        inPlayerZone = false;
        lineRenderer.gameObject.SetActive(false);
        currentVelocity = Vector2.zero;
        coll.enabled = true;

        gameObject.tag = BHBConstants.CELESTIAL_BODY;
        playerDetectionCollider.SetActive(true);
    }

    private void CachePlayerIfValid()
    {
        if (player == null)
            player = PlayerController.Instance;

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
        float forceMagnitude = playerGravitationalForce / (distance * distance);
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

        //Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x);

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
            orbitDistanceMultiplier -= Time.fixedDeltaTime * orbitDeclineRate;

            if (orbitDistanceMultiplier < 0)
                orbitDistanceMultiplier = 0;
        }

        float desiredSpeed;

        if (simulated)
        {
            desiredSpeed = Mathf.Sqrt(simulatedPlayerGravitationalForce / effectiveDistance);
        }
        else
        {
            desiredSpeed = Mathf.Sqrt(playerGravitationalForce / effectiveDistance);
        }

        // 3) Split _velocity
        float errorRadial = -Vector2.Dot(_velocity, radialDir);
        float errorTangent = Mathf.Sign(vTangent) * desiredSpeed - vTangent;

        // 5) Apply a gentle PD-style correction

        Vector2 correction = radialDir * (errorRadial * _radialGain) + tangentDir * (errorTangent * _tangentGain);

        // 6) Integrate into your velocity
        _velocity += correction * Time.fixedDeltaTime;

        return _velocity;
    }

    public void MoveToPlayer(CelestialBodySettings settings)
    {
        if (inPlayerZone)
        {
            CachePlayerIfValid();

            rb.velocity = Vector2.zero;
            playerPos = player.transform.position;
            directionToPlayer = playerPos - (Vector2)transform.position;            

            if (shrinking)
            {
                rb.MovePosition(rb.position + (directionToPlayer.normalized * settings.ShrinkSpeedToPlayer) * Time.fixedDeltaTime);
                return;
            }

            gravitationalForce = CalculateGravitationalForce(directionToPlayer);
            currentVelocity += (gravitationalForce + directionToPlayer) * Time.fixedDeltaTime;
            currentVelocity = NudgeTowardsOrbit(currentVelocity, directionToPlayer, settings.RadialGain, settings.TangentGain, simulated: false);

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
    }

    protected virtual void Shrink(CelestialBodySettings _settings)
    {
        alpha -= _settings.FadeOutSpeed * Time.fixedDeltaTime;
        spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, _settings.ShrinkSpeed * Time.fixedDeltaTime);

        if (alpha <= 0f)
            AbsorbIntoPlayer();
    }


    public void AnimateLine(CelestialBodySettings settings)
    {
        if (shrinking)
        {
            Shrink(settings);
            return;
        }
        else if (!inPlayerZone) // || Type == CelestialBodyType.Gas
            return;

        // If we have fewer than 2 points, nothing to draw/animate
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
        if ((!inPlayerZone && !shrinking)) //Type == CelestialBodyType.Gas
            return;

        simulatedPos = rb.position;
        simulatedVelocity = currentVelocity;

        simCounter = 0;

        if (simulatedPoints == null || simulatedPoints.Length != settings.TrajectorySteps)
            simulatedPoints = new Vector3[settings.TrajectorySteps];

        simulatedOrbitDistanceMultiplier = orbitDistanceMultiplier;
        simulatedPlayerGravitationalForce = playerGravitationalForce;

        for (int i = 0; i < settings.TrajectorySteps; i++)
        {
            simCounter++;

            if (i == 0)
            {
                simulatedPoints[i] = new Vector3(rb.position.x, rb.position.y, 0f);
                continue;
            }

            simulatedDirectionToPlayer = playerPos - simulatedPos;

            simulatedVelocity += (CalculateGravitationalForce(simulatedDirectionToPlayer) + simulatedDirectionToPlayer) * settings.TrajectoryDeltaTime;
            simulatedVelocity = NudgeTowardsOrbit(simulatedVelocity, simulatedDirectionToPlayer, settings.RadialGain, settings.TangentGain, simulated: true);
            
            simulatedPos += simulatedVelocity * settings.TrajectoryDeltaTime;            
            simulatedPoints[i] = new Vector3(simulatedPos.x, simulatedPos.y, 0f);

            float distance = Vector2.Distance(playerPos, simulatedPos);
            
            if (distance < 7f)
                break;
        }
        
        if (simCounter != settings.TrajectorySteps)
            simulatedPointsSlice = new ArraySegment<Vector3>(simulatedPoints, 0, simCounter);
    }

    public void DiveIntoEventHorizon()
    {
        playerDetectionCollider.SetActive(false);
        lineRenderer.gameObject.SetActive(false);
        shrinking = true;
    }

    protected virtual void AbsorbIntoPlayer()
    {
        player.AddMass(Manager.GetMass(Type));
        player.RemoveObjectFromOrbitingList(this);
        Manager.RemoveCelestialBodyFromActiveSet(this);
        Manager.PerformAbsorbBehavior(Type, entry);
        inPlayerZone = false;
        shrinking = false;
        lineRenderer.positionCount = 0;
        lineRenderer.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        CachePlayerIfValid();
        player.RemoveObjectFromOrbitingList(this);
        Manager.RemoveCelestialBodyFromActiveSet(this);
        lineRenderer.positionCount = 0;
        lineRenderer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public virtual void EnterOrbit()
    {
        CachePlayerIfValid();

        rb.isKinematic = true;
        orbitDistanceMultiplier = Manager.NewOrbitMultiplier(Type);
        playerGravitationalForce = player.GravitationalForce;

        orbitDeclineRate = Manager.OrbitDeclineRate(Type);
        increaseSpeedThreshold = Manager.GetMinOrbitMultiplier(Type);
        gameObject.tag = BHBConstants.NULL;
        coll.enabled = false;
        inPlayerZone = true;
        Manager.MakeCelestialBodyMoveable(this);
        rb.velocity = Vector2.zero;
        lineRenderer.gameObject.SetActive(true);
    }

    public void HitBarrier(Vector2 contactPoint)
    {
        if (inPlayerZone)
            return;
    }
}
