﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLightController : MonoBehaviour
{
    private static Object _lock = new Object();
    private static GLightController _instance;

    public static GLightController instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<GLightController>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                }
            }
            return _instance;
        }
    }

    private VolumetricFogAndMist.VolumetricFog m_VolumetricFog;

    [SerializeField]
    private Light m_SunLight;

    void Awake(){
        m_VolumetricFog = Camera.main.GetComponent<VolumetricFogAndMist.VolumetricFog>();
    }

    private Quaternion computeLightRotation(float hour)
    {
        var xValue = Mathf.Lerp(-90f, 270f, hour / 24f);
        var yValue = Mathf.SmoothStep(0, -70f, (hour % 24f) / 12f);
        var rotator = Quaternion.Euler(xValue, yValue, 0f);
        return rotator;
    }

    private void setLightRotation(float hour)
    {
        m_SunLight.transform.rotation = computeLightRotation((float)hour);
    }

    void Update(){
        var currentHour = GameManager.instance.getTimeOfDayOriginal();
        setLightRotation(currentHour);
    }
}
