using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardManager : MonoBehaviour
{
	[SerializeField] int ParticleEmissionRate = 1000;
	[SerializeField] int MaxParticleCount = 5000;
	private ParticleSystem localRef;

	private void Start()
	{
		localRef = GetComponent<ParticleSystem>();
	}

	void Update()
    {
		ParticleSystem.MainModule mainModule = localRef.main;
		mainModule.maxParticles = MaxParticleCount;
		ParticleSystem.EmissionModule emissionModule;
		emissionModule = localRef.emission;
		emissionModule.rateOverTime = ParticleEmissionRate;
    }
}
