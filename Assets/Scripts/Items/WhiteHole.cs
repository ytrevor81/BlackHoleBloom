using System.Collections;
using UnityEngine;

public class WhiteHole : MonoBehaviour, IGravityInteract
{
    private GameManager GM;
    protected Rigidbody2D rb;
    private Collider2D coll;
    private PlayerController playerLogic;

    [Header("MAIN REFS")]
    [Space]

    [SerializeField] private CodexEntry codexEntry;
    [SerializeField] private Transform beacon;
    [SerializeField] private float beaconFadeSpeed;
    [SerializeField] private float beaconExpandSpeed;

    [SerializeField] private Animator vfxContainer;
    [SerializeField] private ParticleSystem gasParticle;
    [SerializeField] private ParticleSystem matterParticle;
    [SerializeField] private ParticleSystem matterParticle2;

    private SpriteRenderer beaconSpriteRenderer;
    private float beaconAlpha;
    private Vector2 beaconInitialScale = new Vector2(0.2f, 0.2f);
    private Vector2 beaconTargetScale = new Vector2(10f, 10f);
    private bool beaconFinished;

    [Header("Level Scaling")]
    [Space]

    [SerializeField] private Vector2 level2ScaleVectors; //x = main, y = particles
    [SerializeField] private Vector2 level3ScaleVectors;
    [SerializeField] private Vector2 level4ScaleVectors;
    [SerializeField] private Vector2 level5ScaleVectors;
    [SerializeField] private Vector2 level6ScaleVectors;
    [SerializeField] private Vector2 level7ScaleVectors;
    [SerializeField] private Vector2 level8ScaleVectors;
    [SerializeField] private Vector2 level9ScaleVectors;

    [Header("Stats to Player")]
    [Space]

    [SerializeField] private float sameScaleAsPlayerTime;
    [SerializeField] private float decreaseScaleTime;
    [SerializeField] private int massPerTick;
    [SerializeField] private float timeBetweenTicks;
    [SerializeField] private int numOfTicks;
    private int maxNumOfPoints;
    private float lastTickTime;
    private bool fillStarted;
    private int currentTick;
    private Vector3 initialScale;
    private float scaleLerpStartTime;
    private bool codexEntryChecked;

    [Header("Eject Into Player VFX")]
    [Space]

    [SerializeField] private float finalShapeAngle;
    [SerializeField] private float finalShapeRadius;
    [SerializeField] private float animateSpeed;
    [SerializeField] private float finalSimSpeedMatterLine;
    [SerializeField] private float finalSimSpeedGasLine;
    [SerializeField] private float maxLifetimeMatterLine;
    [SerializeField] private float minLifetimeMatterLine;
    [SerializeField] private float maxLifetimeGasLine;
    [SerializeField] private float minLifetimeGasLine;
    [SerializeField] private float maxDistanceVFX;
    [SerializeField] private float minDistanceVFX;
    private GameManager.Level levelOfInjection;

    private float newSimulationSpeedMatterLine1;
    private float newSimulationSpeedMatterLine2;
    private float newSimulationSpeedGasLine;
    private float newShapeAngleGas;
    private float newShapeAngleMatter;
    private float newShapeAngleMatter2;
    private float newShapeRadiusGas;
    private float newShapeRadiusMatter;
    private float newShapeRadiusMatter2;

    private ParticleSystem.MainModule gasParticleMain;
    private ParticleSystem.ShapeModule gasParticleShape;
    private ParticleSystem.ShapeModule matterParticleShape;
    private ParticleSystem.ShapeModule matterParticle2Shape;
    private ParticleSystem.MainModule matterParticleMain;
    private ParticleSystem.MainModule matterParticle2Main;


    [Space]

    [SerializeField] private float Gravity;
    [SerializeField] private float RadialGain;
    [SerializeField] private float TangentGain;
    [SerializeField] private float SpeedToPlayer;
    [SerializeField] private float PathSpeed;
    [SerializeField] private int TrajectorySteps;
    [SerializeField] private float OrbitRadius;
    private SpriteRenderer spriteRenderer;
    private Transform targetTransform;
    private bool inTargetOrbit;
    private Vector2 currentVelocity;
    private Vector2 gravitationalForce;
    private bool particlesAnimated;
    private bool disappearing;
    private float disappearingTimer;
    private Vector3 targetFinalScale = new Vector3(0.01f, 0.01f, 1f);
    private Vector3 particleInitialScale;
    private IEnumerator currentCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        beaconSpriteRenderer = beacon.GetComponent<SpriteRenderer>();

        gasParticleMain = gasParticle.main;
        gasParticleShape = gasParticle.shape;
        matterParticleShape = matterParticle.shape;
        matterParticle2Shape = matterParticle2.shape;
        matterParticleMain = matterParticle.main;
        matterParticle2Main = matterParticle2.main;

        newShapeAngleGas = gasParticleShape.angle;
        newShapeRadiusGas = gasParticleShape.radius;
        newShapeAngleMatter = matterParticleShape.angle;
        newShapeRadiusMatter = matterParticleShape.radius;
        newShapeAngleMatter2 = matterParticle2Shape.angle;
        newShapeRadiusMatter2 = matterParticle2Shape.radius;
    }

    void Start()
    {
        playerLogic = PlayerController.Instance;
        GM = GameManager.Instance;
        GM.OnLevelChanged += ChangeSize;
    }

    void OnDisable()
    {
        StopCurrentCoroutine();

        if (GM != null)
            GM.OnLevelChanged -= ChangeSize;
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    void Update()
    {
        if (disappearing)
        {
            disappearingTimer += Time.deltaTime;

            if (disappearingTimer >= 1f)
                gameObject.SetActive(false);
        }
        else
        {
            if (inTargetOrbit)
                EjectIntoBlackHole();

            AnimateBeacon();
        }
    }

    private void AnimateBeacon()
    {
        if (beaconFinished)
            return;

        if (inTargetOrbit)
        {
            if (beaconAlpha <= 0)
            {
                beaconFinished = true;
                beacon.gameObject.SetActive(false);
                return;
            }

            beaconAlpha -= beaconFadeSpeed * Time.deltaTime;
            beaconSpriteRenderer.color = new Color(1, 1, 1, beaconAlpha);
            beacon.localScale = Vector2.MoveTowards(beacon.localScale, beaconTargetScale * 2, beaconExpandSpeed * Time.deltaTime);
        }
        else
        {
            if (beaconAlpha <= 0)
            {
                beaconAlpha = 1;
                beacon.localScale = beaconInitialScale;
                beaconSpriteRenderer.color = new Color(1, 1, 1, 1);
            }
            else
            {
                beaconAlpha -= beaconFadeSpeed * Time.deltaTime;
                beaconSpriteRenderer.color = new Color(1, 1, 1, beaconAlpha);
                beacon.localScale = Vector2.MoveTowards(beacon.localScale, beaconTargetScale, beaconExpandSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        if (inTargetOrbit)
            OrbitTarget();
    }

    private Vector2 CalculateGravitationalForce(Vector2 dirToPlayer)
    {
        float distance = dirToPlayer.magnitude;
        float forceMagnitude = Gravity / (distance * distance);
        return dirToPlayer.normalized * forceMagnitude;
    }

    private Vector2 NudgeTowardsOrbit(Vector2 _velocity, Vector2 _dirToPlayer, float _radialGain, float _tangentGain, bool simulated)
    {
        float r = _dirToPlayer.magnitude;
        Vector2 radialDir = _dirToPlayer / r;
        Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x);

        float vTangent = Vector2.Dot(_velocity, tangentDir);
        float vRadial = Vector2.Dot(_velocity, radialDir);

        float requiredSpeed = Mathf.Sqrt(Gravity / r);

        // radial: damp out any in/out motion
        float radialError = -vRadial;
        Vector2 radialCorrection = radialDir * (radialError * _radialGain);

        // tangent: always push toward the correct speed *in your chosen direction*—
        // here we assume tangentDir is the + direction you want
        float tangentError = requiredSpeed - vTangent;
        Vector2 tangentCorrection = tangentDir * (tangentError * _tangentGain);

        // integrate
        _velocity += (radialCorrection + tangentCorrection) * Time.fixedDeltaTime;
        return _velocity;
    }

    private void OrbitTarget()
    {
        rb.velocity = Vector2.zero;
        Vector3 directionToTarget = targetTransform.position - transform.position;
        // Apply gravitational force
        gravitationalForce = CalculateGravitationalForce(directionToTarget);
        currentVelocity += gravitationalForce * Time.fixedDeltaTime;

        // Apply orbital correction to maintain circular orbit
        currentVelocity = NudgeTowardsOrbit(currentVelocity, directionToTarget, RadialGain, TangentGain, simulated: false);
        // Distance constraint: gently pull back if too far, push away if too close
        float currentDistance = directionToTarget.magnitude;
        float distanceError = OrbitRadius - currentDistance;

        if (Mathf.Abs(distanceError) > 0.5f) // Only apply correction if significantly off
        {
            Vector2 distanceCorrection = directionToTarget.normalized * (distanceError * 0.1f); // Gentle correction
            currentVelocity += distanceCorrection * Time.fixedDeltaTime;
        }
        // Ensure minimum velocity to prevent stopping
        float minSpeed = 2.0f; // Adjust this value as needed
        if (currentVelocity.magnitude < minSpeed)
        {
            currentVelocity = currentVelocity.normalized * minSpeed;
        }
        rb.MovePosition(rb.position + (currentVelocity * SpeedToPlayer) * Time.fixedDeltaTime);
    }

    private void EjectIntoBlackHole()
    {
        AdaptParticleVisualDistance();

        Vector2 directionToTarget = (targetTransform.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        vfxContainer.transform.rotation = Quaternion.Euler(0, 0, targetAngle + 90);


        if (!particlesAnimated)
        {
            AnimateGasParticle();
            AnimateMatterParticle1();
            AnimateMatterParticle2();
        }

        if (ParticlesAnimated())
            particlesAnimated = true;

        UpdateMassAndPointsToPlayer();
    }

    private void UpdateMassAndPointsToPlayer()
    {
        if (!fillStarted)
        {
            lastTickTime = Time.time;
            scaleLerpStartTime = Time.time;
            fillStarted = true;
            currentTick = 0;
        }

        // Lerp scale from current scale to minimum scale over decreaseScaleTime
        float scaleLerpProgress = (Time.time - scaleLerpStartTime) / decreaseScaleTime;
        scaleLerpProgress = Mathf.Clamp01(scaleLerpProgress);
        Vector3 targetScale = Vector3.Lerp(initialScale, targetFinalScale, scaleLerpProgress);
        transform.localScale = targetScale;

        Vector3 targetParticleScale = Vector3.Lerp(particleInitialScale, targetFinalScale, scaleLerpProgress);
        gasParticle.transform.localScale = targetParticleScale;
        matterParticle.transform.localScale = targetParticleScale;
        matterParticle2.transform.localScale = targetParticleScale;

        if (Time.time >= lastTickTime + timeBetweenTicks && currentTick < numOfTicks)
        {
            int numOfObjectsToAbsorb = maxNumOfPoints / numOfTicks;

            if (numOfObjectsToAbsorb < 1)
                numOfObjectsToAbsorb = 1;

            playerLogic.AddWhiteHoleMass(massPerTick * MassPerTickMultiplier(), numOfObjectsToAbsorb);

            if (!codexEntryChecked)
            {
                codexEntryChecked = true;
                HUDController.Instance.CheckCodexEntry(codexEntry);
            }

            currentTick++;
            lastTickTime = Time.time;
        }

        if (currentTick >= numOfTicks)
        {
            disappearing = true;
            spriteRenderer.enabled = false;
            gasParticleMain.simulationSpeed = 1;
            matterParticleMain.simulationSpeed = 1;
            matterParticle2Main.simulationSpeed = 1;
            gasParticle.Stop();
            matterParticle.Stop();
            matterParticle2.Stop();
        }
    }

    private int MassPerTickMultiplier()
    {
        if (levelOfInjection == GameManager.Level.Level2)
            return 2;

        else if (levelOfInjection == GameManager.Level.Level3)
            return 4;

        else if (levelOfInjection == GameManager.Level.Level4)
            return 6;

        else if (levelOfInjection == GameManager.Level.Level5)
            return 8;

        else if (levelOfInjection == GameManager.Level.Level6)
            return 10;

        else if (levelOfInjection == GameManager.Level.Level7)
            return 12;

        else if (levelOfInjection == GameManager.Level.Level8)
            return 14;

        else if (levelOfInjection == GameManager.Level.Level9)
            return 16;

        else
            return 16;
    }

    private void AdaptParticleVisualDistance()
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetTransform.position);
        float normalizedDistance = Mathf.Clamp01((distanceToTarget - minDistanceVFX) / (maxDistanceVFX - minDistanceVFX));
        float newLifetimeMatterLine = Mathf.Lerp(minLifetimeMatterLine, maxLifetimeMatterLine, normalizedDistance);
        float newLifetimeGasLine = Mathf.Lerp(minLifetimeGasLine, maxLifetimeGasLine, normalizedDistance);

        matterParticleMain.startLifetime = newLifetimeMatterLine;
        matterParticle2Main.startLifetime = newLifetimeMatterLine;
        gasParticleMain.startLifetime = newLifetimeGasLine;
    }

    private bool ParticlesAnimated()
    {
        return gasParticleShape.angle == finalShapeAngle
            && gasParticleShape.radius == finalShapeRadius
            && matterParticleShape.angle == finalShapeAngle
            && matterParticleShape.radius == finalShapeRadius
            && matterParticleMain.simulationSpeed == finalSimSpeedMatterLine
            && gasParticleMain.simulationSpeed == finalSimSpeedGasLine;
    }

    private void AnimateGasParticle()
    {
        if (gasParticleShape.angle > finalShapeAngle)
        {
            newShapeAngleGas -= animateSpeed * Time.deltaTime;
            gasParticleShape.angle = newShapeAngleGas;

            if (newShapeAngleGas <= finalShapeAngle)
            {
                newShapeAngleGas = finalShapeAngle;
                gasParticleShape.angle = newShapeAngleGas;
            }
        }

        if (gasParticleShape.radius > finalShapeRadius)
        {
            newShapeRadiusGas -= animateSpeed * Time.deltaTime;
            gasParticleShape.radius = newShapeRadiusGas;

            if (newShapeRadiusGas <= finalShapeRadius)
            {
                newShapeRadiusGas = finalShapeRadius;
                gasParticleShape.radius = newShapeRadiusGas;
            }
        }

        if (gasParticleMain.simulationSpeed < finalSimSpeedGasLine)
        {
            newSimulationSpeedGasLine += animateSpeed * Time.deltaTime;
            gasParticleMain.simulationSpeed = newSimulationSpeedGasLine;

            if (newSimulationSpeedGasLine >= finalSimSpeedGasLine)
            {
                newSimulationSpeedGasLine = finalSimSpeedGasLine;
                gasParticleMain.simulationSpeed = newSimulationSpeedGasLine;
            }
        }
    }

    private void AnimateMatterParticle1()
    {
        if (matterParticleShape.angle > finalShapeAngle)
        {
            newShapeAngleMatter -= animateSpeed * Time.deltaTime;
            matterParticleShape.angle = newShapeAngleMatter;

            if (newShapeAngleMatter <= finalShapeAngle)
            {
                newShapeAngleMatter = finalShapeAngle;
                matterParticleShape.angle = newShapeAngleMatter;
            }
        }

        if (matterParticleShape.radius > finalShapeRadius)
        {
            newShapeRadiusMatter -= animateSpeed * Time.deltaTime;
            matterParticleShape.radius = newShapeRadiusMatter;

            if (newShapeRadiusMatter <= finalShapeRadius)
            {
                newShapeRadiusMatter = finalShapeRadius;
                matterParticleShape.radius = newShapeRadiusMatter;
            }
        }

        if (matterParticleMain.simulationSpeed < finalSimSpeedMatterLine)
        {
            newSimulationSpeedMatterLine1 += animateSpeed * Time.deltaTime;
            matterParticleMain.simulationSpeed = newSimulationSpeedMatterLine1;

            if (newSimulationSpeedMatterLine1 >= finalSimSpeedMatterLine)
            {
                newSimulationSpeedMatterLine1 = finalSimSpeedMatterLine;
                matterParticleMain.simulationSpeed = newSimulationSpeedMatterLine1;
            }
        }
    }

    private void AnimateMatterParticle2()
    {
        if (matterParticle2Shape.angle > finalShapeAngle)
        {
            newShapeAngleMatter2 -= animateSpeed * Time.deltaTime;
            matterParticle2Shape.angle = newShapeAngleMatter2;

            if (newShapeAngleMatter2 <= finalShapeAngle)
            {
                newShapeAngleMatter2 = finalShapeAngle;
                matterParticle2Shape.angle = newShapeAngleMatter2;
            }
        }

        if (matterParticle2Shape.radius > finalShapeRadius)
        {
            newShapeRadiusMatter2 -= animateSpeed * Time.deltaTime;
            matterParticle2Shape.radius = newShapeRadiusMatter2;

            if (newShapeRadiusMatter2 <= finalShapeRadius)
            {
                newShapeRadiusMatter2 = finalShapeRadius;
                matterParticle2Shape.radius = newShapeRadiusMatter2;
            }
        }

        if (matterParticle2Main.simulationSpeed < finalSimSpeedMatterLine)
        {
            newSimulationSpeedMatterLine2 += animateSpeed * Time.deltaTime;
            matterParticle2Main.simulationSpeed = newSimulationSpeedMatterLine2;

            if (newSimulationSpeedMatterLine2 >= finalSimSpeedMatterLine)
            {
                newSimulationSpeedMatterLine2 = finalSimSpeedMatterLine;
                matterParticle2Main.simulationSpeed = newSimulationSpeedMatterLine2;
            }
        }
    }

    private void ChangeSize()
    {
        if (inTargetOrbit)
            return;

        OrbitRadius += 5;

        if (GM.CurrentLevel == GameManager.Level.Level2)
            ChangeScale(level2ScaleVectors);

        else if (GM.CurrentLevel == GameManager.Level.Level3)
            ChangeScale(level3ScaleVectors);

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            ChangeScale(level4ScaleVectors);

        else if (GM.CurrentLevel == GameManager.Level.Level5)
            ChangeScale(level5ScaleVectors);

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            ChangeScale(level6ScaleVectors);

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            ChangeScale(level7ScaleVectors);

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            ChangeScale(level8ScaleVectors);

        else if (GM.CurrentLevel == GameManager.Level.Level9)
            ChangeScale(level9ScaleVectors);

        else
            ChangeScale(level9ScaleVectors);
    }

    private void ChangeScale(Vector2 scaleVectors)
    {
        StopCurrentCoroutine();

        currentCoroutine = LerpToNewScale(scaleVectors);
        StartCoroutine(currentCoroutine);
    }

    private IEnumerator LerpToNewScale(Vector2 scaleVectors)
    {
        float elapsedTime = 0;

        Vector3 currentScale = transform.localScale;
        Vector3 currentGasParticleScale = gasParticle.transform.localScale;
        Vector3 currentMatterParticleScale = matterParticle.transform.localScale;

        Vector3 targetScale = new Vector3(scaleVectors.x, scaleVectors.x, 1f);
        Vector3 targetGasParticleScale = new Vector3(scaleVectors.y, scaleVectors.y, 1f);
        Vector3 targetMatterParticleScale = new Vector3(scaleVectors.y, scaleVectors.y, 1f);

        while (elapsedTime < sameScaleAsPlayerTime)
        {
            elapsedTime += Time.deltaTime;

            transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / sameScaleAsPlayerTime);
            gasParticle.transform.localScale = Vector3.Lerp(currentGasParticleScale, targetGasParticleScale, elapsedTime / sameScaleAsPlayerTime);
            matterParticle.transform.localScale = Vector3.Lerp(currentMatterParticleScale, targetMatterParticleScale, elapsedTime / sameScaleAsPlayerTime);
            matterParticle2.transform.localScale = Vector3.Lerp(currentMatterParticleScale, targetMatterParticleScale, elapsedTime / sameScaleAsPlayerTime);

            yield return null;
        }

        transform.localScale = targetScale;
        gasParticle.transform.localScale = targetGasParticleScale;
        matterParticle.transform.localScale = targetMatterParticleScale;
        matterParticle2.transform.localScale = targetMatterParticleScale;
    }

    private int CalculatePoints()
    {
        if (levelOfInjection == GameManager.Level.Level1)
            return GM.Level2ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level2)
            return GM.Level3ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level3)
            return GM.Level4ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level4)
            return GM.Level5ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level5)
            return GM.Level6ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level6)
            return GM.Level7ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level7)
            return GM.Level8ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level8)
            return GM.Level9ObjectCount;

        else if (levelOfInjection == GameManager.Level.Level9)
            return GM.Level10ObjectCount;

        else
            return GM.Level10ObjectCount;
    }

    public void EnterOrbitOfPlayer(Transform _targetOrbit)
    {
        targetTransform = _targetOrbit;
        levelOfInjection = GM.CurrentLevel;
        maxNumOfPoints = CalculatePoints();
        initialScale = transform.localScale;
        particleInitialScale = gasParticle.transform.localScale;

        coll.enabled = false;
        inTargetOrbit = true;

        Vector2 toTarget = targetTransform.position - transform.position;
        float r = toTarget.magnitude;
        Vector2 radialDir = toTarget / r;
        Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x);

        float orbitalSpeed = Mathf.Sqrt(Gravity / r);
        currentVelocity = tangentDir * orbitalSpeed;

        vfxContainer.enabled = true;

        // Reset fill state
        fillStarted = false;
        currentTick = 0;
        lastTickTime = 0f;
    }
    public void EnterOrbitOfClone(Transform _targetOrbit)
    {
        EnterOrbitOfPlayer(_targetOrbit);
    }
    public void EnterOrbitOfOtherCelestialBody(CelestialBody celestialBody, Collider2D _collider)
    {
        // This method is not used in WhiteHole, but must be implemented due to interface
        // No specific behavior for entering orbit of another celestial body
    }
}
