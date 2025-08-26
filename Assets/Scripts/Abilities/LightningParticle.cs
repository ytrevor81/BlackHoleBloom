using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningParticle : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float travelTime;
    private float elaspedTime;

    private ParticleSystem particles;
    private ParticleSystem.Particle[] particleArray;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        if (particles == null || target == null) return;
        
        int particleCount = particles.main.maxParticles;
        
        if (particleArray == null || particleArray.Length < particleCount)
            particleArray = new ParticleSystem.Particle[particleCount];
        
        int aliveParticles = particles.GetParticles(particleArray);
        
        for (int i = 0; i < aliveParticles; i++)
        {
            Vector3 direction = (target.position - particleArray[i].position).normalized;
            particleArray[i].velocity = direction * 5f; // 5f is speed, adjust as needed
        }
        
        particles.SetParticles(particleArray, aliveParticles);
    }
}
