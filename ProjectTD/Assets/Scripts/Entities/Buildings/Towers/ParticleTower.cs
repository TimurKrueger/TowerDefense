using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTower : Tower {

    [Header("Particle Tower Attributes")]
    public ParticleSystem mainPS;

    protected override void FireFunctionality()
    {
        mainPS.Play();
    }

    private void OnParticleTrigger()
    {
        
    }
}
