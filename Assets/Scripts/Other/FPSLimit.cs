using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimit : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;//�������֡��Ϊ60֡
        //Application.targetFrameRate = 120;//�������֡��Ϊ120֡
    }
}
