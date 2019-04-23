using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestControl : MonoBehaviour
{
    private Slider m_Slider;
    [SerializeField]
    private Image m_Image;
    private float m_ImageMaxMovement;
    [SerializeField, Range(0, 180)]
    private float m_BPM;
    private bool m_Inverse;
    private float m_DeltaSlide
    {
        get
        {
            return m_BPM / 60.0f;
        }
    }

    private float imageToPercent(){
        float imageCurPos = m_Image.rectTransform.localPosition.y / (m_ImageMaxMovement * 2.0f);
        return Mathf.Clamp((imageCurPos + 0.5f), 0.0f, 1.0f);
    }

    private float percentToImage(float val)
    {
        float value = Mathf.Clamp(val, 0.0f, 1.0f);
        return ((value - 0.5f) * m_ImageMaxMovement * 2.0f);
    }

    void Awake()
    {
        m_Slider = GetComponentInChildren<Slider>();
        m_ImageMaxMovement = 80.0f;
    }
    void Update()
    {
        updateSliderValue();
        updateImagePosition();
        calcBetweenTwo();
    }

    void calcBetweenTwo(){
        float slider = m_Slider.value;
        float image = imageToPercent();
        float affect = Time.deltaTime * 10f;
        if (m_Inverse)
        {
            affect = -affect;
        }
        if(image < slider){
            m_BPM -= affect;
            
        }else{
            m_BPM += affect;
        }
    }

    void updateSliderValue()
    {
        float delatDir = 1.0f;
        if (m_Inverse) delatDir = -1.0f;

        var curValue = m_Slider.value;
        curValue += delatDir * m_DeltaSlide * Time.deltaTime;
        m_Slider.value = curValue;
        if (1 - curValue <= float.Epsilon || curValue <= float.Epsilon)
        {
            m_Inverse = !m_Inverse;
        }
    }

    void updateImagePosition()
    {
        float percent = imageToPercent();
        float movement = 1f * Time.deltaTime;
        if (!Input.GetButton("Jump"))
        {
            movement = -movement;
        }
        percent += movement;
        float pos = percentToImage(percent);
        Vector3 imgPos = m_Image.rectTransform.localPosition;
        imgPos.y = pos;
        m_Image.rectTransform.localPosition = imgPos;
    }
}
