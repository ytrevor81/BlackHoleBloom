using Cinemachine;
using UnityEngine;

public class SplitController : MonoBehaviour
{
    private enum CurrentState
    {
        Initializing,
        Orbiting,
        Retracting
    }

    private CurrentState currentState;

    [Header("MAIN REFS")]
    [Space]

    [SerializeField] private PlayerController player;
    [SerializeField] private GalaxyVFXController vfxController;
    [SerializeField] private SplitClone clone;
    [SerializeField] private Transform cloneSpiral;
    [SerializeField] private Transform targetClonePos;
    [SerializeField] private CinemachineVirtualCamera mainCamera;

    [Header("SETTINGS")]
    [Space]
    [SerializeField] private AnimationCurve goToOrbitPosCurve;
    [SerializeField] private float spiralRotationSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float cloneLerpToTargetPosTime;

    [Space]

    [SerializeField] private float timeInOrbitState;
    private float timer;
    private float elaspedTime;
    private float rotz;
    private float spiralRotz;
    private Transform cloneTrans;
    public bool Active { get; private set; }
    private CinemachineFramingTransposer transposer;
    private float targetCameraDistance;
    private float previousCameraDistance;
    private static int ARMS_ALPHA = Shader.PropertyToID("_ArmsAlpha");
    private Material cloneSpiralMaterial;
    private float currentArmsAlpha;

    void Awake()
    {
        cloneTrans = clone.transform;
        transposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    void OnEnable()
    {
        timer = timeInOrbitState;
        previousCameraDistance = transposer.m_CameraDistance;
        targetCameraDistance = player.GetLevelCloneCameraDistanceValue();
        Active = true;
        currentState = CurrentState.Initializing;
        elaspedTime = 0;
        rotz = 0;
        spiralRotz = 0;
        cloneTrans.position = transform.position;

        vfxController.SetCloneVFXForSplit();
        cloneSpiralMaterial = vfxController.GetSpiralMaterial();
    }

    public Transform GetCloneTransform()
    {
        return cloneTrans;
    }

    void OnDisable()
    {
        Active = false;
        cloneTrans.position = transform.position;
    }

    private void SetGalaxyVFX()
    {
        
    }

    void Update()
    {
        rotz -= rotationSpeed * Time.deltaTime;
        spiralRotz -= spiralRotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0, 0, rotz);
        cloneSpiral.rotation = Quaternion.Euler(0, 0, spiralRotz);

        if (currentState == CurrentState.Initializing)
            InitializingLerpValues();

        else if (currentState == CurrentState.Orbiting)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                currentState = CurrentState.Retracting;
                targetCameraDistance = player.GetTargetCameraDistance();
                previousCameraDistance = transposer.m_CameraDistance;
                elaspedTime = 0;
                Active = false;
            }
        }
        else if (currentState == CurrentState.Retracting)
            RetractingLerpValues();
    }

    private void InitializingLerpValues()
    {
        elaspedTime += Time.deltaTime;
        float curveValue = goToOrbitPosCurve.Evaluate(elaspedTime / cloneLerpToTargetPosTime);
        cloneTrans.position = Vector3.Lerp(transform.position, targetClonePos.position, curveValue);
        transposer.m_CameraDistance = Mathf.Lerp(previousCameraDistance, targetCameraDistance, curveValue);

        if (elaspedTime >= cloneLerpToTargetPosTime)
        {
            cloneTrans.position = targetClonePos.position;
            transposer.m_CameraDistance = targetCameraDistance;
            currentState = CurrentState.Orbiting;
            elaspedTime = 0;
            currentArmsAlpha = cloneSpiralMaterial.GetFloat(ARMS_ALPHA);
        }
    }
    private void RetractingLerpValues()
    {
        elaspedTime += Time.deltaTime;
        float curveValue = goToOrbitPosCurve.Evaluate(elaspedTime / cloneLerpToTargetPosTime);

        cloneTrans.position = Vector3.Lerp(targetClonePos.position, transform.position, curveValue);
        transposer.m_CameraDistance = Mathf.Lerp(previousCameraDistance, targetCameraDistance, curveValue);
        cloneSpiralMaterial.SetFloat(ARMS_ALPHA, Mathf.Lerp(currentArmsAlpha, 0, curveValue));

        if (elaspedTime >= cloneLerpToTargetPosTime)
        {
            cloneTrans.position = transform.position;
            transposer.m_CameraDistance = targetCameraDistance;
            cloneSpiralMaterial.SetFloat(ARMS_ALPHA, 0);
            gameObject.SetActive(false);
        }
    }
}
