using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : Building {

    static bool showingRange = false;
    static Transform indicator;
    bool thisOneIsShowing = false;

    [Header("Tower Stats")]
    public float attacksPerSecond;
    public float attackRange; //Radius
    public float[] damages = new float[System.Enum.GetValues(typeof(GameMaster.ElementTypes)).Length];
    public float[] resistances = new float[System.Enum.GetValues(typeof(GameMaster.ElementTypes)).Length];

    protected Transform attackHolder;
    public Transform attackPrefab;
    public Transform towerBody;
    public Transform towerHead;
    public Transform target;
    List<Transform> enemiesInRange = new List<Transform>();

    public enum TargetModes
    {
        nearest,
        farthest,
        strongest,
        weakest,
        healthiest,
        mostfragile,
        random
    }

    public TargetModes targetMode;

    protected bool canAttack = true;

    private void OnMouseDown()
    {
        if (isActiveAndPlaced)
        {
            Vector3 indicatorNewPos = new Vector3(0.0f, -((gameObject.GetComponent<Renderer>().bounds.max.y - gameObject.GetComponent<Renderer>().bounds.min.y) / 2 / transform.lossyScale.y) + 0.06f, 0.0f);
            if (!showingRange)
            {
                if (indicator != null)
                    Destroy(indicator.gameObject);
                indicator = Instantiate(GameMaster.GetInstance().attackRangeIndicatorPrefab, transform.position, Quaternion.identity, this.transform);
                indicator.localPosition = indicatorNewPos;
                indicator.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
                indicator.localScale = new Vector3(attackRange / indicator.lossyScale.x, attackRange / indicator.lossyScale.y, attackRange / indicator.lossyScale.z);
                showingRange = true;
                thisOneIsShowing = true;

            }
            else
            {
                if (thisOneIsShowing)
                {
                    if (indicator != null)
                        Destroy(indicator.gameObject);
                    thisOneIsShowing = false;
                    showingRange = false;
                }
                else
                {
                    if (indicator != null)
                        Destroy(indicator.gameObject);
                    indicator = Instantiate(GameMaster.GetInstance().attackRangeIndicatorPrefab, transform.position, Quaternion.identity, this.transform);
                    indicator.localPosition = indicatorNewPos;
                    indicator.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
                    indicator.localScale = new Vector3(attackRange   / indicator.lossyScale.x, attackRange / indicator.lossyScale.y, attackRange / indicator.lossyScale.z);
                    showingRange = true;
                    thisOneIsShowing = true;
                }


            }
        }
    }

    private void Awake()
    {
        if(attackHolder == null)
        {
            //attackHolder = GameMaster.GetInstance().GetAttackHolder();
        }
    }

    public void Fire()
    {
        if (isActiveAndEnabled)
        {
            if (canAttack)
            {
                canAttack = false;
                countdown();

                CustomFire();
            }
        }
    }

    IEnumerator countdown()
    {
        yield return new WaitForSeconds(1.0f / attacksPerSecond);

        canAttack = true;

        yield return null;
    }

    private void Update()
    {
        if(target != null)
        {
            towerHead.LookAt(target);
            towerHead.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
        }
        if (enemiesInRange.Count > 0)
            SwitchTarget();
    }

    private void OnDrawGizmos()
    {
        if(target != null)
        {
            Debug.DrawLine(transform.position, target.position, Color.red);
        }
    }

    protected abstract void CustomFire();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.name);
        if (!enemiesInRange.Contains(other.transform))
        {
            enemiesInRange.Add(other.transform);
            SwitchTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemiesInRange.Contains(other.transform))
        {
            enemiesInRange.Remove(other.transform);
            if(target == other.transform)
                SwitchTarget();
        }
    }

    public void SwitchTarget()
    {
        Transform newTarget = target;
        float newDistance = 0;// = Mathf.Abs(Vector3.Distance(transform.position, target.position));
        float newPower = 0;// = target.GetComponent<Enemy>().power;
        float newHealth = 0;// = target.GetComponent<Enemy>().hpMax;
        if (newTarget != null)
        {
            newDistance = Mathf.Abs(Vector3.Distance(transform.position, target.position));
            newPower = target.GetComponent<Enemy>().power;
            newHealth = target.GetComponent<Enemy>().hpMax;
        }

        switch (targetMode)
        {
            case TargetModes.nearest:
                foreach(Transform t in enemiesInRange){
                    float tempDist = Mathf.Abs(Vector3.Distance(transform.position, t.position));
                    if (newTarget == null || tempDist < newDistance)
                    {
                        newTarget = t;
                        newDistance = tempDist;
                    }
                }
                break;
            case TargetModes.farthest:
                foreach (Transform t in enemiesInRange)
                {
                    float tempDist = Mathf.Abs(Vector3.Distance(transform.position, t.position));
                    if (newTarget == null || tempDist > newDistance)
                    {
                        newTarget = t;
                        newDistance = tempDist;
                    }
                }
                break;
            case TargetModes.strongest:
                foreach (Transform t in enemiesInRange)
                {
                    float tempPower = t.GetComponent<Enemy>().power;
                    if (newTarget == null || tempPower > newPower)
                    {
                        newTarget = t;
                        newPower = tempPower;
                    }
                }
                break;
            case TargetModes.weakest:
                foreach (Transform t in enemiesInRange)
                {
                    float tempPower = t.GetComponent<Enemy>().power;
                    if (newTarget == null || tempPower < newPower)
                    {
                        newTarget = t;
                        newPower = tempPower;
                    }
                }
                break;
            case TargetModes.healthiest:
                foreach (Transform t in enemiesInRange)
                {
                    float tempHealth = t.GetComponent<Enemy>().hpCurrent;
                    if (newTarget == null || tempHealth > newHealth)
                    {
                        newTarget = t;
                        newHealth = tempHealth;
                    }
                }
                break;
            case TargetModes.mostfragile:
                foreach (Transform t in enemiesInRange)
                {
                    float tempHealth = t.GetComponent<Enemy>().hpCurrent;
                    if (newTarget == null || tempHealth < newHealth)
                    {
                        newTarget = t;
                        newHealth = tempHealth;
                    }
                }
                break;
            case TargetModes.random:

                int rIndex = Random.Range(0, enemiesInRange.Count - 1);
                newTarget = enemiesInRange[rIndex];

                break;
            default:
                break;
        }
        target = newTarget;
    }
}
