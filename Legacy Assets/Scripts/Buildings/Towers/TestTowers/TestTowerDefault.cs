using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTowerDefault : Tower
{
    protected override void CustomFire()
    {
        Transform latestBullet = Instantiate(attackPrefab, towerHead.position, towerHead.rotation, attackHolder);

        latestBullet.GetComponent<Rigidbody>().AddForce(latestBullet.forward * 100.0f, ForceMode.VelocityChange);
    }
}
