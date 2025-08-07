//using UnityEngine;

//public class AbsorbButtonAnimations : MonoBehaviour
//{
//    private Animator animator;
//    [SerializeField] private OrbitingBodiesHUDController controller;
//    private bool readyToAbsorb;
//    private const string EXIT_TRIGGER = "Exit";

//    void Awake()
//    {
//        animator = GetComponent<Animator>();
//    }

//    public void AbsorbOrbits_Event()
//    {
//        // if (readyToAbsorb)
//        // {
//        //     controller.AbsorbOrbits();
//        //     readyToAbsorb = false;
//        //     animator.SetTrigger(EXIT_TRIGGER);
//        // }
//    }

//    public void ReadyToAbsorb_AnimationEvent()
//    {
//        readyToAbsorb = true;
//    }

//    public void Exit_AnimationEvent()
//    {
//        gameObject.SetActive(false);
//    }
//}
