using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : Tower
{
    [Header("Projectile Tower Variables/Attributes")]
    public Transform projectile;
    public float projectileSpeed; 


    protected override void FireFunctionality()
    {
        Transform t = Instantiate(projectile);
        t.position = transform.position + shootingSpot;
        t.GetComponent<TowerProjectile>().tower = this;
        t.GetComponent<TowerProjectile>().target = target;//Activate(target, projectileSpeed);
        t.parent = GameMaster.gm.projectileHolder;
        StartCoroutine(Cooldown(1/aps));
    }
}
