using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TowerTargetMode
{
    first,
    last,
    random,
    closest,
    furthest
}

public enum TowerTag
{
    projectile,
    particle,
    fire,
    water,
    ice,
    metal,
    lightning,
    light,
    darkness

}

public abstract class Tower : Building {

    [Header("Attributes, etc...")]
    public float aps = 1.0f; // Attacks per second
    public float attackDamage = 1.0f;
    public float attackRange = 10.0f; // Sphere Collider Range; <= 0 = No Range Update
    public bool needsDirectVision = false; // If the tower should be able to shoot through walls/etc... if the enemy is in range
    public List<TowerTag> tags = new List<TowerTag>();

    [Header("Management Stats, etc...")]
    public int killCount = 0;
    public long earnedScore = 0L;
    public TowerTargetMode targetMode;
    public Transform target;

    [Header("Background Variables")]
    public Vector3 shootingSpot = Vector3.zero;
    public List<Transform> enemiesInRange = new List<Transform>();
    SphereCollider attackRangeTrigger;
    public bool canShoot = true;

    private void OnDrawGizmosSelected()
    {
        if (target != null) Debug.DrawLine(transform.position, target.position, Color.red);
        foreach(Transform t in enemiesInRange)
        {
            Debug.DrawLine(transform.position, t.position, Color.green);
        }
    }

    protected void Awake()
    {
        attackRangeTrigger = GetComponent<SphereCollider>();
        if(attackRange > 0f) attackRangeTrigger.radius = attackRange;

        DebuffManager.ApplyDebuff(this, DebuffList.PhysicalDamageOverTime, 10.0f, 1.0f);
    }

    protected void Update()
    {
        if(canShoot && target != null)
        {
            if (target.gameObject.activeSelf) Fire();
            else target = null;
        }
    }

    protected void LateUpdate()
    {
        for(int i = 0; i < debuffs.Count; i++) {
            bool stays = debuffs[i].Trigger(Time.deltaTime);

            if (!stays)
            {
                //Debug.Log("Removed the Debuff: " + debuffs[i].ToString());
                debuffs.RemoveAt(i);
                i--;
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !enemiesInRange.Contains(other.transform) && (!needsDirectVision || Physics.Raycast(transform.position + shootingSpot, other.transform.position - transform.position)))
        {
            //Debug.Log("Added new Enemy to EIR List");
            other.GetComponent<Enemy>().targetedBy.Add(this);

            //Sort / Update Target
            /*float otherCurrDist = other.GetComponent<Enemy>().currDist;//other.GetComponent<NavMeshAgent>().remainingDistance;
            float targetCurrDist = (target != null) ? target.GetComponent<Enemy>().currDist : -1.0f;//target.GetComponent<NavMeshAgent>().remainingDistance;

            Debug.Log("ORD: " + otherCurrDist + "; " + enemiesInRange.Count + "; " + ((target != null) ? target.name : ""));
            if (target == null || enemiesInRange.Count == 0)
            {
                target = other.transform;
                enemiesInRange.Add(other.transform);
            }
            else if (otherCurrDist > targetCurrDist)
            {
                enemiesInRange.Insert(0, other.transform);
                UpdateTarget();
            }
            else if (otherCurrDist < enemiesInRange[enemiesInRange.Count - 1].GetComponent<Enemy>().currDist)//enemiesInRange[enemiesInRange.Count - 1].GetComponent<NavMeshAgent>().remainingDistance)
            {
                enemiesInRange.Add(other.transform);
                UpdateTarget();
            }
            else
            {
                for (int i = enemiesInRange.Count; i > 1; i--)
                {
                    if (otherCurrDist >= enemiesInRange[i - 1].GetComponent<Enemy>().currDist)
                    {
                        enemiesInRange.Insert(i, other.transform);
                        i = -1;
                    }
                }
                UpdateTarget();
            }*/
            /*
            float otherRemDist = other.GetComponent<Enemy>().remDist;
            float targetRemDist = (target != null) ? target.GetComponent<Enemy>().remDist : -1.0f;

            //Debug.Log("ORD: " + otherRemDist + "; " + enemiesInRange.Count + "; " + ((target != null) ? target.name : ""));

            if (target == null || enemiesInRange.Count == 0)
            {
                target = other.transform;
                enemiesInRange.Add(other.transform);
            }
            else if (otherRemDist < targetRemDist)
            {
                enemiesInRange.Insert(0, other.transform);
                UpdateTarget();
            }
            else if (otherRemDist > enemiesInRange[enemiesInRange.Count - 1].GetComponent<Enemy>().currDist)//enemiesInRange[enemiesInRange.Count - 1].GetComponent<NavMeshAgent>().remainingDistance)
            {
                enemiesInRange.Add(other.transform);
                UpdateTarget();
            }
            else
            {
                /*for(int i = enemiesInRange.Count; i > 1; i--)
                {
                    if(otherRemDist >= enemiesInRange[i-1].GetComponent<Enemy>().remDist)
                    {
                        enemiesInRange.Insert(i, other.transform);
                    }
                }*/
                enemiesInRange.Add(other.transform);
                enemiesInRange.Sort(SortByDistance);
                UpdateTarget();
            //}
               //enemiesInRange.Add(other.transform);
               //enemiesInRange.Sort(SortByDistance);
               //UpdateTarget();
        }
    }

    public void UpdateTarget()
    {
        switch (targetMode)
        {
            case TowerTargetMode.first:
                target = enemiesInRange[0];
                break;
        }
    }

    protected static int SortByDistance(Transform a1, Transform a2)
    {
        if (a1 != null && a2 != null)
            return a1.GetComponent<Enemy>().remDist.CompareTo(a2.GetComponent<Enemy>().remDist);
        else
            return 0;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (enemiesInRange.Contains(other.transform))
        {
            //Debug.Log("Removed enemy from EIR list!");
            other.GetComponent<Enemy>().targetedBy.Remove(this);
            enemiesInRange.Remove(other.transform);
            if(enemiesInRange.Count > 0 && (target == null || target.Equals(other.transform)))
            {
                UpdateTarget();
            } else
            {
                target = null;
            }
        }
    }

    public void Fire()
    {
        FireFunctionality();
    }

    protected abstract void FireFunctionality();

    protected IEnumerator Cooldown(float coolDown)
    {
        canShoot = false;
        yield return new WaitForSeconds(coolDown);
        canShoot = true;
        yield return null;
    }

    public void RemoveEnemyFromTargetables(Transform t)
    {
        enemiesInRange.Remove(t);
        if (target == t && enemiesInRange.Count > 0)
        {
            UpdateTarget();
        }
    }
}
