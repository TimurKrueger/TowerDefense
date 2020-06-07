using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    [System.Serializable]
    public struct RectangularCameraBounds
    {
        [Header("Note: Y equals the respective Z World Coordinate")]
        [Header("Points that define the rectangular area within which the Camera can be.")]
        public Vector3 bottomLeftPoint;
        public Vector3 topRightPoint;

        /// <summary>
        /// Set the Cuboid Area in World Space within which the Camera can move.
        /// </summary>
        /// <param name="bottomLeftPoint"></param>
        /// <param name="topRightPoint"></param>
        public RectangularCameraBounds(Vector3 bottomLeftPoint, Vector3 topRightPoint)
        {
            this.bottomLeftPoint = bottomLeftPoint;
            this.topRightPoint = topRightPoint;
        }
    }

    public RectangularCameraBounds bounds = new RectangularCameraBounds(new Vector3(-10.0f, 2.5f, -10.0f), new Vector3(10.0f, 10.0f, 10.0f));
    public float speed = 10.0f;
    public float zoomSpeed = 10.0f;
    public bool drawBBOnSelected = true;

    private void OnDrawGizmosSelected()
    {
        /*      
         *        tLA               tRA (=bounds.topRightPoint)
         *         x-----------------x
         *        /|                /|
         *       / |               / |
         *  bLA x--+--------------x <--bRA
         *      |  |              |  |
         *  tLB--> x--------------+--x tRB
         *      | /               | /
         *      |/                |/
         *  bLB x-----------------x bRB
         * (=bounds.bottomLeftPoint)
         */

        if (drawBBOnSelected)
        {
            Vector3 tRA = bounds.topRightPoint;
            Vector3 bLB = bounds.bottomLeftPoint;

            Vector3 bRB = bLB; bRB.x = tRA.x;
            Vector3 tRB = tRA; tRB.y = bLB.y;
            Vector3 tLB = bLB; tLB.z = tRA.z;

            Vector3 tLA = tRA; tLA.x = bLB.x;
            Vector3 bLA = bLB; bLA.y = tRA.y;
            Vector3 bRA = tRA; bRA.z = bLB.z;

            Color col = Color.green;

            Debug.DrawLine(bLB, bRB, col);
            Debug.DrawLine(bRB, tRB, col);
            Debug.DrawLine(tRB, tLB, col);
            Debug.DrawLine(tLB, bLB, col);

            Debug.DrawLine(bLA, bRA, col);
            Debug.DrawLine(bRA, tRA, col);
            Debug.DrawLine(tRA, tLA, col);
            Debug.DrawLine(tLA, bLA, col);

            Debug.DrawLine(bLA, bLB, col);
            Debug.DrawLine(bRA, bRB, col);
            Debug.DrawLine(tRA, tRB, col);
            Debug.DrawLine(tLA, tLB, col);
        }
    }

    /// <summary>
    /// Moves the camera by its own speed into the indexed direction; 0 = Up; 1 = Right; 2 = Down; 3 = Left; 4 = Forwards (Zoom in); 5 = Backwards (Zoom out)
    /// </summary>
    public void Move(int direction)
    {
        switch (direction)
        {
            case 0:
                MoveUp();
                break;
            case 1:
                MoveRight();
                break;
            case 2:
                MoveDown();
                break;
            case 3:
                MoveLeft();
                break;
            case 4:
                MoveForwards();
                break;
            case 5:
                MoveBackwards();
                break;
        }
    }

    void MoveUp()
    {
        float newZ = transform.position.z + (speed * Time.deltaTime);
        if (newZ <= bounds.topRightPoint.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, bounds.topRightPoint.z);
        }
    }

    void MoveRight()
    {
        float newX = transform.position.x + (speed * Time.deltaTime);
        if (newX <= bounds.topRightPoint.x)
        {
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(bounds.topRightPoint.x, transform.position.y, transform.position.z);
        }
    }

    void MoveDown()
    {
        float newZ = transform.position.z - (speed * Time.deltaTime);
        if (newZ >= bounds.bottomLeftPoint.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, bounds.bottomLeftPoint.z);
        }
    }

    void MoveLeft()
    {
        float newX = transform.position.x - (speed * Time.deltaTime);
        if (newX >= bounds.bottomLeftPoint.x)
        {
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(bounds.bottomLeftPoint.x, transform.position.y, transform.position.z);
        }
    }

    void MoveForwards()
    {
        float newY = transform.position.y - (zoomSpeed * Time.deltaTime);
        if (newY >= bounds.bottomLeftPoint.y)
        {
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, bounds.bottomLeftPoint.y, transform.position.z);
        }
    }

    void MoveBackwards()
    {
        float newY = transform.position.y + (zoomSpeed * Time.deltaTime);
        if (newY <= bounds.topRightPoint.y)
        {
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, bounds.topRightPoint.y, transform.position.z);
        }
    }
}
