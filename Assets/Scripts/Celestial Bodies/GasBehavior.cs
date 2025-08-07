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

    public override void EnterOrbit()
    {
        base.EnterOrbit();
        trailParticle.gameObject.SetActive(true);
    }

    protected override void Shrink(CelestialBodySettings _settings)
    {
        if (trailParticle.isPlaying) 
        {
            trailParticle.Stop();
            trailParticle.transform.SetParent(null);
            trailParticleMain.simulationSpeed = 3f;
        }

        alpha -= _settings.FadeOutSpeed * Time.fixedDeltaTime;
        mainBodyTransform.localScale = Vector3.Lerp(mainBodyTransform.localScale, targetScale, _settings.ShrinkSpeed * Time.fixedDeltaTime);

        if (alpha <= 0f)
            base.AbsorbIntoPlayer();
    }
}
