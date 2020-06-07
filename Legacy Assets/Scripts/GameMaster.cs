using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MonoBehaviour Singleton. Needs to be put on a single object though!
public class GameMaster : MonoBehaviour {

    #region Manage Singleton
    static GameMaster instance;

    public static GameMaster GetInstance()
    {
        return instance;
    }

    void ManageSingleton()
    {
        if (GetInstance() == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    private void Awake()
    {
        ManageSingleton();
    }

    #region Attributes
    public enum ElementTypes
    {
        physical,
        arcane,
        poison,
        earth,
        fire,
        ice,
        light,
        dark
    }

    [Header("Building Stuff")]
    public Vector2Int gridSize;
    public Transform[] buildingPrefabs;
    public bool[] unlockedBuilding;

    public Transform buildingOnCursor;
    public Transform grid3dPrefab;

    [Header("Tower Stuff")]
    private Transform attackHolder;
    public Transform GetAttackHolder()
    {
        Debug.Log("Hi!");
        return ((attackHolder == null) ? (new GameObject("Attack_Holder").transform) : (attackHolder));
    }
    public Transform attackRangeIndicatorPrefab;

    [Header("Current Level Data")]
    public Terrain terrain;
    public float energySupplyMax;
    public float energySupplyCurrent;
    public float manaSupplyMax;
    public float manaSupplyCurrent;
    public List<Building> buildings = new List<Building>();

    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetBuilding();
        }
    }

    private void LateUpdate()
    {
        UpdateBuildingOnCursorPosition();
    }

    #region Building Stuff

    public void ToggleGrid(Transform target)
    {
        if (target.GetComponent<Grid3D>() != null)
        {
            //Debug.Log("Stopping");
            target.GetComponent<Grid3D>().Stop();
        }
        else
        {    
            Renderer r = (target.GetComponent<Tower>() != null) ? target.GetComponent<Tower>().towerBody.GetComponent<Renderer>() : target.GetComponent<Renderer>();
            int width = (int) (r.bounds.max.x - r.bounds.min.x);
            int length = (int) (r.bounds.max.z - r.bounds.min.z);
            int height = (int) (r.bounds.max.y - r.bounds.min.y);
            target.gameObject.AddComponent<Grid3D>().resize(width, length, height);
        }
    }

    public void SetBuilding()
    {
        if (buildingOnCursor != null)
        {
            buildings.Add(buildingOnCursor.GetComponent<Building>());
            buildingOnCursor.GetComponent<Building>().Settle();
            buildingOnCursor = null;
        }
    }

    public void DestroyBuilding(Building target)
    {
        if (buildings.Contains(target))
        {
            buildings.Remove(target);
        }
        Destroy(target.gameObject);
    }

    public void SetBuildingOnCursor(int index)
    {
        if(buildingOnCursor != null)
        {
            Destroy(buildingOnCursor.gameObject);
        }

        if (unlockedBuilding[index])
        {
            buildingOnCursor = Instantiate(buildingPrefabs[index]);
        }
    }
    
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

            yOffset = terrain.SampleHeight(finalPoint) + (buildingOnCursor.GetComponent<Renderer>().bounds.max.y - buildingOnCursor.GetComponent<Renderer>().bounds.min.y)/2;

            Vector3 newCursorObjPos = new Vector3(finalPoint.x, yOffset, finalPoint.z); 

            buildingOnCursor.position = newCursorObjPos;

            //buildingOnCursor.position = newPos;
        }
    }

    #endregion

    #region Manage Buildings
    public void ManageEnergyAndMana()
    {
        energySupplyCurrent = energySupplyMax;
        manaSupplyCurrent = manaSupplyMax;

        foreach(Building b in buildings)
        {
            b.UpdateConsumption();
        }
    }

    #endregion

}
