using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGameDataStruct;

namespace LGameDataStruct
{
    public struct CharacterData
    {
        public int coin;
        public float healthPercent;
        public float currentBodyPercent;
    }
}

public class GameManager : MonoBehaviour
{
    private static Object _lock = new Object();
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<GameManager>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                }
            }
            return _instance;
        }
    }

    private CharacterData m_Character;
    public DataManager m_SaveDataManager
    {
        get
        {
            return DataManager.Instance;
        }
    }

    void Awake()
    {
        // var savedata = m_SaveDataManager.
        // 载入存档
        m_Character.coin = 0;
        m_Character.currentBodyPercent = 1f;
        m_Character.healthPercent = 1f;
    }

    private int m_DayCount = 0;

    private float _currenttod;

    private float m_CurrentTimeOfDay
    {
        get
        {
            return _currenttod;
        }
        set
        {
            if (value > 24f)
            {
                _currenttod = value - 24f;
            }
            else if (value < 0f)
            {
                _currenttod = 0f;
            }
            else
            {
                _currenttod = value;
            }
        }
    }

    public void addDayCount()
    {
        ++m_DayCount;
    }

    public int getDayCount()
    {
        return m_DayCount;
    }

    public KeyValuePair<int, int> getTimeOfDayFormat()
    {
        var hour = Mathf.FloorToInt(m_CurrentTimeOfDay);
        var minute = Mathf.FloorToInt((m_CurrentTimeOfDay - hour) * 60f);
        var pair = new KeyValuePair<int, int>(hour, minute);
        return pair;
    }

    public float getTimeOfDayOriginal()
    {
        return m_CurrentTimeOfDay;
    }

    public void setTimeOfDay(float original)
    {
        m_CurrentTimeOfDay = original;
    }

    public void setTimeOfDay(int hour, int minute)
    {
        float value = hour;
        value += (float)minute / 60f;
        m_CurrentTimeOfDay = value;
    }

    Coroutine m_DayLoopCoro;
    public void startDayLoop()
    {
        stopDayLoop();
        m_DayLoopCoro = StartCoroutine(ieDayLoop());
    }

    public void stopDayLoop()
    {
        if (m_DayLoopCoro != null)
        {
            StopCoroutine(m_DayLoopCoro);
            m_DayLoopCoro = null;
        }
    }

    public void adjustTimeFlow(float newFlow)
    {
        m_MinuteOfDays = newFlow;
    }

    private float m_MinuteOfDays = 10f;

    IEnumerator ieDayLoop()
    {

        while (true)
        {
            yield return null;
            float deltaHour = 24f / (m_MinuteOfDays * 60f);
            var result = m_CurrentTimeOfDay + deltaHour * Time.deltaTime;
            setTimeOfDay(result);
        }
    }

    public void startLerpTime(float toValue)
    {
        StartCoroutine(ieDayLerp(toValue));
    }

    public void startLerpTime(int hour, int minute)
    {
        float value = hour;
        value += (float)minute / 60f;
        StartCoroutine(ieDayLerp(value));
    }

    IEnumerator ieDayLerp(float toValue)
    {
        float currentTime = 0f;
        float progress = 0f;

        float lerpTimer = 4f;

        float startTimeOfDays = m_CurrentTimeOfDay;

        while (currentTime < lerpTimer)
        {
            yield return null;
            currentTime += Time.deltaTime;
            progress = currentTime / lerpTimer;

            var current = Mathf.Lerp(m_CurrentTimeOfDay, toValue, progress);
            setTimeOfDay(current);
        }
    }
}

interface ICharacterDataDidChanged{
    void OnCharacterDataChanged(GameManager sender, CharacterData data);
}

interface IGameTimeDidChanged{
    void OnGameTimeChanged(GameManager sender);
}
