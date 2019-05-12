using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSoundController : MonoBehaviour
{
    private static Object _lock = new Object();
    private static GSoundController _instance;

    public static GSoundController instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<GSoundController>();
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
    private AudioSource m_EffectSource;

    [SerializeField]
    private AudioClip m_ButtonPressClip;
}
