using System.Collections;
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

    [SerializeField]
    private Light m_SunLight;
    private float _currentHour;
    private float m_CurrentHour
    {
        get
        {
            return _currentHour;
        }
        set
        {
            if (value > 24f)
            {
                _currentHour = value - 24f;
            }else if (value < 0f)
            {
                _currentHour = 0f;
            }else{
                _currentHour = value;
            }
        }
    }

    private Quaternion computeLightRotation(float hour)
    {
        var xValue = Mathf.Lerp(-90f, 270f, hour / 24f);
        var yValue = Mathf.SmoothStep(90f, 120f, (hour % 24f) / 12f);
        var rotator = Quaternion.Euler(xValue, yValue, 0f);
        return rotator;
    }

    public void setTimeOfDays(float hour)
    {
        m_SunLight.transform.rotation = computeLightRotation((float)hour);
        m_CurrentHour = hour;
    }

    public void startLightAnimation(int hour, float time)
    {
        StartCoroutine(ieRotateLight(hour, time));
    }

    public void startDefaultLightLoop()
    {
        StartCoroutine(ieLightLoop(12));
    }

    public void stopAllAnimation()
    {
        StopAllCoroutines();
    }

    private UnityEngine.Object animator_lock = new Object();

    private IEnumerator ieRotateLight(int hour, float time)
    {
        lock (animator_lock)
        {
            float currentTime = 0f;
            float progress = 0f;

            var startRotation = m_SunLight.transform.rotation;
            var targetRotation = computeLightRotation((float)hour);

            while (currentTime < time)
            {
                yield return null;
                float deltaTime = Time.deltaTime;
                currentTime += deltaTime;
                progress = currentTime / time;

                var current = Quaternion.Slerp(startRotation, targetRotation, progress);

                m_SunLight.transform.rotation = current;
            }
            m_CurrentHour = hour;
        }
    }

    private IEnumerator ieLightLoop(int minuteOfDays)
    {
        float deltaHour = 24f / (minuteOfDays * 60f);

        while (true)
        {
            yield return null;
            float deltaTime = Time.deltaTime;
            m_CurrentHour += deltaHour * deltaTime;
            setTimeOfDays(m_CurrentHour);
        }
    }
}
