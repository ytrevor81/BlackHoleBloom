//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class OrbitingBodiesHUDController : MonoBehaviour
//{
//    private GameManager GM;

//    [SerializeField] private GameObject resetButton;
//    [SerializeField] private Image levelBar;
//    [SerializeField] private Animator levelBarAnimator;
//    [SerializeField] private TMP_Text levelText;
//    [SerializeField] private TMP_Text scoreText;
    
//    private PlayerController player;
//    private float score;

//    void Start()
//    {
//        player = PlayerController.Instance;
//        GM = GameManager.Instance;
//        levelBar.fillAmount = 0;
//    }

    
//    private void MassToLevelUpText(float _mass)
//    {
//        score += _mass;
//        scoreText.text = score.ToString("N0");
//    }

//    private string LevelText()
//    {
//        if (GM.CurrentLevel == GameManager.Level.Level1)
//            return "LVL 1";
        
//        else if (GM.CurrentLevel == GameManager.Level.Level2)
//            return "LVL 2";
        
//        else if (GM.CurrentLevel == GameManager.Level.Level3)
//            return "LVL 3";
        
//        else if (GM.CurrentLevel == GameManager.Level.Level4)
//            return "LVL 4";
        
//        else if (GM.CurrentLevel == GameManager.Level.Level5)
//            return "LVL 5";

//        else if (GM.CurrentLevel == GameManager.Level.Level6)
//            return "LVL 6";
        
//        else if (GM.CurrentLevel == GameManager.Level.Level7)
//            return "LVL 7";

//        else if (GM.CurrentLevel == GameManager.Level.Level8)
//            return "LVL 8";

//        else if (GM.CurrentLevel == GameManager.Level.Level9)
//            return "LVL 9";
        
//        else
//            return "LVL 10";
//    }

    
//    public void UpdateMassNum(float mass) 
//    {
//        MassToLevelUpText(mass);
//        UpdateLevelBar(mass);

//        // if (CanLevelUp(mass))
//        //     LevelUp();        
//    }

    
//    private void UpdateLevelBar(float _mass)
//    {
//        //levelBar.fillAmount = _mass / (float)RequiredMassToLevelUp();
//    }

    
//    // private float RequiredMassToLevelUp()
//    // {
//    //     if (GM.CurrentLevel == GameManager.Level.Level1)
//    //         return GM.Level2MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level2)
//    //         return GM.Level3MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level3)
//    //         return GM.Level4MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level4)
//    //         return GM.Level5MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level5)
//    //         return GM.Level6MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level6)
//    //         return GM.Level7MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level7)
//    //         return GM.Level8MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level8)
//    //         return GM.Level9MassCount;
        
//    //     else if (GM.CurrentLevel == GameManager.Level.Level9)
//    //         return GM.Level10MassCount;

//    //     else 
//    //         return GM.Level10MassCount;
//    // }

    
//    // private bool CanLevelUp(float _mass)
//    // {
//    //     return (GM.CurrentLevel == GameManager.Level.Level1 && _mass >= GM.Level2MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level2 && _mass >= GM.Level3MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level3 && _mass >= GM.Level4MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level4 && _mass >= GM.Level5MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level5 && _mass >= GM.Level6MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level6 && _mass >= GM.Level7MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level7 && _mass >= GM.Level8MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level8 && _mass >= GM.Level9MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level9 && _mass >= GM.Level10MassCount) ||
//    //            (GM.CurrentLevel == GameManager.Level.Level10 && _mass >= GM.Level10MassCount);
//    // }
    
//    private void LevelUp()
//    {
//        // //player.LevelUp();
//        // UpdateMassNum(player.Mass);  
//        // levelBar.fillAmount = 0;      
//        // levelText.text = LevelText();
//        // levelBarAnimator.SetTrigger("LevelUp");

//        // if (GM.CurrentLevel == GameManager.Level.Level10)
//        //     resetButton.SetActive(true);        
//    }
//}
