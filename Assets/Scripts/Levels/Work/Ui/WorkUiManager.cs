using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkUiManager : MonoBehaviour, IStackableUi
{
    [SerializeField]
    private RectTransform m_ClockTextPrefab;
    [SerializeField]
    private RectTransform m_BlurPanelPrefab;
    private UiActionContainer m_ActionContainer;
    private UiClockText m_ClockText;
    private UiBlurPanel m_BlurPanel;

    void Awake()
    {
        m_ActionContainer = GetComponentInChildren<UiActionContainer>();
    }

    public void setupWorkAction()
    {
        m_ActionContainer.cleanAllAction();
        m_ActionContainer.setupAction(null, null, "SPACE", "看时间");
    }

    public void setupLeaveAction(WorkSceneManager sender){
        m_ActionContainer.setupAction(KeyCode.G, sender.leaveWork, "G", "下班");
    }

    public RectTransform getTransform()
    {
        return transform as RectTransform;
    }
    public void onDidPushToStack(bool animate)
    {
        m_BlurPanel = GCanvasController.instance.addToCover(m_BlurPanelPrefab).GetComponent<UiBlurPanel>();
        m_ClockText = GCanvasController.instance.addToCover(m_ClockTextPrefab).GetComponent<UiClockText>();
        // m_ClockText.gameObject.SetActive(false);
        // m_BlurPanel.gameObject.SetActive(false);
        m_BlurPanel.setBlurValue(0f);
        m_ClockText.setTransparent(0f);
    }
    public void onDidBecomeTop()
    {

    }
    public void onWillNotBecomeTop()
    {

    }
    public float onWillRemoveFromStack(bool animate)
    {
        GCanvasController.instance.removeFromCover(m_ClockText.transform as RectTransform);
        m_BlurPanel.setBlurValue(1f);
        GCanvasController.instance.removeFromCover(m_BlurPanel.transform as RectTransform);
        return 0f;
    }

    void Update()
    {
        checkUserInput();
    }

    private float m_ClockTransparent = 1f;
    private float m_TransferSecond = 0.3f;

    private string m_ShaderValue = "_Transparent";

    void checkUserInput()
    {
        if (!Input.GetKey(KeyCode.Space))
        {
            m_ClockTransparent += Time.deltaTime / m_TransferSecond;
        }
        else
        {
            m_ClockTransparent -= Time.deltaTime / m_TransferSecond;
        }

        m_ClockTransparent = Mathf.Clamp01(m_ClockTransparent);
        m_BlurPanel.setBlurValue(m_ClockTransparent);
        m_ClockText.setTransparent(m_ClockTransparent);
    }
}
