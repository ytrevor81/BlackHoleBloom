using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]

public class VCHelper : MonoBehaviour
{
    private CinemachineBrain brain;
    private Camera cam;
    private CinemachineVirtualCamera vc;
    private CinemachineFramingTransposer trans;
    private CinemachineConfiner2D confiner2D;

    public VCHelperVectors attachedVCVectors;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        brain = cam.GetComponent<CinemachineBrain>();
        vc = GetComponent<CinemachineVirtualCamera>();
        trans = vc.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void RecaheValues()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        brain = cam.GetComponent<CinemachineBrain>();
        vc = GetComponent<CinemachineVirtualCamera>();
        trans = vc.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    
    public CinemachineFramingTransposer GetTransposer() => trans;

    private bool IsVirtualCameraActiveInBrain()
    {
        if (!brain.isActiveAndEnabled)
            return false;

        else if (brain.ActiveVirtualCamera == null)
            return false;

        else if (brain.ActiveVirtualCamera.VirtualCameraGameObject != gameObject)
            return false;

        return true;
    }

    private void CacheVCVectors()
    {
        float frustumHeight = 2.0f * trans.m_CameraDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float cameraViewWidth = frustumHeight * cam.aspect;
        float cameraViewHeight = cameraViewWidth / cam.aspect;

        Vector3 center = new Vector3(cam.transform.position.x, cam.transform.position.y, 0);
        Vector3 bounds = new Vector3(cameraViewWidth, cameraViewHeight, 1);

        attachedVCVectors = new VCHelperVectors(center, bounds);
    }

    public VCHelperVectors GetVCVectorsAtRuntime()
    {
        if (trans == null)
            RecaheValues();

        float frustumHeight = 2.0f * trans.m_CameraDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float cameraViewWidth = frustumHeight * cam.aspect;
        float cameraViewHeight = cameraViewWidth / cam.aspect;

        Vector3 center = new Vector3(cam.transform.position.x, cam.transform.position.y, 0);
        Vector3 bounds = new Vector3(cameraViewWidth, cameraViewHeight, 1);

        VCHelperVectors vcVectors = new VCHelperVectors(center, bounds);
        return vcVectors;
    }

    private void OnDrawGizmos()
    {
        if (cam == null || vc == null || trans == null || brain == null)
        {
            RecaheValues();
            return;
        }

        if (!IsVirtualCameraActiveInBrain()) return; //do not draw gizmo if current VC is not live

        CacheVCVectors();

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(attachedVCVectors.center, attachedVCVectors.bounds);
    }
}

public struct VCHelperVectors
{
    public Vector3 center;
    public Vector3 bounds;
    public VCHelperVectors(Vector3 centerOfVC, Vector3 boundsOfVC)
    {
        center = centerOfVC;
        bounds = boundsOfVC;
    }
}

