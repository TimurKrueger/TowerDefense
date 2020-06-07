using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaster : MonoBehaviour {

    public int width = 5;
    public int length = 5;

    public Vector2 perSquareOffset = new Vector2(1.0f, 1.0f);

    public Material gridMaterial;

    public Sprite[] gridSprites;

    Transform[,] gridSquares;
    Transform gridHolder;

    public Transform target;

    public Material defaultMat;

    #region Singleton Management & Initialization

    public static GridMaster gm;

    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
        else if (gm != this)
        {
            Destroy(this);
        }
        //ShowGrid();
    }
    #endregion

    /*
    private void LateUpdate()
    {
        if(target != null)
        {
            Debug.Log(target.position.y);
            gridHolder.localPosition = target.position - new Vector3(perSquareOffset.x/2 * width - perSquareOffset.x/2, target.position.y + 0.1f, perSquareOffset.y/2 * length - perSquareOffset.y/2);
        }
    }*/

    /// <summary>
    /// Generates a grid dependent on the currently set width, length and perSquareOffset of the GridMaster
    /// </summary>
    public void GenerateGrid()
    {
        if (gridSquares != null)
        {
            foreach (Transform t in gridSquares)
            {
                Destroy(t.gameObject);
            }
        }

        gridSquares = new Transform[width, length];

        if (gridHolder == null)
        {
            gridHolder = new GameObject("GridHolder").transform;
            gridHolder.parent = this.transform;
        }

        Vector3 offset = new Vector3(Mathf.Floor(width / 2) * perSquareOffset.x, 0.0f, Mathf.Floor(length / 2) * perSquareOffset.y);
        gridHolder.position = transform.position - offset;

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < length; j++)
            {
                SpriteRenderer currentGrid = new GameObject("Grid_" + i + "_" + j).AddComponent<SpriteRenderer>();
                
                //TODO: Implement support for different Sprites/Rotation of those dependent on position of the Subsprite
                currentGrid.sprite = gridSprites[0];

                currentGrid.material = gridMaterial;

                currentGrid.transform.parent = gridHolder;
                currentGrid.transform.localPosition = new Vector3(i* perSquareOffset.x, 0.0f, j* perSquareOffset.y);
                currentGrid.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

                gridSquares[i, j] = currentGrid.transform;
            }
        }
    }

    /// <summary>
    /// Shows the grid. If there is no grid it is automatically generated
    /// </summary>
    public void ShowGrid()
    {
        if (gridHolder == null || gridSquares == null) GenerateGrid();

        foreach(Transform t in gridSquares)
        {
            if (t != null) t.GetComponent<Renderer>().enabled = true;
        }

        if (target != null)
        {
            gridHolder.parent = target;
            gridHolder.localPosition = - new Vector3(perSquareOffset.x / 2 * width - perSquareOffset.x / 2, -0.1f, perSquareOffset.y / 2 * length - perSquareOffset.y / 2);
        }
    }

    /// <summary>
    /// Hides the grid.
    /// </summary>
    public void HideGrid()
    {
        if (gridHolder == null || gridSquares == null) return;

        foreach (Transform t in gridSquares)
        {
            if (t != null) t.GetComponent<Renderer>().enabled = false;
        }

        gridHolder.parent = this.transform;
        gridHolder.localPosition = Vector3.zero;
    }

    public void SetGridMaterial(Material mat)
    {
        if(mat == null)
        {
            foreach (Transform t in gridSquares)
            {
                if (t != null) t.GetComponent<Renderer>().material = defaultMat;
            }
        } else
        {
            foreach (Transform t in gridSquares)
            {
                if (t != null) t.GetComponent<Renderer>().material = mat;
            }
        }
    }
}