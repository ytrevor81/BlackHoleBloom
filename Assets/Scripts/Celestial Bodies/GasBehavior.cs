using UnityEngine;

public class GasBehavior : CelestialBody
{
    [Space]

    [SerializeField] private Transform mainBodyTransform;
    [SerializeField] private ParticleSystem trailParticle;

    private ParticleSystem.MainModule trailParticleMain;
    protected override void Awake()
    {
        base.Awake();
        originalScale = mainBodyTransform.localScale;
        trailParticleMain = trailParticle.main;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        mainBodyTransform.localScale = originalScale; 

        if (trailParticle.transform.parent == null)
        {
            trailParticle.transform.position = transform.position;
            trailParticle.transform.SetParent(transform);
            trailParticle.Play();
            trailParticleMain.simulationSpeed = 1f;
        }
    }

    public override void EnterOrbitOfPlayer(Transform _targetOrbit)
    {
        base.EnterOrbitOfPlayer(_targetOrbit);
        trailParticle.gameObject.SetActive(true);
    }

    protected override void Shrink()
    {
        if (trailParticle.isPlaying) 
        {
            trailParticle.Stop();
            trailParticle.transform.SetParent(null);
            trailParticleMain.simulationSpeed = 3f;
        }

        elaspedTime += Time.fixedDeltaTime;
        float lerpedProgress = elaspedTime / BHBConstants.SHRINK_TO_SINGULARITY_TIME;
        
        mainBodyTransform.localScale = Vector3.Lerp(originalScale, targetScale, lerpedProgress);
        rb.position = Vector3.Lerp(startingPos, targetPos, lerpedProgress);


        if (elaspedTime >= BHBConstants.SHRINK_TO_SINGULARITY_TIME)
            base.AbsorbIntoPlayer();
    }
}
