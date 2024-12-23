using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    void Start()
    {
        SetFPSLimit(fpsLimit);
    }

    public enum FPSLimit
    {
        NoLimit = 0,
        Limit30 = 30,
        Limit60 = 60,
        Limit120 = 120,
        Limit240 = 240
    }

    public FPSLimit fpsLimit = FPSLimit.Limit60;
    public void SetFPSLimit(FPSLimit limit)
    {
        Application.targetFrameRate = (int)limit;
        Debug.Log("FPS Limit set to: " + limit);
    }
}
