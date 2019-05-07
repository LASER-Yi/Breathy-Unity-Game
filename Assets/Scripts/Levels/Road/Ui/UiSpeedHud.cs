using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSpeedHud : MonoBehaviour
{
    private RoadUiController m_Controller;
    [SerializeField]
    private Text m_SpeedText;
    [SerializeField]
    private Image m_SpeedImage;
    [SerializeField]
    private Text m_DistanceText;
    private System.Text.StringBuilder m_TextBuilder;

    void Start(){
        m_TextBuilder = new System.Text.StringBuilder();
        m_Controller = GetComponentInParent<RoadUiController>();
    }

    void Update()
    {
        var speed = m_Controller.getPlayerVelocity() * 3.6f;
        var percent = speed / 120f;
        m_SpeedImage.fillAmount = percent;

        m_TextBuilder.Clear();
        m_TextBuilder.Append(Mathf.CeilToInt(speed));
        m_TextBuilder.Append(" KM/H");
        m_SpeedText.text = m_TextBuilder.ToString();

        var distance = m_Controller.getFinilizeDistance();

        m_TextBuilder.Clear();
        m_TextBuilder.Append(Mathf.CeilToInt(distance));
        m_TextBuilder.Append('m');
        m_DistanceText.text = m_TextBuilder.ToString();
    }
}
