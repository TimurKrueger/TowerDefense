using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour {

    public float hpMax;
    public float hpCurrent;
    public float attackRange;

    public float power;

    protected NavMeshAgent agent;

    public Transform target;
    public float distanceOffset;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(target != null)
        {
            Ray ray = new Ray(transform.position, target.position - transform.position);
            RaycastHit[] info = Physics.RaycastAll(ray);
            foreach(RaycastHit rh in info){
                if(rh.transform == target)
                {
                    distanceOffset = Vector3.Distance(rh.point, rh.transform.position);
                    continue;
                }
            }

        }
    }

    private void Update()
    {
        RaycastHit hit;
        if (target.GetComponent<Collider>().Raycast(new Ray(transform.position, target.position - transform.position), out hit, attackRange + 2))//Vector3.Distance(transform.position, target.position) <= attackRange + distanceOffset * 1.1f)
        {
            Attack();
            agent.isStopped = true;
            transform.LookAt(target);
            //Debug.Log(Vector3.Distance(transform.position, target.position));
        } else
        {
            FindPath();
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, target.position - transform.position);
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }

    protected void FindPath()
    {
        agent.SetDestination(target.position);
    }

    void Attack()
    {

    }
}
