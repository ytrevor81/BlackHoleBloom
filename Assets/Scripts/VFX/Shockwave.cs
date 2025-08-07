using Cinemachine;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private float shockwaveTime;
    [SerializeField] private CinemachineVirtualCamera mainVC;
    public float CameraShakeValue { get; set; } = 5f;
    private CinemachineBasicMultiChannelPerlin mainVCPerlin;

    private Material material;
    private static int WAVE_DISTANCE = Shader.PropertyToID("_WaveDistance");
    private float lerpedAmountShockwave;
    private float lerpedAmountCameraShake;
    private float elaspedTime;
    private static float MIN_DISTANCE = -0.1f;
    private static float MAX_DISTANCE = 1f;
    void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        mainVCPerlin = mainVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void OnEnable()
    {
        material.SetFloat(WAVE_DISTANCE, MIN_DISTANCE);
        mainVCPerlin.m_AmplitudeGain = CameraShakeValue;
        elaspedTime = 0f;
        lerpedAmountShockwave = 0f;
        lerpedAmountCameraShake = 0f;
    }

    void Update()
    {
        if (elaspedTime > shockwaveTime)
        {
            mainVCPerlin.m_AmplitudeGain = 0f;
            gameObject.SetActive(false);
            return;
        }

        elaspedTime += Time.deltaTime;
        lerpedAmountShockwave = Mathf.Lerp(MIN_DISTANCE, MAX_DISTANCE, elaspedTime / shockwaveTime);
        lerpedAmountCameraShake = Mathf.Lerp(CameraShakeValue, 0f, elaspedTime / shockwaveTime);
        material.SetFloat(WAVE_DISTANCE, lerpedAmountShockwave);
        mainVCPerlin.m_AmplitudeGain = lerpedAmountCameraShake;
    }
}
