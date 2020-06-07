using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Increase Bounds so the Mesh will always be rendered
/// </summary>
public class BoundsMaster : MonoBehaviour {

    private void Start()
    {
        Mesh m = GetComponent<MeshFilter>().mesh;
        m.bounds = new Bounds(Vector3.zero, Vector3.one * 2000);
    }
}