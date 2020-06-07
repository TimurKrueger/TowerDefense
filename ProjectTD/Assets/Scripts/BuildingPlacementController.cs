using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementController : MonoBehaviour {

    public Vector2Int sizeForGrid = new Vector2Int(4, 4);
    public List<Component> cTDOA = new List<Component>(); // = components to delete on activation
    public List<Component> cTAOD = new List<Component>(); // = components to activate on activation
    public List<Transform> collisions = new List<Transform>();
    Color rendererColor = Color.white;
    public Material notPlaceable;
    Material oldMat;
    public Renderer r;
    public bool evenGround = false;

    private void Awake()
    {
        GridMaster.gm.target = this.transform;
        GridMaster.gm.width = sizeForGrid.x + 4;
        GridMaster.gm.length = sizeForGrid.y + 4;
        GridMaster.gm.GenerateGrid();
        GridMaster.gm.ShowGrid();
    }

    private void Update()
    {
        Vector3 pointBL = transform.position - Vector3.right * sizeForGrid.x / 2 * 0.75f - Vector3.forward * sizeForGrid.y / 2 * 0.75f;
        Vector3 pointBR = transform.position + Vector3.right * sizeForGrid.x / 2 * 0.75f - Vector3.forward * sizeForGrid.y / 2 * 0.75f;
        Vector3 pointTL = transform.position - Vector3.right * sizeForGrid.x / 2 * 0.75f + Vector3.forward * sizeForGrid.y / 2 * 0.75f;
        Vector3 pointTR = transform.position + Vector3.right * sizeForGrid.x / 2 * 0.75f + Vector3.forward * sizeForGrid.y / 2 * 0.75f;

        RaycastHit hitBL;
        RaycastHit hitBR;
        RaycastHit hitTL;
        RaycastHit hitTR;

        Ray rayBL = new Ray(pointBL + Vector3.up * 100.0f, -Vector3.up * 100.0f);
        Ray rayBR = new Ray(pointBR + Vector3.up * 100.0f, -Vector3.up * 100.0f);
        Ray rayTL = new Ray(pointTL + Vector3.up * 100.0f, -Vector3.up * 100.0f);
        Ray rayTR = new Ray(pointTR + Vector3.up * 100.0f, -Vector3.up * 100.0f);

        Physics.Raycast(rayBL, out hitBL, 4096);
        Physics.Raycast(rayBR, out hitBR, 4096);
        Physics.Raycast(rayTL, out hitTL, 4096);
        Physics.Raycast(rayTR, out hitTR, 4096);

        float tolerance = 0.1f;

        if(GameMaster.Approx(hitBL.point.y, hitBR.point.y, tolerance) && GameMaster.Approx(hitBL.point.y, hitTL.point.y, tolerance) && GameMaster.Approx(hitTR.point.y, hitBR.point.y, tolerance)) //hitBL.point.y == hitBR.point.y && hitBL.point.y == hitTL.point.y && hitBL.point.y == hitTR.point.y)
        {
            evenGround = true;
        } else
        {
            evenGround = false;
        }

        if (true)
        {
            //Debug.Log("BL.y = " + hitBL.point.y + "; BR.y = " + hitBR.point.y + "; TL.y = " + hitTL.point.y + "; TR.y = " + hitTR.point.y);
            Debug.DrawRay(pointBL + Vector3.up * 100.0f, -Vector3.up * 100.0f, Color.green);
            Debug.DrawRay(pointBR + Vector3.up * 100.0f, -Vector3.up * 100.0f, Color.green);
            Debug.DrawRay(pointTL + Vector3.up * 100.0f, -Vector3.up * 100.0f, Color.green);
            Debug.DrawRay(pointTR + Vector3.up * 100.0f, -Vector3.up * 100.0f, Color.green);

            Vector3 offset_x = Vector3.right * 0.5f;
            Vector3 offset_z = Vector3.forward * 0.5f;

            Debug.DrawLine(hitBL.point - offset_x, hitBL.point + offset_x, Color.red);
            Debug.DrawLine(hitBL.point - offset_z, hitBL.point + offset_z, Color.red);

            Debug.DrawLine(hitBR.point - offset_x, hitBR.point + offset_x, Color.red);
            Debug.DrawLine(hitBR.point - offset_z, hitBR.point + offset_z, Color.red);

            Debug.DrawLine(hitTL.point - offset_x, hitTL.point + offset_x, Color.red);
            Debug.DrawLine(hitTL.point - offset_z, hitTL.point + offset_z, Color.red);

            Debug.DrawLine(hitTR.point - offset_x, hitTR.point + offset_x, Color.red);
            Debug.DrawLine(hitTR.point - offset_z, hitTR.point + offset_z, Color.red);
        }
        //float yOffset = (int)hit.point.y;
        //yOffset = 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Building" && !collisions.Contains(collision.transform))
        {
            if (collisions.Count == 0)
            {
                oldMat = r.material;
                r.material = notPlaceable;
            }

            collisions.Add(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisions.Contains(collision.transform))
        {
            collisions.Remove(collision.transform);

            if (collisions.Count == 0)
            {
                r.material = oldMat;
            }
        }
    }

    public bool Place()
    {
        if(collisions.Count != 0)
        {
            return false;
        }

        foreach(Component c in cTAOD)
        {
            if (c is Collider)
            {
                (c as Collider).enabled = true;
            }
            else if (c is Behaviour)
            {
                (c as Behaviour).enabled = true;
            }
        }

        foreach (Component c in cTDOA)
        {
            Destroy(c);
        }

        GetComponent<Rigidbody>().isKinematic = true;
        GridMaster.gm.HideGrid();
        StartCoroutine(SelfDestruct());

        return true;
    }

    public bool CheckIfLocationViable()
    {
        if(collisions.Count != 0)
        {
            rendererColor = r.material.color;
            r.material.color = Color.red;
            //Debug.Log("Not viable!");
            return false;
        }

        //Debug.Log("viable!");
        return true;
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForEndOfFrame();

        Destroy(this);

        yield return null;
    }
}
