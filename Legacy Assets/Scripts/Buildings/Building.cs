using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour {

    [Header("Building Stats")]
    public string buildingName = "Building Template Name";
    public string buildingDescription = "Building Template Description";
    public float hpMax;
    public float hpCurrent;
    public int aggroValue;
    public float energyConsumption;
    public float manaConsumption;

    protected bool isActiveAndPlaced = false;

    private void Start()
    {
        //GameMaster.GetInstance().ToggleGrid(this.transform);
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
            GameMaster.GetInstance().ToggleGrid(this.transform);

    }

    public void UpdateConsumption()
    {
        GameMaster.GetInstance().energySupplyCurrent -= energyConsumption;
        GameMaster.GetInstance().manaSupplyCurrent -= manaConsumption;
    }

    public void Settle()
    {
        //Debug.Log("Settling!");
        GameMaster.GetInstance().ToggleGrid(this.transform);
        isActiveAndPlaced = true;
    }

    public void DestroyBuilding()
    {
        GameMaster.GetInstance().DestroyBuilding(this);
    }
}
