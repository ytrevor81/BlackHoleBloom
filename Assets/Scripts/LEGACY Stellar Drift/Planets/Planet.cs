//using UnityEngine;

//public class Planet : MonoBehaviour
//{
//    private enum PlanetType 
//    {
//        Small,
//        Large
//    }

//    [SerializeField] private PlanetType planetType;
//    //[field: SerializeField] public float PercentageScanned { get; private set; }
//    [SerializeField] private Transform circumferencePoint;
//    [SerializeField] private float radiusOfPlanet;
//    [SerializeField] private float gravityRadius;
//    [field: SerializeField] public float GravitationalForce { get; private set; }
//    [SerializeField] private LayerMask validLayers;

//    private PlayerController player;
//    private Collider2D[] collidersDetected = new Collider2D[5];

//    private Vector3 startingPoint;
//    private bool hasStartedOrbit;
//    private float totalCircumference;
//    private float previousAngle;
//    private Vector2 centerPos;

//    private void Awake()    
//    {
//        centerPos = circumferencePoint.position;
//    }

//    private void Start()
//    {
//        player = PlayerController.Instance;
//        //totalCircumference = 2 * Mathf.PI * (circleCollider.radius * transform.localScale.x);
//    }

//    private void Update()
//    {
//        if (hasStartedOrbit)
//        {
//            UpdateCircumferencePoint();
//        }
//    }

    

//    private bool PlayerIsInOrbit(Collider2D _collider)
//    {
//        return _collider.CompareTag(StellarDriftConstants.PLAYER);
//    }

//    private bool PlayerIsOutOfOrbit(bool _playerInOrbit)
//    {
//         return !_playerInOrbit && hasStartedOrbit;
//    }

//    private void FixedUpdate()
//    {
//        bool playerInOrbit = false;
//        int numOfCollidersDetected = Physics2D.OverlapCircleNonAlloc(transform.position, gravityRadius, collidersDetected, validLayers);

//        for (int i = 0; i < numOfCollidersDetected; i++)
//        {
//            if (collidersDetected[i] == null || collidersDetected[i].attachedRigidbody == null)
//                continue;

//            if (PlayerIsInOrbit(collidersDetected[i]))
//            {
//                if (!hasStartedOrbit)
//                {
//                    startingPoint = player.transform.position - transform.position;
//                    player.AddPlanet(this);
//                }

//                playerInOrbit = true;
//            }

//            if (!collidersDetected[i].attachedRigidbody.CompareTag(StellarDriftConstants.PLAYER))
//            {

//                Vector2 directionToCenter = (Vector2)transform.position - collidersDetected[i].attachedRigidbody.position;
//                float distance = directionToCenter.magnitude;   

//                // Safeguard against zero or extremely small distance to avoid infinite or huge forces  
//                if (distance < 0.001f)  
//                    distance = 0.001f;  

//                directionToCenter.Normalize();  

//                // Option 1: Strict inverse-square  
//                // F = G / r^2  
//                float forceMagnitude = GravitationalForce / (distance * distance);  

//                collidersDetected[i].attachedRigidbody.AddForce(directionToCenter * forceMagnitude, ForceMode2D.Force); 
//            }
//        }

//        if (PlayerIsOutOfOrbit(playerInOrbit))
//        {
//            player.RemovePlanet(this);
//            circumferencePoint.gameObject.SetActive(false);
//        }

//        hasStartedOrbit = playerInOrbit;        
//    }

//    public Vector2 GetCicumferencePosition()
//    {
//        circumferencePoint.gameObject.SetActive(true);
//        return circumferencePoint.GetChild(0).position;
//    }
    
//    /*
//     * To find a point on the circumference of a circle:
//     * (x, y) = (h + r * cos(θ), k + r * sin(θ))
//     * where:
//     * (h,k) is the center of the circle
//     * r is the radius
//     * θ is the angle in radians
//     */
    

//    private void UpdateCircumferencePoint()
//    {
//        Vector2 playerPosition = player.transform.position;
//        Vector2 directionToPlayer = (playerPosition - centerPos).normalized;

//        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x);

//        float x = centerPos.x + radiusOfPlanet * Mathf.Cos(angle);
//        float y = centerPos.y + radiusOfPlanet * Mathf.Sin(angle);

//        circumferencePoint.position = new Vector2(x, y);


//        Vector2 direction = (playerPosition - centerPos);
//        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

//        circumferencePoint.rotation = Quaternion.Euler(0f, 0f, rot - 90);
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag(StellarDriftConstants.PLAYER))
//            player.ResetPlayer();
        
//        else if (collision.gameObject.CompareTag(StellarDriftConstants.ASTEROID))
//            collision.gameObject.GetComponent<Asteroid>().ImpactedPlanet();
//    }


//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag(StellarDriftConstants.PLAYER))
//            player.InGravityAssistArea = true;
//    }

//    void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.CompareTag(StellarDriftConstants.PLAYER))
//            player.InGravityAssistArea = false;
//    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.DrawWireSphere(transform.position, gravityRadius);
//        //Gizmos.DrawWireSphere(circumferencePoint.position, radiusOfPlanet);
//    }
//}
