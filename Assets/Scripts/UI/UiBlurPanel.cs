using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBlurPanel : MonoBehaviour
{
    private Material m_BlurMaterial;
    private Image m_BlurImage;
    private string m_ShaderBlurValue = "_Transparent";

    void Awake(){
        m_BlurImage = GetComponent<Image>();
        m_BlurMaterial = Instantiate(m_BlurImage.material);
        m_BlurImage.material = m_BlurMaterial;
    }

    public void setTransparent(float val){
        m_BlurMaterial.SetFloat(m_ShaderBlurValue, val);
    }

    public void setTransparent(float val, float time){
        StartCoroutine(ieSetValueCoro(val, time));
    }

    public IEnumerator ieSetValueCoro(float endValue, float time){
        float currentTime = 0f;
        float progress = 0f;
        float startValue = m_BlurMaterial.GetFloat(m_ShaderBlurValue);
        while(currentTime < time){
            yield return null;
            currentTime += Time.deltaTime;
            progress = currentTime / time;

            float currVal = Mathf.SmoothStep(startValue, endValue, progress);
            setTransparent(currVal);
        }
        setTransparent(endValue);
    }
}
