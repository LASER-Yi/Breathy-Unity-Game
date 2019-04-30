using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLoadPanel : MonoBehaviour, ICoverableUi
{

    public RectTransform getTransform()
    {
        return transform as RectTransform;
    }

    public void onAddToCanvas(bool animate)
    {

    }

    public float onRemoveFromCanvas(bool animate)
    {
        return 0f;
    }

    private float m_Progress
    {
        get
        {
            return GSceneController.instance.getLoadProgress();
        }
    }

    private Image m_LoadImage;

    void Awake()
    {
        m_LoadImage = GetComponentInChildren<Image>();
    }

    public void setLoadImage(Sprite image)
    {
        m_LoadImage.sprite = image;
    }

    void Update()
    {

    }
}