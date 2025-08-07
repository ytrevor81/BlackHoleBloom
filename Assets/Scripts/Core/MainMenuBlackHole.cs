using UnityEngine;

public class MainMenuBlackHole : GalaxyVFXController
{
    [Space]

    [SerializeField] private float stayOnColorTime; // 3
    [SerializeField] private float changeColorTime; //1

    private float mainMenuTimer;
    private bool stayingOnColor;
    private bool canChangeColor;
    protected override void Awake()
    {
        sprialMaterial = sprialSpriteRenderer.material;
        accretionDiskMaterial = accretionDiskSpriteRenderer.material;
        targetColorArm = spiralLightOrangeColor;
        sparklesEmission = sparkles.emission;

        targetColorArm = spiralBlueColor;
        accretionDiskMaterial.SetColor(COLOR_DISK, spiralBlueColor);
    }
    protected override void Start()
    {
        //do nothing. avoiding cached things that will cause a null ref error
    }

    protected override void Update()
    {
        RotateDiskAndSpiral();
        ChangeMainMenuColor();
    }
    protected override void RotateDiskAndSpiral()
    {
        accretionDiskRotation -= accretionDiskRotationSpeed * Time.deltaTime;
        spiralArmsRotation -= spiralArmsRotationSpeed * Time.deltaTime;

        accretionDiskContainer.rotation = Quaternion.Euler(0, 0, accretionDiskRotation);
        spiralArms.rotation = Quaternion.Euler(0, 0, spiralArmsRotation);
    }

    private void ChangeMainMenuColor()
    {
        if (!canChangeColor)
            return;

        if (!stayingOnColor)
        {
            colorElaspedTime += Time.deltaTime;

            currentColorArm = Color.Lerp(previousArmColor, targetColorArm, colorElaspedTime / changeColorTime);
            sprialMaterial.SetColor(COLOR_ARM, currentColorArm);
            accretionDiskMaterial.SetColor(COLOR_DISK, currentColorArm);

            if (colorElaspedTime > changeColorTime)
            {
                stayingOnColor = true;
                colorElaspedTime = 0;
                sprialMaterial.SetColor(COLOR_ARM, targetColorArm);
                accretionDiskMaterial.SetColor(COLOR_DISK, targetColorArm);
                mainMenuTimer = stayOnColorTime;
            }
        }
        else
        {
            mainMenuTimer -= Time.deltaTime;

            if (mainMenuTimer <= 0)
            {
                stayingOnColor = false;
                previousArmColor = sprialMaterial.GetColor(COLOR_ARM);
                targetColorArm = ChooseNextMainMenuColor(targetColorArm);
            }
        }
    }

    private Color ChooseNextMainMenuColor(Color _color)
    {
        if (_color == spiralBlueColor)
            return spiralBoostColor;

        else if (_color == spiralBoostColor)
            return spiralLightOrangeColor;

        else if (_color == spiralLightOrangeColor)
            return Color.white;

        else if (_color == Color.white)
            return spiralVioletColor;

        return spiralBlueColor;
    }
    
    public void MainMenuChangeColor()
    {
        mainMenuTimer = stayOnColorTime;
        stayingOnColor = true;
        canChangeColor = true;
    }
}
