using System.Collections;
using UnityEngine;

public class GalaxyVFXController : MonoBehaviour
{
    protected GameManager GM; 
    [SerializeField] protected ParticleSystem sparkles;
    protected ParticleSystem.EmissionModule sparklesEmission;
    protected PlayerController player;

    [Header("Accretion Disk")]
    [Space]
    [SerializeField] protected SpriteRenderer accretionDiskSpriteRenderer;
    [SerializeField] protected Transform accretionDiskContainer;
    [SerializeField] protected float accretionDiskRotationSpeed;
    protected Material accretionDiskMaterial;
    protected float accretionDiskRotation;

    [Header("Sprial")]
    [Space]
    [SerializeField] protected Transform spiralArms;
    [SerializeField] protected float spiralArmsRotationSpeed;
    [SerializeField] protected float lerpTimeToShaderValues;
    [SerializeField] protected Color spiralLightOrangeColor;
    [SerializeField] protected Color spiralBlueColor;
    [SerializeField] protected Color spiralVioletColor;
    [SerializeField] protected Color spiralBoostColor;

    [SerializeField] protected SpriteRenderer sprialSpriteRenderer;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level2;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level3;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level4;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level5;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level6;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level7;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level8;
    [SerializeField] private SprialShaderValues sprialShaderValues_Level9;
    protected float spiralArmsRotation;
    private SprialShaderValues previousShaderValues;
    private SprialShaderValues currentShaderValues;

    protected static int TWIST_STRENGTH = Shader.PropertyToID("_TwistStrength");
    protected static int ARM_THICKNESS = Shader.PropertyToID("_ArmThickness");
    protected static int NOISE_SCALE = Shader.PropertyToID("_NoiseScale");
    protected static int NOISE_STRENGTH = Shader.PropertyToID("_NoiseStrength");
    protected static int RADIUS_FALLOFF = Shader.PropertyToID("_RadiusFalloff");
    protected static int NOISE_SPEED = Shader.PropertyToID("_NoiseSpeed");
    protected static int COLOR_INTENSITY = Shader.PropertyToID("_ColorIntensity");
    protected static int BRIGHTNESS_EXPONENT = Shader.PropertyToID("_BrightnessExponent");
    protected static int EDGE_BRIGHTNESS = Shader.PropertyToID("_EdgeBrightness");
    protected static int ARMS_ALPHA = Shader.PropertyToID("_ArmsAlpha");
    protected static int COLOR_ARM = Shader.PropertyToID("_ColorArm");
    protected static int COLOR_DISK = Shader.PropertyToID("_GlowColor");

    protected float currentTwistStrength;
    protected float currentArmThickness;
    protected float currentNoiseScale;
    protected float currentNoiseStrength;
    protected float currentRadiusFalloff;
    protected float currentNoiseSpeed;
    protected float currentColorIntensity;
    protected float currentBrightnessExponent;
    protected float currentEdgeBrightness;
    protected float currentArmsAlpha;
    protected Color currentColorArm;
    protected Color targetColorArm;
    protected Color previousArmColor;
    protected bool lerpColor;
    protected Material sprialMaterial;
    protected bool shaderChangeActive;
    protected bool colorChangeActive;
    protected float shaderElaspedTime;
    protected float colorElaspedTime;
    protected IEnumerator currentCoroutine;

    [Header("Split Clone")]
    [Space]
    [SerializeField] private SplitController splitCloneContainer;
    [SerializeField] private Transform cloneSpiralArms;
    [SerializeField] private Transform cloneAccretionDiskContainer;

    private Material cloneDiskMaterial;
    private Material cloneSpiralMaterial;

    protected virtual void Awake()
    {
        sprialMaterial = sprialSpriteRenderer.material;
        accretionDiskMaterial = accretionDiskSpriteRenderer.material;
        targetColorArm = spiralLightOrangeColor;
        sparklesEmission = sparkles.emission;
        accretionDiskMaterial.SetColor(COLOR_DISK, spiralLightOrangeColor);

        cloneDiskMaterial = cloneAccretionDiskContainer.GetComponent<SpriteRenderer>().material;
        cloneSpiralMaterial = cloneSpiralArms.GetComponent<SpriteRenderer>().material;
        cloneDiskMaterial.SetColor(COLOR_DISK, spiralLightOrangeColor);
        cloneSpiralMaterial.SetColor(COLOR_ARM, spiralLightOrangeColor);
    }

    protected virtual void Start()
    {
        GM = GameManager.Instance;
        player = PlayerController.Instance;
        InitializeShaderValues();
    }

    void OnDisable()
    {
        StopCurrentCoroutine();
    }

    protected virtual void Update()
    {
        RotateDiskAndSpiral();
        ChangeShaderValues();
        
        if (!player.inBoostMode)
            ChangeAccretionDiskAndSpiralColor();
    }

    public void SetCloneVFXForSplit()
    {
        cloneDiskMaterial.SetColor(COLOR_DISK, targetColorArm);
        cloneSpiralMaterial.SetColor(COLOR_ARM, targetColorArm);

        cloneSpiralMaterial.SetFloat(TWIST_STRENGTH, currentTwistStrength);
        cloneSpiralMaterial.SetFloat(ARM_THICKNESS, currentArmThickness);
        cloneSpiralMaterial.SetFloat(NOISE_SCALE, currentNoiseScale);
        cloneSpiralMaterial.SetFloat(NOISE_STRENGTH, currentNoiseStrength);
        cloneSpiralMaterial.SetFloat(RADIUS_FALLOFF, currentRadiusFalloff);
        cloneSpiralMaterial.SetFloat(NOISE_SPEED, currentNoiseSpeed);
        cloneSpiralMaterial.SetFloat(COLOR_INTENSITY, currentColorIntensity);
        cloneSpiralMaterial.SetFloat(BRIGHTNESS_EXPONENT, currentBrightnessExponent);
        cloneSpiralMaterial.SetFloat(EDGE_BRIGHTNESS, currentEdgeBrightness);
        cloneSpiralMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);
    }

    public Material GetSpiralMaterial()
    {
        return cloneSpiralMaterial;
    }

    protected virtual void RotateDiskAndSpiral()
    {
        accretionDiskRotation -= accretionDiskRotationSpeed * player.BoostMultiplier * Time.deltaTime;
        spiralArmsRotation -= spiralArmsRotationSpeed * player.BoostMultiplier * Time.deltaTime;

        accretionDiskContainer.rotation = Quaternion.Euler(0, 0, accretionDiskRotation);
        spiralArms.rotation = Quaternion.Euler(0, 0, spiralArmsRotation);

        if (splitCloneContainer.Active)
        {
            cloneAccretionDiskContainer.rotation = Quaternion.Euler(0, 0, accretionDiskRotation);
            cloneSpiralArms.rotation = Quaternion.Euler(0, 0, spiralArmsRotation);
        }
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }
    private void ChangeShaderValues()
    {
        if (!shaderChangeActive) return;

        if (shaderElaspedTime > lerpTimeToShaderValues)
        {
            shaderChangeActive = false;
            SetShaderValues();
            return;
        }

        shaderElaspedTime += Time.deltaTime;
        LerpShaderValues();
    }

    private void ChangeAccretionDiskAndSpiralColor()
    {
        if (colorChangeActive)
        {
            colorElaspedTime += Time.deltaTime;
            currentColorArm = Color.Lerp(previousArmColor, targetColorArm, colorElaspedTime / lerpTimeToShaderValues);

            sprialMaterial.SetColor(COLOR_ARM, currentColorArm);
            accretionDiskMaterial.SetColor(COLOR_DISK, currentColorArm);

            if (colorElaspedTime > lerpTimeToShaderValues)
            {
                colorChangeActive = false;
                sprialMaterial.SetColor(COLOR_ARM, targetColorArm);
                accretionDiskMaterial.SetColor(COLOR_DISK, targetColorArm);
            }
        }
    }

    public void ChangeToBoostColor()
    {
        lerpColor = false;
        StopCurrentCoroutine();

        currentCoroutine = ChangeToColorOverrideCoroutine(spiralBoostColor);
        StartCoroutine(currentCoroutine);
    }
    public void ChangeToPreviousColor()
    {
        lerpColor = false;
        StopCurrentCoroutine();
        currentCoroutine = ChangeToColorOverrideCoroutine(targetColorArm);
        StartCoroutine(currentCoroutine);
    }

    private IEnumerator ChangeToColorOverrideCoroutine(Color _targetColor)
    {
        Color currentColor;
        Color cachedColor = sprialMaterial.GetColor(COLOR_ARM);
        float _elaspedTime = 0;

        while (_elaspedTime < lerpTimeToShaderValues)
        {
            _elaspedTime += Time.deltaTime;
            currentColor = Color.Lerp(cachedColor, _targetColor, _elaspedTime / lerpTimeToShaderValues);
            sprialMaterial.SetColor(COLOR_ARM, currentColor);
            accretionDiskMaterial.SetColor(COLOR_DISK, currentColor);
            yield return null;
        }

        sprialMaterial.SetColor(COLOR_ARM, _targetColor);
        accretionDiskMaterial.SetColor(COLOR_DISK, _targetColor);
    }

    public void UpgradeVFX()
    {
        shaderElaspedTime = 0;

        if (GM.CurrentLevel == GameManager.Level.Level2)
        {
            previousShaderValues = sprialShaderValues_Level2;
            currentShaderValues = sprialShaderValues_Level2;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level3)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level3;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level4)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level4;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level5)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level5;

            sparklesEmission.rateOverTime = sprialShaderValues_Level5.SparklesEmissionIntensity;
            
            previousArmColor = sprialMaterial.GetColor(COLOR_ARM);
            targetColorArm = spiralVioletColor;

            if (!player.inBoostMode)
                lerpColor = true;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level6)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level6;
            sparklesEmission.rateOverTime = sprialShaderValues_Level6.SparklesEmissionIntensity;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level7)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level7;

            previousArmColor = sprialMaterial.GetColor(COLOR_ARM);
            targetColorArm = spiralBlueColor;

            if (!player.inBoostMode)
                lerpColor = true;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level8)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level8;
            sparklesEmission.rateOverTime = sprialShaderValues_Level8.SparklesEmissionIntensity;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level9)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level9;
            sparklesEmission.rateOverTime = sprialShaderValues_Level9.SparklesEmissionIntensity;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level10)
        {
            previousShaderValues = currentShaderValues;
            currentShaderValues = sprialShaderValues_Level9;
            sparklesEmission.rateOverTime = sprialShaderValues_Level9.SparklesEmissionIntensity;
        }

        shaderChangeActive = true;
    }

    private void LerpShaderValues()
    {
        if (GM.CurrentLevel == GameManager.Level.Level2)
        {
            currentArmsAlpha = Mathf.Lerp(0, currentShaderValues.ArmsAlpha, shaderElaspedTime / lerpTimeToShaderValues);
            sprialMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);

            if (splitCloneContainer.Active)
                cloneSpiralMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);
        }
        else
        {
            currentTwistStrength = Mathf.Lerp(previousShaderValues.TwistStrength, currentShaderValues.TwistStrength, shaderElaspedTime / lerpTimeToShaderValues);
            currentArmThickness = Mathf.Lerp(previousShaderValues.ArmThickness, currentShaderValues.ArmThickness, shaderElaspedTime / lerpTimeToShaderValues);
            currentNoiseScale = Mathf.Lerp(previousShaderValues.NoiseScale, currentShaderValues.NoiseScale, shaderElaspedTime / lerpTimeToShaderValues);
            currentNoiseStrength = Mathf.Lerp(previousShaderValues.NoiseStrength, currentShaderValues.NoiseStrength, shaderElaspedTime / lerpTimeToShaderValues);
            currentRadiusFalloff = Mathf.Lerp(previousShaderValues.RadiusFalloff, currentShaderValues.RadiusFalloff, shaderElaspedTime / lerpTimeToShaderValues);
            currentNoiseSpeed = Mathf.Lerp(previousShaderValues.NoiseSpeed, currentShaderValues.NoiseSpeed, shaderElaspedTime / lerpTimeToShaderValues);
            currentColorIntensity = Mathf.Lerp(previousShaderValues.ColorIntensity, currentShaderValues.ColorIntensity, shaderElaspedTime / lerpTimeToShaderValues);
            currentBrightnessExponent = Mathf.Lerp(previousShaderValues.BrightnessExponent, currentShaderValues.BrightnessExponent, shaderElaspedTime / lerpTimeToShaderValues);
            currentEdgeBrightness = Mathf.Lerp(previousShaderValues.EdgeBrightness, currentShaderValues.EdgeBrightness, shaderElaspedTime / lerpTimeToShaderValues);
            currentArmsAlpha = Mathf.Lerp(previousShaderValues.ArmsAlpha, currentShaderValues.ArmsAlpha, shaderElaspedTime / lerpTimeToShaderValues);

            sprialMaterial.SetFloat(TWIST_STRENGTH, currentTwistStrength);
            sprialMaterial.SetFloat(ARM_THICKNESS, currentArmThickness);
            sprialMaterial.SetFloat(NOISE_SCALE, currentNoiseScale);
            sprialMaterial.SetFloat(NOISE_STRENGTH, currentNoiseStrength);
            sprialMaterial.SetFloat(RADIUS_FALLOFF, currentRadiusFalloff);
            sprialMaterial.SetFloat(NOISE_SPEED, currentNoiseSpeed);
            sprialMaterial.SetFloat(COLOR_INTENSITY, currentColorIntensity);
            sprialMaterial.SetFloat(BRIGHTNESS_EXPONENT, currentBrightnessExponent);
            sprialMaterial.SetFloat(EDGE_BRIGHTNESS, currentEdgeBrightness);
            sprialMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);

            if (splitCloneContainer.Active)
            {
                cloneSpiralMaterial.SetFloat(TWIST_STRENGTH, currentTwistStrength);
                cloneSpiralMaterial.SetFloat(ARM_THICKNESS, currentArmThickness);
                cloneSpiralMaterial.SetFloat(NOISE_SCALE, currentNoiseScale);
                cloneSpiralMaterial.SetFloat(NOISE_STRENGTH, currentNoiseStrength);
                cloneSpiralMaterial.SetFloat(RADIUS_FALLOFF, currentRadiusFalloff);
                cloneSpiralMaterial.SetFloat(NOISE_SPEED, currentNoiseSpeed);
                cloneSpiralMaterial.SetFloat(COLOR_INTENSITY, currentColorIntensity);
                cloneSpiralMaterial.SetFloat(BRIGHTNESS_EXPONENT, currentBrightnessExponent);
                cloneSpiralMaterial.SetFloat(EDGE_BRIGHTNESS, currentEdgeBrightness);
                cloneSpiralMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);
            }

            if (lerpColor)
            {
                currentColorArm = Color.Lerp(previousArmColor, targetColorArm, shaderElaspedTime / lerpTimeToShaderValues);
                sprialMaterial.SetColor(COLOR_ARM, currentColorArm);
                accretionDiskMaterial.SetColor(COLOR_DISK, currentColorArm);

                if (splitCloneContainer.Active)
                {
                    cloneSpiralMaterial.SetColor(COLOR_ARM, currentColorArm);
                    cloneDiskMaterial.SetColor(COLOR_DISK, currentColorArm);
                }
            }
        }
    }

    private void SetShaderValues()
    {
        currentTwistStrength = currentShaderValues.TwistStrength;
        currentArmThickness = currentShaderValues.ArmThickness;
        currentNoiseScale = currentShaderValues.NoiseScale;
        currentNoiseStrength = currentShaderValues.NoiseStrength;
        currentRadiusFalloff = currentShaderValues.RadiusFalloff;
        currentNoiseSpeed = currentShaderValues.NoiseSpeed;
        currentColorIntensity = currentShaderValues.ColorIntensity;
        currentBrightnessExponent = currentShaderValues.BrightnessExponent;
        currentEdgeBrightness = currentShaderValues.EdgeBrightness;
        currentArmsAlpha = currentShaderValues.ArmsAlpha;

        sprialMaterial.SetFloat(TWIST_STRENGTH, currentTwistStrength);
        sprialMaterial.SetFloat(ARM_THICKNESS, currentArmThickness);
        sprialMaterial.SetFloat(NOISE_SCALE, currentNoiseScale);
        sprialMaterial.SetFloat(NOISE_STRENGTH, currentNoiseStrength);
        sprialMaterial.SetFloat(RADIUS_FALLOFF, currentRadiusFalloff);
        sprialMaterial.SetFloat(NOISE_SPEED, currentNoiseSpeed);
        sprialMaterial.SetFloat(COLOR_INTENSITY, currentColorIntensity);
        sprialMaterial.SetFloat(BRIGHTNESS_EXPONENT, currentBrightnessExponent);
        sprialMaterial.SetFloat(EDGE_BRIGHTNESS, currentEdgeBrightness);
        sprialMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);

        if (splitCloneContainer.Active)
        {
            cloneSpiralMaterial.SetFloat(TWIST_STRENGTH, currentTwistStrength);
            cloneSpiralMaterial.SetFloat(ARM_THICKNESS, currentArmThickness);
            cloneSpiralMaterial.SetFloat(NOISE_SCALE, currentNoiseScale);
            cloneSpiralMaterial.SetFloat(NOISE_STRENGTH, currentNoiseStrength);
            cloneSpiralMaterial.SetFloat(RADIUS_FALLOFF, currentRadiusFalloff);
            cloneSpiralMaterial.SetFloat(NOISE_SPEED, currentNoiseSpeed);
            cloneSpiralMaterial.SetFloat(COLOR_INTENSITY, currentColorIntensity);
            cloneSpiralMaterial.SetFloat(BRIGHTNESS_EXPONENT, currentBrightnessExponent);
            cloneSpiralMaterial.SetFloat(EDGE_BRIGHTNESS, currentEdgeBrightness);
            cloneSpiralMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);
        }

        if (lerpColor)
        {
            sprialMaterial.SetColor(COLOR_ARM, targetColorArm);
            accretionDiskMaterial.SetColor(COLOR_DISK, targetColorArm);
            lerpColor = false;

            if (splitCloneContainer.Active)
            {
                cloneSpiralMaterial.SetColor(COLOR_ARM, targetColorArm);
                cloneDiskMaterial.SetColor(COLOR_DISK, targetColorArm);
            }
        }
    }
    private void SetShaderValuesInEditor(SprialShaderValues shaderValues, Color _spiralColor, bool isLevel1)
    {
        Material _spriralMaterial = sprialSpriteRenderer.sharedMaterial;
        Material _accretionDiskMaterial = accretionDiskSpriteRenderer.sharedMaterial;

        _spriralMaterial.SetFloat(TWIST_STRENGTH, shaderValues.TwistStrength);
        _spriralMaterial.SetFloat(ARM_THICKNESS, shaderValues.ArmThickness);
        _spriralMaterial.SetFloat(NOISE_SCALE, shaderValues.NoiseScale);
        _spriralMaterial.SetFloat(NOISE_STRENGTH, shaderValues.NoiseStrength);
        _spriralMaterial.SetFloat(RADIUS_FALLOFF, shaderValues.RadiusFalloff);
        _spriralMaterial.SetFloat(NOISE_SPEED, shaderValues.NoiseSpeed);
        _spriralMaterial.SetFloat(COLOR_INTENSITY, shaderValues.ColorIntensity);
        _spriralMaterial.SetFloat(BRIGHTNESS_EXPONENT, shaderValues.BrightnessExponent);
        _spriralMaterial.SetFloat(EDGE_BRIGHTNESS, shaderValues.EdgeBrightness);
        _spriralMaterial.SetColor(COLOR_ARM, _spiralColor);
        _accretionDiskMaterial.SetColor(COLOR_DISK, _spiralColor);

        if (isLevel1)
            _spriralMaterial.SetFloat(ARMS_ALPHA, 0);

        else
            _spriralMaterial.SetFloat(ARMS_ALPHA, shaderValues.ArmsAlpha);
    }

    private void InitializeShaderValues()
    {
        currentColorArm = spiralLightOrangeColor;

        currentTwistStrength = sprialShaderValues_Level2.TwistStrength;
        currentArmThickness = sprialShaderValues_Level2.ArmThickness;
        currentNoiseScale = sprialShaderValues_Level2.NoiseScale;
        currentNoiseStrength = sprialShaderValues_Level2.NoiseStrength;
        currentRadiusFalloff = sprialShaderValues_Level2.RadiusFalloff;
        currentNoiseSpeed = sprialShaderValues_Level2.NoiseSpeed;
        currentColorIntensity = sprialShaderValues_Level2.ColorIntensity;
        currentBrightnessExponent = sprialShaderValues_Level2.BrightnessExponent;
        currentEdgeBrightness = sprialShaderValues_Level2.EdgeBrightness;
        currentArmsAlpha = 0;

        sprialMaterial.SetFloat(TWIST_STRENGTH, currentTwistStrength);
        sprialMaterial.SetFloat(ARM_THICKNESS, currentArmThickness);
        sprialMaterial.SetFloat(NOISE_SCALE, currentNoiseScale);
        sprialMaterial.SetFloat(NOISE_STRENGTH, currentNoiseStrength);
        sprialMaterial.SetFloat(RADIUS_FALLOFF, currentRadiusFalloff);
        sprialMaterial.SetFloat(NOISE_SPEED, currentNoiseSpeed);
        sprialMaterial.SetFloat(COLOR_INTENSITY, currentColorIntensity);
        sprialMaterial.SetFloat(BRIGHTNESS_EXPONENT, currentBrightnessExponent);
        sprialMaterial.SetFloat(EDGE_BRIGHTNESS, currentEdgeBrightness);
        sprialMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);
        sprialMaterial.SetColor(COLOR_ARM, currentColorArm);

        if (splitCloneContainer == null)
            return;
        
        cloneSpiralMaterial.SetFloat(TWIST_STRENGTH, currentTwistStrength);
        cloneSpiralMaterial.SetFloat(ARM_THICKNESS, currentArmThickness);
        cloneSpiralMaterial.SetFloat(NOISE_SCALE, currentNoiseScale);
        cloneSpiralMaterial.SetFloat(NOISE_STRENGTH, currentNoiseStrength);
        cloneSpiralMaterial.SetFloat(RADIUS_FALLOFF, currentRadiusFalloff);
        cloneSpiralMaterial.SetFloat(NOISE_SPEED, currentNoiseSpeed);
        cloneSpiralMaterial.SetFloat(COLOR_INTENSITY, currentColorIntensity);
        cloneSpiralMaterial.SetFloat(BRIGHTNESS_EXPONENT, currentBrightnessExponent);
        cloneSpiralMaterial.SetFloat(EDGE_BRIGHTNESS, currentEdgeBrightness);
        cloneSpiralMaterial.SetFloat(ARMS_ALPHA, currentArmsAlpha);
        cloneSpiralMaterial.SetColor(COLOR_ARM, currentColorArm);
    }

    public void Level1SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level2, spiralLightOrangeColor, true);
    }

    public void Level2SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level2, spiralLightOrangeColor, false);
    }

    public void Level3SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level3, spiralLightOrangeColor, false);
    }

    public void Level4SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level4, spiralLightOrangeColor, false);
    }

    public void Level5SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level5, spiralVioletColor, false);
    }

    public void Level6SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level6, spiralVioletColor, false);
    }

    public void Level7SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level7, spiralBlueColor, false);
    }

    public void Level8SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level8, spiralBlueColor, false);
    }

    public void Level9SprialShader_Editor()
    {
        SetShaderValuesInEditor(sprialShaderValues_Level9, spiralBlueColor, false);
    }
}

[System.Serializable]
public struct SprialShaderValues
{
    public float TwistStrength;
    public float ArmThickness;
    public float NoiseScale;
    public float NoiseStrength;
    public float RadiusFalloff;
    public float NoiseSpeed;
    public float ColorIntensity;
    public float BrightnessExponent;
    public float EdgeBrightness;
    public float ArmsAlpha;
    public int SparklesEmissionIntensity;
}
