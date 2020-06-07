using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D : MonoBehaviour {

    Transform spritePrefab = GameMaster.GetInstance().grid3dPrefab;
    Transform[,/*,*/] gridSprites;
    bool marked = false;

    public void resize(int width, int length, int height)
    {
        if (!marked)
        {
            if (gridSprites != null)
            {
                foreach (Transform t in gridSprites)
                {
                    Destroy(t.gameObject);
                }
            }

#if (false)
            gridSprites = new Transform[width + 5, length + 5/*, height*/];
            Vector3 baseOffset = new Vector3(width / 2 / transform.lossyScale.x * 5, (height / 2 / transform.lossyScale.y)-0.05f, length / 2 / transform.lossyScale.z * 5);
            
            for (int i = 0; i < width + 5; i++)
            {
                for (int j = 0; j < length + 5; j++)
                {
                    //for(int k = 0; k < height; k++)
                    //{
                    gridSprites[i, j/*, k*/] = Instantiate(spritePrefab, this.transform);
                    gridSprites[i, j/*, k*/].localPosition = new Vector3((i - 2) / gridSprites[i, j].lossyScale.x / 5, 0.0f /*k*/, (j - 2) / gridSprites[i, j].lossyScale.z / 5) - baseOffset;
                    gridSprites[i, j/*, k*/].eulerAngles += new Vector3(0.0f, 0.0f, 0.0f);
                    gridSprites[i, j].localScale = new Vector3(gridSprites[i, j].localScale.x / gridSprites[i, j].lossyScale.x * .1f, gridSprites[i, j].localScale.y / gridSprites[i, j].lossyScale.y, gridSprites[i, j].localScale.z / gridSprites[i, j].lossyScale.z * .1f);
                    //}
                }
            }    
#endif
            gridSprites = new Transform[width, length];
            float scale = 5;
            Vector3 baseOffset = new Vector3(2.0f, 0.0f, 2.0f);

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < length; j++)
                {
                    gridSprites[i, j] = Instantiate(spritePrefab, this.transform);
                    gridSprites[i, j].localPosition += new Vector3(i/scale, 0.0f, j/scale) - baseOffset;
                }
            }



        }
    }

    public void Stop()
    {
        marked = true;
        if (gridSprites != null)
        {
            foreach (Transform t in gridSprites)
            {
                Destroy(t.gameObject);
            }
        }
        Destroy(this);
    }
}
