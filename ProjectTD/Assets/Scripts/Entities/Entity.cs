using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

    [Header("Entity Attributes")]
    public float hpMax = 10.0f;
    public float hp = 10.0f;
    public List<Debuff> debuffs = new List<Debuff>();
    public List<string> debuffData = new List<string>();
}
