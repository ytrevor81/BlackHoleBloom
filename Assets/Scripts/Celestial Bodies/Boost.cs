// using UnityEngine;

// public class Boost : MonoBehaviour
// {
//     private Animator animator;
//     private Rigidbody2D rb;
    
//     [SerializeField] private float speedToPlayer;
//     [SerializeField] private float shrinkSpeed;
//     private PlayerController player;
//     private Vector2 playerPos;
//     private bool inPlayerZone;
//     private bool shrinking;
//     void Awake()
//     {
//         animator = GetComponent<Animator>();
//         rb = GetComponent<Rigidbody2D>();
//     }

//     void OnEnable()
//     {
//         inPlayerZone = false;
//         rb.velocity = Vector2.zero;
//         gameObject.tag = BHBConstants.BOOST;
//     }

//     void Start()
//     {
//         player = PlayerController.Instance;
//     }
    
//     void FixedUpdate()
//     {
//         MoveToPlayer();
//     }

//     private void MoveToPlayer()
//     {
//         if (inPlayerZone)
//         {
//             playerPos = player.transform.position;

//             if (shrinking)
//             {
//                 rb.position = Vector2.MoveTowards(rb.position, playerPos, shrinkSpeed * Time.fixedDeltaTime);
//             }
//             else
//             {
//                 rb.position = Vector2.MoveTowards(rb.position, playerPos, speedToPlayer * Time.fixedDeltaTime);
//             }
//         }
//     }

//     public void EnterOrbit()
//     {
//         inPlayerZone = true;
//         gameObject.tag = BHBConstants.NULL;  
//     }

//     public void TriggerBoost_AnimationEvent()
//     {
//         player.EnterBoostMode();
//     }     
//     public void DeactivateObject_AnimationEvent()
//     {
//         gameObject.SetActive(false);
//     }     

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag(BHBConstants.PLAYER) && !shrinking) 
//         {
//             shrinking = true;
//             animator.SetTrigger("Boost");
//         }
//     }
// }
