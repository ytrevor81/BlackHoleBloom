//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    private InputManager IM;
//    public static PlayerController Instance { get; private set; }

//    [SerializeField] private bool turnOffTrajectory;
//    [SerializeField] private float thrustForce;
//    [SerializeField] private float maxSpeed;
//    [SerializeField] private Camera mainCamera;

//    [Space]

//    [SerializeField] private FuelMeter fuelMeter;
//    [SerializeField] private float usingFuelSpeed;
//    [SerializeField] private float refillFuelSpeed;

//    [Space]
//    [SerializeField] private GameObject scannerContainer;
//    [SerializeField] private LineRenderer scanner;

//    private List<Planet> planets = new List<Planet>();
//    private List<Planet> simulatedPlanets = new List<Planet>();
//    private Vector3[] simulatedPoints = new Vector3[0];

//    private Planet currentPlanet;

//    [Space]

//    [SerializeField] private ParticleSystem leftThrust;
//    [SerializeField] private ParticleSystem rightThrust;

//    private ParticleSystem.EmissionModule leftThrustEmission;
//    private ParticleSystem.EmissionModule rightThrustEmission;
//    private Transform leftThrustTransform;
//    private Transform rightThrustTransform;

//    private Rigidbody2D rb;
//    private Vector3 startingPos;

//    public float Fuel { get; private set; } = 100f;
//    private Vector2 leftThrustDirection;
//    private Vector2 rightThrustDirection;
//    private Vector2 currentVelocity;
//    RaycastHit2D[] raycastResults = new RaycastHit2D[5];

//    [SerializeField] private float gravitationalForceMultiplier = 1.0f; // Multiplier for gravitational force
//    [SerializeField] private float radialGain = 1.0f;
//    [SerializeField] private float tangentGain = 1.0f;
//    [SerializeField] private float slingshotStrength = 5.0f;

//    [Header("Trajectory Preview")]
//    [Space]
//    [SerializeField] private LineRenderer lineRenderer;
//    [Tooltip("How many points to simulate")]
//    [SerializeField] private int trajectorySteps = 50;
//    [Tooltip("Simulation timestep (s) between each sample)")]
//    [SerializeField] private float trajectoryDeltaTime = 0.02f;
//    private bool simulationHitPlanet;
//    public bool InGravityAssistArea { get; set; } = false;

//    private void Awake()
//    {
//        Instance = this;

//        rb = GetComponent<Rigidbody2D>();
//        startingPos = transform.position;

//        leftThrustEmission = leftThrust.emission;
//        leftThrustTransform = leftThrust.transform;
//        leftThrustEmission.enabled = false;

//        rightThrustEmission = rightThrust.emission;
//        rightThrustTransform = rightThrust.transform;
//        rightThrustEmission.enabled = false;
//    }

//    void Start()
//    {
//        IM = InputManager.Instance;
//    }

//    private Vector2 ClampedCurrentVelocity()
//    {
//        return new Vector2(Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(currentVelocity.y, -maxSpeed, maxSpeed));
//    }

//    public void ToggleTrajectoryPath(bool active)
//    {
//        turnOffTrajectory = !active;
//    }

//    // private void HandleRBAngluarVelocity()
//    // {
//    //     if (Mathf.Abs(rb.angularVelocity) > 0)
//    //     {
//    //         float angularVelocity = rb.angularVelocity;

//    //         if (rb.angularVelocity > 0)
//    //         {
//    //             angularVelocity -= 1000f * Time.fixedDeltaTime;
//    //             if (angularVelocity <= 0)
//    //                 angularVelocity = 0;
//    //         }
//    //         else
//    //         {
//    //             angularVelocity += 1000f * Time.fixedDeltaTime;
//    //             if (angularVelocity >= 0)
//    //                 angularVelocity = 0;
//    //         }

//    //         rb.angularVelocity = angularVelocity;
//    //     }
//    //     else
//    //     {
//    //         // Get current rotation and normalize it to 0-360 range
//    //         float currentRotation = transform.rotation.eulerAngles.z;
//    //         float targetRotation = Mathf.Round(currentRotation / 180f) * 180f;

//    //         if (currentRotation != targetRotation)
//    //         {
//    //             transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(currentRotation, targetRotation, Time.fixedDeltaTime * 10));
//    //             float angleDifference = Mathf.DeltaAngle(currentRotation, targetRotation);

//    //             if (Mathf.Abs(angleDifference) < 1f)
//    //             {
//    //                 transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
//    //             }
//    //         }
//    //     }
//    // }

//    // void Update()
//    // {
//    //     HandleScanner();
//    // }

//    private Vector2 NudgeTowardsOrbit(Vector2 _velocity, Vector2 _position)
//    {
//        if (planets.Count == 0) 
//            return _velocity;

//        // 1) Vector from ship → planet
//        Vector2 toPlanet = (Vector2)planets[0].transform.position - _position;
//        float r = toPlanet.magnitude;
//        Vector2 radialDir = toPlanet / r;

//        Vector2 tCCW       = new Vector2(-radialDir.y, radialDir.x);
//        Vector2 tCW        = -tCCW;

//        // find how much currentVelocity projects onto each
//        float vCCW = Vector2.Dot(_velocity, tCCW);
//        float vCW = Vector2.Dot(_velocity, tCW);

//        //Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x);

//        // choose the one that matches the existing direction
//        Vector2 tangentDir = (Mathf.Abs(vCCW) >= Mathf.Abs(vCW)) ? tCCW : tCW;
//        float vTangent = Vector2.Dot(_velocity, tangentDir);

//        // 2) Compute the “standard” orbital speed: v = sqrt(mu / r)
//        float desiredSpeed = Mathf.Sqrt(planets[0].GravitationalForce / r);

//        // 3) Split _velocity
//        float vRadial = Vector2.Dot(_velocity, radialDir);
//        // 4) Errors
//        // float errorRadial = -vRadial;                // we want zero radial drift
//        // float errorTangent = desiredSpeed - vTangent; // how far off from perfect orbit
//        float errorRadial   = -Vector2.Dot(_velocity, radialDir);
//        float errorTangent  = Mathf.Sign(vTangent) * desiredSpeed - vTangent;

//        // 5) Apply a gentle PD-style correction

//        Vector2 correction = radialDir * (errorRadial * radialGain) + tangentDir * (errorTangent * tangentGain);

//        // 6) Integrate into your velocity
//        _velocity += correction * Time.fixedDeltaTime;

//        return _velocity;
//    }

//    private Vector2 ApplyContinuousAssist(Vector2 _velocity, Vector2 _position)
//    {
//        if (!InGravityAssistArea || planets.Count == 0) 
//            return _velocity;

//        // Optional: skip boost if you're pointing too directly at/away from the planet
//        Vector2 toPlanet = (Vector2)planets[0].transform.position - _position;
//        Vector2 radial  = toPlanet.normalized;
//        float  radialDot = Mathf.Abs(Vector2.Dot(_velocity.normalized, radial));

//        if (radialDot > 0.3f)  // e.g. > 0.3 means > ~17° off tangent
//            return _velocity;

//        // Simple speed‐scale: multiply your vector by (1 + kΔt)
//        float boostFactor = 1f + slingshotStrength * Time.fixedDeltaTime;
//        return _velocity * boostFactor;
//    }   

//    private bool EscapingWithGravityAssist(Vector2 _velocity, Vector2 _position) //expand on with timer?
//    {
//        if (!InGravityAssistArea || planets.Count == 0)
//            return false;

//        Vector2 toPlanet= (Vector2)planets[0].transform.position - _position;
//        Vector2 radial  = toPlanet.normalized;
//        // dot < 0 means velocity is pointing away from the planet
//        float dot       = Vector2.Dot(_velocity.normalized, radial);
//        bool escaping   = InGravityAssistArea && dot < -Mathf.Cos(20f * Mathf.Deg2Rad);

//        return escaping;
//    }
//    private bool OmitOrbitAssistance(Vector2 _inputDir, Vector2 _velocity)
//    {
//        if (_inputDir == Vector2.zero)
//            return false;

//        else if (_inputDir.x < 0 && _velocity.x > 0)
//            return true;

//        else if (_inputDir.x > 0 && _velocity.x < 0)
//            return true;

//        else if (_inputDir.y < 0 && _velocity.y > 0)
//            return true;

//        else if (_inputDir.y > 0 && _velocity.y < 0)
//            return true;

//        return false;
//    }

//    private void FixedUpdate()
//    {
//        rb.velocity = Vector2.zero;
//        Vector2 gravitationalForce = CalculateGravitationalForce(rb.position) * gravitationalForceMultiplier;
//        Vector2 playerInputForce = CalculatePlayerInputForce(true);

//        bool isEscaping = EscapingWithGravityAssist(currentVelocity, rb.position);
//        bool omitOrbitAssistance = OmitOrbitAssistance(playerInputForce, currentVelocity);

//        if (isEscaping)
//            gravitationalForce = Vector2.zero;

//        currentVelocity += (gravitationalForce + playerInputForce) * Time.fixedDeltaTime;

//        if (!omitOrbitAssistance)
//            currentVelocity = NudgeTowardsOrbit(currentVelocity, rb.position);

//        currentVelocity = ApplyContinuousAssist(currentVelocity, rb.position);

//        if (!isEscaping)
//            currentVelocity = ClampedCurrentVelocity();
        
//        Vector2 predictedPosition = rb.position + currentVelocity * Time.fixedDeltaTime;
        
//        // Move the player to the predicted position
//        rb.MovePosition(predictedPosition);

//        PlotTrajectory();
//        // if (IM.LeftThrustInput == Vector2.zero && IM.RightThrustInput == Vector2.zero)
//        //     RefillFuel();
//    }

//    private Vector2 CalculateGravitationalForce(Vector2 position)
//    {
//        Vector2 totalGravitationalForce = Vector2.zero;

//        for (int i = 0; i < planets.Count; i++)
//        {
//            Vector2 directionToPlanet = (Vector2)planets[i].transform.position - position;
//            float distance = directionToPlanet.magnitude;
//            directionToPlanet.Normalize();
//            // Calculate gravitational force
//            float forceMagnitude = planets[i].GravitationalForce / (distance * distance);
//            totalGravitationalForce += directionToPlanet * forceMagnitude;
//        }

//        return totalGravitationalForce;
//    }
//    private Vector2 CalculatePlayerInputForce(bool engageWithThurstParticles)
//    {
//        Vector2 inputForce = Vector2.zero;

//        if (Fuel <= 0)
//        {
//            if (engageWithThurstParticles)  
//            {
//                leftThrustEmission.enabled = false;
//                rightThrustEmission.enabled = false;
//            }

//            return inputForce;
//        }

//        if (IM.LeftThrustInput != Vector2.zero)
//        {
//            leftThrustDirection = IM.LeftThrustInput.normalized;

//            if (engageWithThurstParticles)  
//                HandleThrustParticle(leftThrustEmission, leftThrustTransform, leftThrustDirection);

//            inputForce += leftThrustDirection * thrustForce;

//            //UseFuel();
//            //HandleRBAngluarVelocity();
//        }
//        else
//        {
//            if (engageWithThurstParticles)  
//                leftThrustEmission.enabled = false;
//        }

//        if (IM.RightThrustInput != Vector2.zero)
//        {
//            rightThrustDirection = IM.RightThrustInput.normalized;

//            if (engageWithThurstParticles)  
//                HandleThrustParticle(rightThrustEmission, rightThrustTransform, rightThrustDirection);

//            inputForce += rightThrustDirection * thrustForce;

//            //UseFuel();
//            //HandleRBAngluarVelocity();
//        }
//        else
//        {
//            rightThrustEmission.enabled = false;
//        }

//        return inputForce;
//    }

//    private void PlotTrajectory()
//    {
//        if (planets.Count == 0 || turnOffTrajectory)
//        {
//            lineRenderer.gameObject.SetActive(false);
//            return;
//        }

//        lineRenderer.gameObject.SetActive(true);

//        Vector2 simPos = rb.position;
//        Vector2 simVel = currentVelocity;
//        Vector2 previousSimPos;

//        simulationHitPlanet = false;

//        if (simulatedPoints.Length != trajectorySteps)
//            simulatedPoints = new Vector3[trajectorySteps];

//        // 3) Step through a mini‐simulation:
//        for (int i = 0; i < trajectorySteps; i++)
//        {
//            if (simulationHitPlanet)
//            {
//                simulatedPoints[i] = new Vector3(simPos.x, simPos.y, 0f);            
//                continue;
//            }

//            previousSimPos = simPos;
//            Vector2 grav = CalculateGravitationalForce(simPos) * gravitationalForceMultiplier;
//            Vector2 input = CalculatePlayerInputForce(false); 

//            simVel += (grav + input) * trajectoryDeltaTime;
//            simVel = NudgeTowardsOrbit(simVel, simPos);
//            simVel = ApplyContinuousAssist(simVel, simPos);

//            simVel = new Vector2(Mathf.Clamp(simVel.x, -maxSpeed, maxSpeed), Mathf.Clamp(simVel.y, -maxSpeed, maxSpeed));
//            simPos += simVel * trajectoryDeltaTime;
//            simPos = DetermineFinalSimulationPoint(previousSimPos, simPos);

//            simulatedPoints[i] = new Vector3(simPos.x, simPos.y, 0f);            
//        }

//        simulationHitPlanet = false;
//        lineRenderer.positionCount = trajectorySteps;
//        lineRenderer.SetPositions(simulatedPoints);
//    }

//    private Vector2 DetermineFinalSimulationPoint(Vector2 previousSimPos, Vector2 currentSimPos)
//    {
//        Vector2 finalSimPos = currentSimPos;
//        //Vector2 totalGravitationalForce = Vector2.zero;

//        Vector2 direction = (currentSimPos - previousSimPos).normalized;
//        float distance = Vector2.Distance(previousSimPos, currentSimPos);

//        int hitCount = Physics2D.RaycastNonAlloc(previousSimPos, direction, raycastResults, distance);

//        for (int i = 0; i < hitCount; i++)
//        {
//            if (raycastResults[i].collider.gameObject.layer == LayerMask.NameToLayer(StellarDriftConstants.PLANET))
//            {
//                simulationHitPlanet = true;
//                break;
//            }
//        }
//        return finalSimPos;
//    }

//    private void RefillFuel()
//    {
//        if (Fuel == 100f)
//            return;

//        Fuel += Time.deltaTime * refillFuelSpeed;
//        Fuel = Mathf.Clamp(Fuel, 0, 100f);
//        fuelMeter.UpdateMeter(Fuel);
//    }
//    private void UseFuel()
//    {
//        Fuel -= Time.deltaTime * usingFuelSpeed;
//        Fuel = Mathf.Clamp(Fuel, 0, 100f);
//        fuelMeter.UpdateMeter(Fuel);
//    }

//    private void HandleThrustParticle(ParticleSystem.EmissionModule _emission, Transform particleTransform, Vector2 thrustDirection)
//    {
//        _emission.enabled = true;

//        float angle = Mathf.Atan2(thrustDirection.y, thrustDirection.x) * Mathf.Rad2Deg;
//        particleTransform.rotation = Quaternion.Euler(0f, 0f, angle - 180f);
//    }

//    public void ResetPlayer()
//    {
//        currentVelocity = Vector2.zero;
//        transform.position = startingPos;
//        transform.rotation = Quaternion.identity;
//        rb.velocity = Vector2.zero;
//        rb.angularVelocity = 0;
//        rb.rotation = 0;

//        Fuel = 100f;
//        fuelMeter.UpdateMeter(Fuel);

//        planets.Clear();
//        currentPlanet = null;
//        scannerContainer.SetActive(false);
//        InGravityAssistArea = false;
//    }

//    // private void HandleScanner()
//    // {
//    //     if (planets.Count == 0)
//    //     {
//    //         scannerContainer.SetActive(false);
//    //         return;
//    //     }

//    //     if (currentPlanet == null || currentPlanet != planets[0])
//    //         currentPlanet = planets[0];

//    //     scanner.positionCount = 2;
//    //     scanner.SetPosition(0, transform.position);
//    //     scanner.SetPosition(1, currentPlanet.GetCicumferencePosition());

//    //     if (!scannerContainer.activeInHierarchy)
//    //         scannerContainer.SetActive(true);
//    // }

//    public void AddPlanet(Planet planet)
//    {
//        if (planets.Contains(planet))
//            return;

//        planets.Add(planet);
//    }

//    public void RemovePlanet(Planet planet)
//    {
//        if (!planets.Contains(planet))
//            return;

//        planets.Remove(planet);
//    } 

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag(StellarDriftConstants.BARRIER) && currentVelocity != Vector2.zero)
//        {
//            Vector2 collisionNormal = collision.contacts[0].normal;
            
//            if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
//                currentVelocity.x = 0;
//            else
//                currentVelocity.y = 0;
//        }
//    }
//}
