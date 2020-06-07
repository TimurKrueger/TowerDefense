using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TowerProjectile : MonoBehaviour {

    public Transform target;
    public ProjectileTower tower;
    public float speed = 1.0f; //-1 for instant teleportation of the projectile
    public float dmg = 1.0f;
    public bool activated;
    public Vector3 velocity = Vector3.zero;
    //Rigidbody rb;
    public long projectileID = -1;
    public Transform childEmitterHolder;
    public TimedDestruction childDestroyer;
}

class TowerProjectileSystem : ComponentSystem
{
    struct Components
    {
        public TowerProjectile proj;
        public Transform transform;
    }

    /// <summary>
    /// Main handle function for any projectile
    /// </summary>
    protected override void OnUpdate()
    {
        //If the game exists continue (we need it for the following operations)
        if (GameMaster.gm != null) {

            //Retrieve a list of all valid projectile entities and iterate over them
            var l = GetEntities<Components>();
            for(int i = 0; i < l.Length; i++)
            {
                var e = l[i];
                TowerProjectile p = e.proj;

                //If the entity hasn't been activated before, initialize it now
                if (!p.activated)
                {
                    //If the tower is still null, wait for it and then activate the entity
                    if (p.tower == null)
                    {
                        GameMaster.gm.StartCoroutine(DelayedActivation(p));
                    }
                    //Otherwise just go ahead
                    else
                    {
                        ActualActivation(p);
                    }
                }
                //If the entity has already been initialized update it properly
                else
                {
                    Transform t = e.transform;

                    //If the target has been destroyed, destroy the projectile too
                    if (p.target == null)
                    {
                        GameMaster.Destroy(t.gameObject);
                    }
                    //Otherwise perform the main update step
                    else
                    {
                        //If the projectile is at a close proximity to its target then handle this like a collision. This is to avoid colliders/rigidbodies and hugely increase the performance
                        if (Vector3.Distance(t.position, p.target.position) < p.speed * Time.deltaTime || t.position == p.target.position)
                        {
                            GameMaster.gm.DestroyProjectile(p);
                            Enemy e_ = p.target.GetComponent<Enemy>();
                            GameMaster.gm.DamageEnemy(e_, p.dmg);
                            //GameMaster.gm.DestroyEnemy(p.target.GetComponent<Enemy>());
                        }
                        //Otherwise move the projectile towards its target
                        else
                        {
                            t.position = Vector3.MoveTowards(t.position, p.target.position, p.speed * Time.deltaTime);
                        }
                    }
                }                
            }
        }
    }

    protected IEnumerator DelayedActivation(TowerProjectile p)
    {
        yield return new WaitUntil(() => p.tower != null);

        ActualActivation(p);

        yield return null;
    }

    protected void ActualActivation(TowerProjectile p)
    {
        p.activated = true;
        p.speed = p.tower.projectileSpeed;
        p.dmg = p.tower.attackDamage;
    }


}