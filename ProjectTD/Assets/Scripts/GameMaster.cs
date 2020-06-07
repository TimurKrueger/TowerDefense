using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameMaster : MonoBehaviour {

    [Header("Background Variables")]
    public CameraController cameraController;
    
    public Transform[] enemyEntrances;
    public Transform enemyTarget;
    public Transform enemyTestPrefab;
    public List<Transform> activeEnemies = new List<Transform>();
    public List<List<Transform>> enemyBin = new List<List<Transform>>();

    [Header("Building Management")]
    public Transform buildingOnCursor;
    public Vector2Int gridSize = new Vector2Int(1, 1);
    public List<Transform> buildingPrefabs = new List<Transform>();

    [Header("Level, Wave and Enemy Management")]
    public int currentLevel = 0;
    public int currentWave = 1;
    public int remainingEnemies = 0;
    public List<Entry> enemiesToSpawn = new List<Entry>();

    [Header("Holders / Sorters")]
    public Transform enemyHolder;
    public Transform towerHolder;
    public Transform projectileHolder;
    public Transform levelHolder;
    public LevelManager levelManager;
    //public Transform sceneHolder;

    [Header("Debug Variables")]
    public int activeEnemiesCount = 0;
    public int totalEnemies = 0;
    public float timeScale = 1.0f;
    public bool autoSpawn = false;
    public int spawnsPerSecond = 10;
    float spawnCounter = 0;

    #region Singleton Management & Initialization

    public static GameMaster gm;

    private void Awake()
    {
        if(gm == null)
        {
            gm = this;
        }
        else if(gm != this)
        {
            Destroy(this);
        }
        for(int i = 0; i < 1; i++)
        {
            enemyBin.Add(new List<Transform>());
        }
        if(enemyHolder == null)
        {
            enemyHolder = new GameObject("EnemyHolder")/*.AddComponent<EnemyMaster>()*/.transform;
        }
        if(towerHolder == null)
        {
            towerHolder = new GameObject("TowerHolder").transform;
        }
        if (projectileHolder == null)
        {
            projectileHolder = new GameObject("ProjectileHolder").transform;
        }
        if (levelHolder == null)
        {
            levelHolder = new GameObject("LevelHolder").transform;
        }
        if(levelManager == null)
        {
            levelManager = new GameObject("LevelManager").AddComponent<LevelManager>();
        }
        /*
        if (sceneHolder == null)
        {
            sceneHolder = new GameObject("SceneHolder").transform;
            enemyHolder.parent = sceneHolder;
            towerHolder.parent = sceneHolder;
            projectileHolder.parent = sceneHolder;
            levelHolder.parent = sceneHolder;
            this.transform.parent = sceneHolder;
        }*/
        LevelSetup(0);
    }
    #endregion
    
    public static bool Approx(Vector3 a, Vector3 b, float tolerance)
    {
        return Mathf.Abs(a.x - b.x) <= tolerance && Mathf.Abs(a.y - b.y) <= tolerance && Mathf.Abs(a.z - b.z) <= tolerance;
    }

    public static bool Approx(float a, float b, float tolerance)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }

    private void Update()
    {
        if(Input.GetAxisRaw("Horizontal") != 0.0f) {
            cameraController.Move((int)Mathf.Sign(Input.GetAxisRaw("Horizontal")) * -1 + 2);
        }
        if (Input.GetAxisRaw("Vertical") != 0.0f)
        {
            cameraController.Move((int)Mathf.Sign(Input.GetAxisRaw("Vertical")) * -1 + 1);
        }
        if(Input.GetAxisRaw("Mouse ScrollWheel") != 0.0f)
        {
            cameraController.Move((int)(Mathf.Sign(Input.GetAxisRaw("Mouse ScrollWheel")) * -1 / 2 + 4.5f));
        }
        
        #region Building Management
        if(buildingOnCursor != null)
        {
            UpdateBuildingOnCursorPosition();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(buildingOnCursor != null)
            {
                buildingOnCursor.GetComponent<BuildingPlacementController>().Place();
                buildingOnCursor = null;
            }
        }
        #endregion

        #region Temporärer/s Debug - Input / Handling
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 10; i++) SpawnEnemy(0);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            autoSpawn = !autoSpawn;
        }
        if (autoSpawn)
        {
            spawnCounter -= Time.deltaTime;
            if (spawnCounter <= 0)
            {
                for (int i = 0; i < spawnsPerSecond; i++) SpawnEnemy(0);
                spawnCounter = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Time.timeScale *= 2.0f;
            timeScale = Time.timeScale;
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            Time.timeScale /= 2.0f;
            timeScale = Time.timeScale;
        }
        #endregion
    }

    #region Building Management Methods
    void UpdateBuildingOnCursorPosition()
    {
        if (buildingOnCursor != null)
        {
            Vector3 newPos = buildingOnCursor.position;

            float yOffset = 0.0f;

            // this creates a horizontal plane passing through this object's center
            Plane plane = new Plane(Vector3.up, transform.position);
            // create a ray from the mousePosition
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // plane.Raycast returns the distance from the ray start to the hit point
            float distance;
            Vector3 hitPoint = Vector3.zero;

            if (plane.Raycast(ray, out distance))
            {
                // some point of the plane was hit - get its coordinates
                hitPoint = ray.GetPoint(distance);
                // use the hitPoint to aim your cannon
            }

            hitPoint.x = Mathf.Round(hitPoint.x) - (hitPoint.x % gridSize.x);
            hitPoint.z = Mathf.Round(hitPoint.z) - (hitPoint.y % gridSize.y);

            Vector3Int finalPoint = new Vector3Int(Mathf.RoundToInt(hitPoint.x), 0, Mathf.RoundToInt(hitPoint.z));

            finalPoint.y = 100;

            RaycastHit hit;
            Ray finalRay = new Ray(finalPoint, Vector3.down);
            Physics.Raycast(finalRay, out hit, 100.0f, 4096);


            yOffset = (int)hit.point.y;
            //yOffset = 
            //yOffset = terrain.SampleHeight(finalPoint) + (buildingOnCursor.GetComponent<Renderer>().bounds.max.y - buildingOnCursor.GetComponent<Renderer>().bounds.min.y) / 2;

            Vector3 newCursorObjPos = new Vector3(finalPoint.x, yOffset, finalPoint.z);

            buildingOnCursor.position = newCursorObjPos;
            buildingOnCursor.GetComponent<BuildingPlacementController>().CheckIfLocationViable();
            //buildingOnCursor.position = newPos;
        }
    }

    public void SetBuildingOnCursor(int index)
    {
        if (index > buildingPrefabs.Count - 1)
        {
            Debug.LogError("Building Prefab Index not defined!");
        }

        if (buildingOnCursor != null)
        {
            Destroy(buildingOnCursor);
        }

        buildingOnCursor = Instantiate(buildingPrefabs[index]);
    }
    #endregion

    #region Level, Wave and Enemy Management Methods

    public void LevelSetup(int index)
    {
        levelManager.LoadLevel(index);
        currentWave = 1;

        StartCoroutine(SpawnNextWave());
    }

    public IEnumerator SpawnNextWave()
    {
        yield return new WaitForSeconds(2.5f);

        Entry[] entries = levelManager.GetNextWave();

        for (int i = 0; i < entries.Length; i++)
            enemiesToSpawn.Add(entries[i]);

        enemiesToSpawn.Sort(SortWavesByDelay);

        remainingEnemies = 0;
        //Debug.Log("Spawning Wave " + currentWave);

        for(int i = 0; i < enemiesToSpawn.Count; i++)
        {
            remainingEnemies += enemiesToSpawn[i].enemyCount;
            //Debug.Log("Entry " + i + " of wave " + currentWave + " contains " + enemiesToSpawn[i].enemyCount + " Enemies with ID " + enemiesToSpawn[i].enemyID + " will be spawned after a delay of " + enemiesToSpawn[i].delay + " seconds.");
            if(enemiesToSpawn[i].delay > 0.0f)
            {
                StartCoroutine(DelayedSpawn(enemiesToSpawn[i]));
            } else
            {
                for(int j = 0; j < enemiesToSpawn[i].enemyCount; j++)
                {
                    SpawnEnemy(enemiesToSpawn[i].enemyID);
                }
            }
        }

        
        //remainingEnemies = 0;

        yield return new WaitUntil(() => remainingEnemies == 0);

        currentWave++;

        if(currentWave <= levelManager.currentLevel.waveCount)
        {
            //Debug.Log("Congrats! Wave beaten! Wave " + currentWave + " of " + levelManager.currentLevel.waveCount + " incoming!");

            enemiesToSpawn.Clear();
            /*
            for (int i = 0; i < levelManager.GetNextWave().Length; i++)
                enemiesToSpawn.Add(levelManager.GetNextWave()[i]);*/

            StartCoroutine(SpawnNextWave());

        } else
        {
            //Debug.Log("Congrats you have finished the level successfully!!!");
        }

        yield return null;
    }


    protected static int SortWavesByDelay(Entry e1, Entry e2)
    {
        return e1.delay.CompareTo(e2.delay);
    }

    public IEnumerator DelayedSpawn(Entry e)
    {
        yield return new WaitForSeconds(e.delay);

        for(int i = 0; i < e.enemyCount; i++)
        {
            SpawnEnemy(e.enemyID);
        }

        yield return null;
    }

    /// <summary>
    /// ID determines which enemy instance will be created
    /// ID values:
    /// 0 = Test Enemy
    /// </summary>
    /// <param name="id"></param>
    public void SpawnEnemy(int id)
    {
        if(enemyBin[id].Count > 0)
        {
            Transform t = enemyBin[id][0];
            enemyBin[id].RemoveAt(0);

            t.position = enemyEntrances[Random.Range(0, enemyEntrances.Length)].position;
            t.gameObject.SetActive(true);
            t.GetComponent<Enemy>().initialized = 0;
            activeEnemies.Add(t);
            activeEnemiesCount++;
        }
        else
        {
            Transform t = Instantiate(enemyTestPrefab);
            t.name = "Enemy_" + totalEnemies;
            t.position = enemyEntrances[Random.Range(0, enemyEntrances.Length)].position;
            activeEnemies.Add(t);
            activeEnemiesCount++;
            totalEnemies++;
            t.parent = enemyHolder;
        }
    }

    IEnumerator DisableEnemy(Enemy e)
    {
        yield return new WaitForEndOfFrame();

        enemyBin[e.id].Add(e.transform);
        e.gameObject.SetActive(false);
        activeEnemies.Remove(e.transform);
        activeEnemiesCount--;

        yield return null;
    }

    public void DestroyEnemy(Enemy e)
    {
        foreach (Tower t in e.targetedBy) t.RemoveEnemyFromTargetables(e.transform);
        StartCoroutine(DisableEnemy(e));
        remainingEnemies--;
    }

    IEnumerator InitEnemy(Enemy e, NavMeshAgent agent)
    {
        e.initialized = 1;
        e.agent = agent;
        e.target = GameMaster.gm.enemyTarget;

        NavMeshPath path = new NavMeshPath();

        //bool couldCreate = e.agent.CalculatePath(e.target.position, path);
        /*while (!couldCreate)
        {
            couldCreate = e.agent.CalculatePath(e.target.position, path);
            Debug.Log(couldCreate);
            yield return new WaitForEndOfFrame();
        }*/
        //Debug.Log("Calculated path with " + path.corners.Length + " corners!");
        float totalDistance = 0.0f;
        /*
        for(int i = 0; i < path.corners.Length-1; i++)
        {
            totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }*/

        e.totalDist = totalDistance;
        e.currDist = 0.0f;
        e.remDist = totalDistance;
        e.agent.path = path;
        /*
        e.agent.SetDestination(e.target.position);
        float speed = e.agent.speed;
        e.agent.speed = 0;*/

        //yield return new WaitUntil(() => e.agent.remainingDistance == e.agent.remainingDistance && e.agent.remainingDistance != Mathf.Infinity);

        //e.agent.speed = speed;

        e.hp = e.hpMax;
        if (e.counterID == -1L) e.counterID = GameMaster.gm.activeEnemiesCount;

        yield return null;
    }

    public void InitializeEnemy(Enemy e, NavMeshAgent agent)
    {
        StartCoroutine(InitEnemy(e, agent));
    }

    public void DamageEnemy(Enemy e, float dmg)
    {
        e.hp -= dmg;
        if (e.hp <= 0)
        {
            DestroyEnemy(e);
        }
    }
    
    #endregion

    #region Projectile Management Methods

    IEnumerator DisableProjectile(TowerProjectile p)
    {
        yield return new WaitForEndOfFrame();

        if(p.childEmitterHolder != null)
        {
            p.childEmitterHolder.parent = projectileHolder;
            p.childDestroyer.Activate();
        }
        Destroy(p.gameObject);

        yield return null;
    }

    public void DestroyProjectile(TowerProjectile p)
    {
        StartCoroutine(DisableProjectile(p));
    }

    #endregion

}
