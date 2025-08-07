using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;

    //[SerializeField] private float timeActive;
    //private float timer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //private void OnEnable()
    //{
    //    timer = timeActive;
    //}

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
    }

    //private void Update()
    //{
    //    timer -= Time.deltaTime;

    //    if (timer < 0)
    //        gameObject.SetActive(false);
    //}

    public void ImpactedPlanet()
    {
        gameObject.SetActive(false);
    }
}
