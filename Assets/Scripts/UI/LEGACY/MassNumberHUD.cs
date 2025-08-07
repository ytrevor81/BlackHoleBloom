//using System.Text;
//using TMPro;
//using UnityEngine;

//public class MassNumberHUD : MonoBehaviour
//{
//    [SerializeField] private TMP_Text massNumberText;
//    private FormationPlayerController player;
//    private bool lerpingToTargetNum;
//    [SerializeField] private float lerpSpeed = 10;

//    private float currentMassNum;
//    private const string MASS_WORD = "Mass: ";
//    private StringBuilder stringBuilder = new StringBuilder();

//    void Start()
//    {
//        player = FormationPlayerController.Instance;
//        //currentMassNum = player.Mass;
//        massNumberText.text = MASS_WORD + currentMassNum.ToString("N0");
//    }

//    // void Update()
//    // {
//    //     if (player.Mass != currentMassNum && !lerpingToTargetNum)
//    //         lerpingToTargetNum = true;

//    //     if (lerpingToTargetNum)
//    //         LerpToTargetNum();
//    // }

//    // private void LerpToTargetNum()
//    // {
//    //     currentMassNum = Mathf.MoveTowards(currentMassNum, player.Mass, lerpSpeed * Time.deltaTime);

//    //     stringBuilder.Clear();
//    //     stringBuilder.Append(MASS_WORD);
//    //     stringBuilder.Append(currentMassNum.ToString("N0"));
//    //     massNumberText.text = stringBuilder.ToString();

//    //     if (currentMassNum == player.Mass)
//    //         lerpingToTargetNum = false;
//    // }
//}
