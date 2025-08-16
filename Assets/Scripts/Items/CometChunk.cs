// using System.Collections;
// using UnityEngine;

// public class CometChunk : MonoBehaviour
// {
//     private Transform playerPos;
//     [SerializeField] private Comet comet;
//     [SerializeField] private ParticleSystem trail;
//     [SerializeField] private SpriteRenderer spriteRenderer;
//     [SerializeField] private Rigidbody2D rb;
//     [SerializeField] private float travelSpeed;
//     [SerializeField] private float shrinkSpeedToPlayer;
//     [SerializeField] private float shrinkSpeed;
//     [SerializeField] private float burstSpeed;
//     [SerializeField] private float fadeOutSpeed;
//     [SerializeField] private float timeUntilMoveToPlayer;

//     private bool canMoveToPlayer;

//     private Vector2 travelDirection;
//     private bool shrinking;
//     private float alpha = 1f;
//     private Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);

//     void OnEnable()
//     {
//         playerPos = PlayerController.Instance.transform;
//         Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
//         rb.AddForce(randomDir * burstSpeed, ForceMode2D.Impulse);

//         StartCoroutine(CanMoveToPlayer());
//     }

//     private IEnumerator CanMoveToPlayer()
//     {
//         float timer = timeUntilMoveToPlayer;

//         while (timer > 0)
//         {
//             timer -= Time.deltaTime;
//             yield return null;
//         }

//         canMoveToPlayer = true;
//     }

//     private void GiveBoost()
//     {
//         if (comet.gameObject.activeInHierarchy)
//             comet.GiveBoost();
        
//         gameObject.SetActive(false);
//     }

//     void FixedUpdate()
//     {
//         if (canMoveToPlayer)
//         {
//             if (shrinking)
//             {
//                 rb.isKinematic = true;
//                 rb.velocity = Vector2.zero;
//                 rb.position = Vector2.MoveTowards(rb.position, playerPos.position, shrinkSpeedToPlayer * Time.fixedDeltaTime);

//                 alpha -= fadeOutSpeed * Time.fixedDeltaTime;
//                 spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

//                 transform.localScale = Vector3.Lerp(transform.localScale, targetScale, shrinkSpeed * Time.fixedDeltaTime);

//                 if (alpha <= 0f)
//                     GiveBoost();
//             }
//             else
//             {
//                 travelDirection = (playerPos.position - transform.position).normalized;
//                 rb.AddForce(travelDirection * travelSpeed, ForceMode2D.Force);
//             }

//         }
//     }

//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag(BHBConstants.PLAYER) && !shrinking)
//         {
//             trail.Stop();
//             trail.transform.SetParent(null);
//             shrinking = true;
//         }
//     }
// }
