using UnityEngine;

public class LightningLine : MonoBehaviour
{
    [SerializeField] private float timeActive;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask detectLayers;
    private float timer;
    private TrailRenderer trailRenderer;
    private Material material;
    private float alpha;
    private static int ALPHA = Shader.PropertyToID("_Alpha");

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        material = trailRenderer.material;
    }
    void OnEnable()
    {
        alpha = 2f;
        material.SetFloat(ALPHA, alpha);
        timer = timeActive;
    }
    // use active list from celestial body manager. which ever celestial body that is within a certain range of the lightning object (detectionRadius)
    void Update()
    {
        ManageTimerAndAlpha();
    }

    private void ManageTimerAndAlpha()
    {
        if (timer <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 1 && alpha > 0)
        {
            alpha -= Time.deltaTime * 2;
            material.SetFloat(ALPHA, alpha);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }  
}
