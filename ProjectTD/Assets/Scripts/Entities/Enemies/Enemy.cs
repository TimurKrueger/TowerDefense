using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;

public class Enemy : Entity {

    [Header("Enemy Attributes")]
    public NavMeshAgent agent;
    public NavMeshPath path;
    public Transform target;
    public int initialized = 0;
    public int step = 0;
    public static int maxStep = 15;
    public long counterID = -1;
    public int id = 0;
    public float totalDist = -1.0f;
    public float currDist = -1.0f;
    public float remDist = -1.0f;
    public Vector3 lastPos = Vector3.zero;
    public List<Tower> targetedBy = new List<Tower>();
    public bool trigger = false;
}

class EnemySystem : ComponentSystem
{
    struct Components
    {
        public Enemy enemy;
        public Transform transform;
        public NavMeshAgent agent;
        public BoxCollider collider;
    }
    
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
    }
    
    protected override void OnUpdate()
    {
        foreach (var e in GetEntities<Components>())
        {
            Enemy e_ = e.enemy;
            if (e_.initialized == 0 && GameMaster.gm != null)
            {
                GameMaster.gm.InitializeEnemy(e_, e.agent);/*
                e_.initialized = true;
                e_.agent = e.agent;
                e_.target = GameMaster.gm.enemyTarget;
                e_.hp = e_.hpMax;
                e.agent.SetDestination(e_.target.position);
                if(e_.counterID == -1L) e_.counterID = GameMaster.gm.activeEnemiesCount;*/
                //GameMaster.gm.activeEnemiesCount++;
                //GameMaster.gm.activeEnemies.Add(e.transform);
            }
            else if (e_.initialized == 1)
            {

                e.agent.SetDestination(e_.target.position);

                NavMeshPath path = e_.agent.path;

                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    e_.totalDist += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }

                e_.initialized++;
            }
            else
            {
                //e_.currDist += Vector3.Distance(e.transform.position, e_.lastPos); // e.agent.velocity;
                //e_.lastPos = e.transform.position;

                // e_.remDist = e_.totalDist - e_.currDist;
                //e_.remDist = 0;
                
                for (int i = 0; i < e.agent.path.corners.Length - 1; i++)
                {
                    Debug.DrawLine(e.agent.path.corners[i], e.agent.path.corners[i + 1], Color.red);
                    //e_.remDist += Vector3.Distance(e.agent.path.corners[i], e.agent.path.corners[i + 1]);
                }
            }


            UpdateTarget(e_);
        }
    }

    protected void UpdateTarget(Enemy e)
    {
        if(e.trigger)
        {
            e.trigger = false;

            e.agent.SetDestination(e.target.position);

            Debug.Log(e.agent.hasPath +"_"+e.agent.path.corners.Length);
        }
        e.step++;
        if(e.step > Enemy.maxStep)
        {
            e.step = 0;
            if (Vector3.Distance(e.transform.position, e.target.position) < 10f)
            {
                GameMaster.gm.DestroyEnemy(e);//StartCoroutine(GameMaster.gm.DisableEnemy(e));
                //GameMaster.gm.MarkForDelete(e.gameObject);
                //GameMaster.gm.activeEnemiesCount--;
                //e.transform.name = "Enemy_"+e.remDist;
            } else
            {
                Vector3[] corners = e.agent.path.corners;
                e.remDist = 0;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    //Debug.DrawLine(e.agent.path.corners[i], e.agent.path.corners[i + 1], Color.red);
                    e.remDist += Vector3.Distance(corners[i], corners[i + 1]);
                }
            }
        }
    }  

}