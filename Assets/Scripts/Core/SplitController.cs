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
    private float elaspedTime;
    private float rotz;
    private float spiralRotz;
    private Transform cloneTrans;
    public bool Active { get; private set; }
    private CinemachineFramingTransposer transposer;
    private float targetCameraDistance;
    private float previousCameraDistance;

    void Awake()
    {
        cloneTrans = clone.transform;
        transposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    void OnEnable()
    {
        previousCameraDistance = transposer.m_CameraDistance;
        targetCameraDistance = player.GetLevelCloneCameraDistanceValue();
        Active = true;
        currentState = CurrentState.Initializing;
        elaspedTime = 0;
        rotz = 0;
        spiralRotz = 0;
        cloneTrans.position = transform.position;
    }

    void OnDisable()
    {
        Active = false;
        cloneTrans.position = transform.position;
    }

    void Update()
    {
        rotz -= rotationSpeed * Time.deltaTime;
        spiralRotz -= spiralRotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0, 0, rotz);
        cloneSpiral.rotation = Quaternion.Euler(0, 0, spiralRotz);

        if (currentState == CurrentState.Initializing)
            InitializingLerpValues();
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
        }
    }
}
