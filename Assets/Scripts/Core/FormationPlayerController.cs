// using System.Collections;
// using System.Collections.Generic;
// using Cinemachine;
// using UnityEngine;

// public class FormationPlayerController : MonoBehaviour
// {
//     private GameManager GM;
//     public static FormationPlayerController Instance { get; private set; }

//     [field: Header("Main Refs")]
//     [field: Space]

//     [field: SerializeField] public float GravitationalForce { get; private set; }
//     [SerializeField] private HUDController HUD;
//     [SerializeField] private float initialSpeed;
//     [SerializeField] private VariableJoystick joystick;
//     [SerializeField] private CinemachineVirtualCamera mainCamera;
//     [SerializeField] private Shockwave shockwave;
//     [SerializeField] private GameObject gravityArea;

//     [Header("Level Increases")]
//     [Space]

//     [SerializeField] private LevelStats level2Stats;
//     [SerializeField] private LevelStats level3Stats;
//     [SerializeField] private LevelStats level4Stats;
//     [SerializeField] private LevelStats level5Stats;
//     [SerializeField] private LevelStats level6Stats;
//     [SerializeField] private LevelStats level7Stats;
//     [SerializeField] private LevelStats level8Stats;
//     [SerializeField] private LevelStats level9Stats;
//     [SerializeField] private LevelStats level10Stats;

//     [Space]
//     [SerializeField] private float scaleLerpSpeed;
//     [SerializeField] private float cameraLerpSpeed;

//     [Header("Extra VFX")]
//     [Space]

//     [SerializeField] private GalaxyVFXController galaxyVFXController;
//     [SerializeField] private Transform accretionDiskContainer;
//     [SerializeField] private Transform spiralArms;
//     [SerializeField] private float accretionDiskRotationSpeed;
//     [SerializeField] private float spiralArmsRotationSpeed;

//     [Header("Boost")]
//     [Space]
//     [SerializeField] private float boostSpeedTime;
//     [SerializeField] private ParticleSystem boostVFX;
//     private float boostMultiplier;
//     private float boostTimer;
//     private bool inBoostMode;
//     private bool boostVFXActive;

//     private Rigidbody2D rb;

//     private Vector2 currentVelocity;
//     private List<CelestialBody> objectsInOrbit = new List<CelestialBody>();
//     private HashSet<GameObject> objectsInOrbitGameObjects = new HashSet<GameObject>();
//     private float speed;
//     private bool updateSizeAndView;
//     private float targetCameraView;
//     private float targetScale;
//     private float accretionDiskRotation;
//     private float spiralArmsRotation;
//     private void Awake()
//     {
//         Instance = this;
//         rb = GetComponent<Rigidbody2D>();

//         targetCameraView = mainCamera.m_Lens.OrthographicSize;
//         targetScale = transform.localScale.x;
//         speed = initialSpeed;
//     }

//     IEnumerator Start()
//     {
//         GM = GameManager.Instance;
//         GM.OnLevelChanged += LevelUp;
//         yield return null;
//         gravityArea.SetActive(true);
//     }

//     void OnDisable()
//     {
//         if (GM != null)
//             GM.OnLevelChanged -= LevelUp;
//     }

//     void Update()
//     {
//         if (updateSizeAndView)
//             UpdateScaleAndCameraSize();

//         UpdateExtraVFX();
//     }

//     private void UpdateExtraVFX()
//     {
//         accretionDiskRotation -= accretionDiskRotationSpeed * Time.deltaTime;
//         accretionDiskContainer.rotation = Quaternion.Euler(0, 0, accretionDiskRotation);

//         if (GM.CurrentLevel == GameManager.Level.Level1)
//             return;

//         spiralArmsRotation -= spiralArmsRotationSpeed * Time.deltaTime;
//         spiralArms.rotation = Quaternion.Euler(0, 0, spiralArmsRotation);
//     }

//     private void FixedUpdate()
//     {
//         rb.velocity = Vector2.zero;
//         Vector2 playerInputForce = CalculatePlayerInputForce();
//         Vector2 predictedPosition = rb.position + playerInputForce * Time.fixedDeltaTime;
//         rb.MovePosition(predictedPosition);
//     }

//     private Vector2 CalculatePlayerInputForce()
//     {
//         Vector2 inputForce;

//         if (inBoostMode)
//         {
//             inputForce = joystick.Direction * (speed * boostMultiplier);
//             boostTimer -= Time.deltaTime;

//             if (boostTimer <= 1)
//             {
//                 boostMultiplier -= Time.fixedDeltaTime;

//                 if (boostVFXActive)
//                 {
//                     boostVFX.Stop();
//                     HUD.EndBoostIconAnimation();
//                     boostVFXActive = false;
//                 }

//                 if (boostMultiplier <= 1)
//                 {
//                     inBoostMode = false;
//                 }
//             }
//         }
//         else
//         {
//             inputForce = joystick.Direction * speed;
//         }
//         return inputForce;
//     }

//     private void UpdateScaleAndCameraSize()
//     {
//         float orthographicSize = mainCamera.m_Lens.OrthographicSize;

//         if (orthographicSize != targetCameraView)
//             mainCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(orthographicSize, targetCameraView, cameraLerpSpeed * Time.deltaTime);

//         if (transform.localScale.x != targetScale)
//             transform.localScale = Vector2.MoveTowards(transform.localScale, new Vector2(targetScale, targetScale), scaleLerpSpeed * Time.deltaTime);

//         if (mainCamera.m_Lens.OrthographicSize == targetCameraView && transform.localScale.x == targetScale)
//             updateSizeAndView = false;
//     }

//     public void AddWhiteHoleMass(float mass, int numOfObjectsAbsorbed)
//     {
//         GM.Score += mass;
//         GM.NumOfObjectsAbsorbed += numOfObjectsAbsorbed;
//         HUD.UpdateHUDNumbers(mass);
//         GM.ChangeLevelIfValid();
//     }

//     public void AddMass(float mass)
//     {
//         GM.Score += mass;
//         GM.NumOfObjectsAbsorbed += 1;

//         HUD.UpdateHUDNumbers(mass);
//         GM.ChangeLevelIfValid();
//     }

//     public void EnterBoostMode()
//     {
//         HUD.ActivateBoostIcon();
//         boostTimer = boostSpeedTime;
//         boostMultiplier = 2;
//         inBoostMode = true;
//         boostVFX.Play();
//         boostVFXActive = true;
//     }

//     private void IncreaseStats()
//     {
//         if (GM.CurrentLevel == GameManager.Level.Level2)
//         {
//             targetScale = level2Stats.Scale;
//             targetCameraView = level2Stats.OrthographicSize;
//             speed = level2Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level3)
//         {
//             targetScale = level3Stats.Scale;
//             targetCameraView = level3Stats.OrthographicSize;
//             speed = level3Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level4)
//         {
//             targetScale = level4Stats.Scale;
//             targetCameraView = level4Stats.OrthographicSize;
//             speed = level4Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level5)
//         {
//             targetScale = level5Stats.Scale;
//             targetCameraView = level5Stats.OrthographicSize;
//             speed = level5Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level6)
//         {
//             targetScale = level6Stats.Scale;
//             targetCameraView = level6Stats.OrthographicSize;
//             speed = level6Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level7)
//         {
//             targetScale = level7Stats.Scale;
//             targetCameraView = level7Stats.OrthographicSize;
//             speed = level7Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level8)
//         {
//             targetScale = level8Stats.Scale;
//             targetCameraView = level8Stats.OrthographicSize;
//             speed = level8Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level9)
//         {
//             targetScale = level9Stats.Scale;
//             targetCameraView = level9Stats.OrthographicSize;
//             speed = level9Stats.Speed;
//         }
//         else if (GM.CurrentLevel == GameManager.Level.Level10)
//         {
//             targetScale = level10Stats.Scale;
//             targetCameraView = level10Stats.OrthographicSize;
//             speed = level10Stats.Speed;
//         }

//         updateSizeAndView = true;
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag(BHBConstants.BARRIER) && currentVelocity != Vector2.zero)
//         {
//             Vector2 collisionNormal = collision.contacts[0].normal;

//             if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
//                 currentVelocity.x = 0;

//             else
//                 currentVelocity.y = 0;
//         }
//     }

//     public void AddObjectToOrbitingList(Collider2D _collider)
//     {
//         if (objectsInOrbitGameObjects.Contains(_collider.gameObject))
//             return;

//         objectsInOrbitGameObjects.Add(_collider.gameObject);

//         CelestialBody celestialBody = _collider.GetComponent<CelestialBody>();
//         celestialBody.EnterOrbit();
//         objectsInOrbit.Add(celestialBody);
//         //orbitsHUD.UpdateOrbitNum(objectsInOrbit.Count);
//     }
//     public void RemoveObjectFromOrbitingList(CelestialBody _celestialBody)
//     {
//         if (!objectsInOrbitGameObjects.Contains(_celestialBody.gameObject))
//             return;

//         objectsInOrbitGameObjects.Remove(_celestialBody.gameObject);
//         objectsInOrbit.Remove(_celestialBody);
//         //orbitsHUD.UpdateOrbitNum(objectsInOrbit.Count);
//     }
//     private void LevelUp()
//     {
//         IncreaseStats();
//         shockwave.CameraShakeValue = GetCameraShakeValue();
//         shockwave.gameObject.SetActive(true);
//         galaxyVFXController.UpgradeVFX();
//     }

//     private float GetCameraShakeValue()
//     {
//         if (GM.CurrentLevel == GameManager.Level.Level2)
//             return level2Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level3)
//             return level3Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level4)
//             return level4Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level5)
//             return level5Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level6)
//             return level6Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level7)
//             return level7Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level8)
//             return level8Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level9)
//             return level9Stats.CameraShakeValue;

//         else if (GM.CurrentLevel == GameManager.Level.Level10)
//             return level10Stats.CameraShakeValue;

//         return 0f;
//     }
// }
// //[System.Serializable]
// //public struct LevelStats
// //{
// //    public float Scale;
// //    public float OrthographicSize;
// //    public float Speed;
// //    public float CameraShakeValue;
// //}
