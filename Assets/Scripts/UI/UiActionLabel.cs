using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiActionLabel : MonoBehaviour
{
    [SerializeField]
    private Text m_ButtonText;
    [SerializeField]
    private Text m_ActionNameText;

    public void setActionLabel(string btn, string action){
        m_ButtonText.text = "[ " + btn + " ]";
        m_ActionNameText.text = action;
    }
}
