using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

    Text text;
    float fps = 0.0f;
    [Range(0, 5)]
    public int fpsAccuracy = 1;
    [Range(0, 60)]
    public int medianSteps = 30;
    int medianCounter = 0;
    float fpsMedian = 0.0f;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        float accMod = Mathf.Pow(10, fpsAccuracy);//(10 ^ (5 - fpsAccuracy));
        fps = 1 / Time.deltaTime;//(float)Mathf.RoundToInt((1 / Time.deltaTime)* accMod)/ accMod;
        fpsMedian += fps;

        if (medianCounter >= medianSteps)
        {
            fps = fpsMedian / medianSteps;
            fps = (float)Mathf.RoundToInt(fps * accMod) / accMod;
            text.text = "FPS: " + fps;// + "_"+fpsMedian+"_"+medianSteps+"_"+medianCounter;
            fpsMedian = 0;
            medianCounter = 0;
        } else
        {
            //text.text = medianCounter + "_" + medianSteps;
            medianCounter++;
        }
    }
}
