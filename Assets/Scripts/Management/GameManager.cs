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

    private int m_CurrentDayCount = 0;
    private int m_CurrentHour
    {
        get
        {
            return Mathf.FloorToInt(m_CurrentTimeOfDay);
        }
    }
    private int m_CurrentMinute
    {
        get
        {
            return Mathf.FloorToInt((m_CurrentTimeOfDay - m_CurrentHour) * 60f);
        }
    }

    private float m_CurrentTimeOfDay;


    public void setTimeOfDay(float original)
    {
        var willValue = original;
        if (willValue > 24) willValue -= 24f;
        m_CurrentTimeOfDay = willValue;
        fireTimeChangedEvent();
    }

    public void setTimeOfDay(int hour, int minute)
    {
        if (hour > 24)
        {
            hour -= 24;
        }
        float time = hour + minute / 60f;

        setTimeOfDay(time);
    }

    public void increaseDayCount()
    {
        ++m_CurrentDayCount;
    }

    public int getDayCount()
    {
        return m_CurrentDayCount;
    }

    public struct GameTime
    {
        public int hour;
        public int minute;
    }

    private delegate void OnGameTimeChangedHandler(GameManager sender, GameTime time);

    private event OnGameTimeChangedHandler eventTimeChanged;

    public void addEventListener(ITimeDidChangedHandler listener)
    {
        eventTimeChanged += listener.OnGameTimeChanged;
    }

    public void removeEventListener(ITimeDidChangedHandler listener)
    {
        eventTimeChanged -= listener.OnGameTimeChanged;
    }

    private void fireTimeChangedEvent()
    {
        try
        {
            eventTimeChanged?.Invoke(this, new GameTime()
            {
                hour = m_CurrentHour,
                minute = m_CurrentMinute
            });
        }
        catch (System.NullReferenceException e)
        {
            Debug.Log(e.ToString());
        }
    }

    Coroutine m_DayLoopCoro;
    public void startTimeLoop()
    {
        stopTimeLoop();
        m_DayLoopCoro = StartCoroutine(ieDayLoop());
    }

    public void stopTimeLoop()
    {
        if (m_DayLoopCoro != null)
        {
            StopCoroutine(m_DayLoopCoro);
            m_DayLoopCoro = null;
        }
    }

    public void setTimeSpeed(float newFlow)
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

public interface ICharacterDataDidChangedHandler
{
    void OnCharacterDataChanged(GameManager sender, CharacterData data);
}

public interface ITimeDidChangedHandler
{
    void OnGameTimeChanged(GameManager sender, GameManager.GameTime time);
}
