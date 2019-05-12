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
    private AudioSource m_MusicSource;

    [SerializeField]
    private AudioClip m_ButtonPressClip;

    void Awake(){
        m_EffectSource.playOnAwake = false;
        m_MusicSource.playOnAwake = false;
        m_MusicSource.loop = true;
    }

    public void playBtnEffect(){
        if (!m_EffectSource.isPlaying)
        {
            playEffectClip(m_ButtonPressClip);
        }
    }

    public void playSelectedClip(AudioClip clip, bool force){
        if(force){
            playEffectClip(clip);
        }else{
            if(!m_EffectSource.isPlaying){
                playEffectClip(clip);
            }
        }
    }

    public void playEffectClip(AudioClip clip){
        m_EffectSource.Pause();
        m_EffectSource.clip = clip;
        m_EffectSource.Play();
    }

    public void swapBackground(AudioClip clip){
        m_MusicSource.clip = clip;
        m_MusicSource.Play();
    }
}
