using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LGameplay;

public class UiFullScreenText : MonoBehaviour, ICoverableUi
{
    [SerializeField]
    private Text m_MainText;
    [SerializeField]
    private Image m_Background;
    [SerializeField]
    private Text m_StepText;

    private SequenceTextParams m_CurrentParams;

    void Awake(){
        m_MainText = GetComponentInChildren<Text>();
        m_Background = GetComponentInChildren<Image>();
    }

    public void updateScreenInfo(SequenceTextParams param){
        m_MainText.text = param.mainText;
        if(param.background is Color col){
            setupBackgroundColor(col);
        }
        if(param.stepText is string step){
            m_StepText.text = step;
        }else{
            if(param.nextScreen != null){
                m_StepText.text = "按任意键继续";
            }else{
                m_StepText.text = "按任意键结束";
            }
        }
        m_CurrentParams = param;
    }

    void Update(){
        if(Input.anyKeyDown){
            m_CurrentParams?.nextAction?.Invoke();
            if(m_CurrentParams?.nextScreen is SequenceTextParams param){
                updateScreenInfo(param);
            }else{
                GCanvasController.instance.removeFromCover(getTransform());
            }
        }
    }

    public void setupBackgroundColor(Color col){
        m_Background.color = col;
    }

    public RectTransform getTransform(){
        return transform as RectTransform;
    }
    public void onAddToCanvas(bool animate){

    }
    public float onRemoveFromCanvas(bool animate){
        return 0f;
    }
}

namespace LGameplay{
    public class SequenceTextParams{
        public string mainText;
        public Color? background;
        public string stepText;
        public SequenceTextParams nextScreen;
        public UnityAction nextAction;

        public SequenceTextParams(string main, Color? bg = null, string step = null, UnityAction action = null)
        {
            mainText = main;
            background = bg;
            stepText = step;
            nextAction = action;
        }

        public SequenceTextParams concat(string main, Color? bg = null, string step = null, UnityAction action = null){
            nextScreen = new SequenceTextParams(main, bg, step, action);
            return nextScreen;
        }

        public static SequenceTextParams computeParamsFromString(string text){
            var strList = text.Split('\n');
            var param = new SequenceTextParams(strList[0]);
            var start = param;
            for (int i = 1; i < strList.Length; ++i){
                param = param.concat(strList[i]);
            }
            return start;
        }
    }
}
