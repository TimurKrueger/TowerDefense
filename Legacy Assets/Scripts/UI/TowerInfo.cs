using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInfo : MonoBehaviour {

    Transform cam;

	void Start () {
        cam = Camera.main.transform;
	}

    private void LateUpdate()
    {
        transform.LookAt(cam.transform.position);
    }
}
