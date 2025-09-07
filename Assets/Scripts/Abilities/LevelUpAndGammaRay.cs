using Cinemachine;
using UnityEngine;

public class LevelUpAndGammaRay : MonoBehaviour
{
    private enum CurrentState
    {
        Idle,
        LevelUpShockwave,
        SoloGammaRayBurst
    }
    private CurrentState currentState;
    [SerializeField] private GameObject shockwaveObject;
    [SerializeField] private float shockwaveTime;
    [SerializeField] private CinemachineVirtualCamera mainVC;
    private CinemachineBasicMultiChannelPerlin mainVCPerlin;
    private Material material;
    private static int WAVE_DISTANCE = Shader.PropertyToID("_WaveDistance");
    private float elaspedTime;
    private static float MIN_DISTANCE = -0.1f;
    private static float MAX_DISTANCE = 1f;
    private float cameraShakeValue;
    void Awake()
    {
        material = shockwaveObject.GetComponent<SpriteRenderer>().material;
        mainVCPerlin = mainVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (currentState == CurrentState.LevelUpShockwave)
            LevelUpShockwave();
    }

    private void LevelUpShockwave()
    {
         if (elaspedTime > shockwaveTime)
        {
            mainVCPerlin.m_AmplitudeGain = 0f;
            shockwaveObject.SetActive(false);
            currentState = CurrentState.Idle;
            return;
        }

        elaspedTime += Time.deltaTime;

        float lerpedProgress = elaspedTime / shockwaveTime;

        material.SetFloat(WAVE_DISTANCE, Mathf.Lerp(MIN_DISTANCE, MAX_DISTANCE, lerpedProgress));
        mainVCPerlin.m_AmplitudeGain = Mathf.Lerp(cameraShakeValue, 0f, lerpedProgress);
    }

    public void ActivateLevelUpShockwave(float _cameraShakeValues)
    {
        elaspedTime = 0f;
        material.SetFloat(WAVE_DISTANCE, MIN_DISTANCE);
        mainVCPerlin.m_AmplitudeGain = _cameraShakeValues;
        cameraShakeValue = _cameraShakeValues;
        shockwaveObject.SetActive(true);
        currentState = CurrentState.LevelUpShockwave;
    }
}
