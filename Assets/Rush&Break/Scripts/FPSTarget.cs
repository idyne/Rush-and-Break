using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTarget : MonoBehaviour
{

    public int target = 30;

    void Awake()
    {
        Application.targetFrameRate = target;
    }

}