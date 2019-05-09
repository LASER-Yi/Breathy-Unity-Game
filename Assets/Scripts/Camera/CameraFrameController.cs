using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;


// TODO: 优化行进轨迹
public class CameraFrameController : MonoBehaviour
{
    // 相机行为：
    // 使用摄像机窗格
    // 玩家越过中心线摄像机进行加速
    // 玩家到达上端线时相机会和玩家具有相同的速度

    // 摄像机窗格设置
    [SerializeField, Range(0f,1f)]
    private float m_TopFramePercent = 0.8f;
    [SerializeField, Range(0f, 1f)]
    private float m_MidFramePercent = 0.4f;
    [SerializeField, Range(0f, 1f)]
    private float m_ButtonFramePercent = 0.2f;

    [SerializeField, Range(0f, 1f)]
    private float m_CenterOffsetPercent = 0.5f;
    [SerializeField, Range(0f, 3f)]
    private float m_CameraCatchupDelay = 1f;
    [SerializeField, Range(0f, 100f)]
    private float m_CameraMaxFollowSpeed = 10f;
    private CameraController m_Controller{
        get{
            return CameraController.instance;
        }
    }
    private Vector3 m_ControllerPosition{
        get{
            return m_Controller.getWorldPosition();
        }
    }
    private IPawnController _chararcter;
    private IPawnController m_Character{
        get{
            if(_chararcter == null){
                _chararcter = GameObject.FindGameObjectWithTag("Player").GetComponent<IPawnController>();
            }
            return _chararcter;
        }
    }
    private float m_RefDeltaPercent = 0.0f;
    private float m_CatchupTimer = 0.0f;

    private Vector2 m_CharacterOnScreen{
        get{
            return m_Controller.getAttachCamera().WorldToScreenPoint(m_Character.getWorldPosition());
        }
    }

    // Botton -> Top : 0 -> 1
    private float m_CharacterYAxisPercent{
        get{
            float height = Screen.height;
            return m_CharacterOnScreen.y / height;
        }
    }

    private bool m_IsEnableFollow = false;

    public void enableFollow(){
        m_IsEnableFollow = true;
    }

    public void disableFollow(){
        m_IsEnableFollow = false;
    }

    private Vector3 m_TargetCameraPosition;

    // catchupdelay -> 玩家越过中线开始摄像机从下窗格追上并锁定玩家的时间
    // 最后使用smoothdamp进行位置的插值
    float computeCounterPosition(){
        var velocity = m_Character.getVelocity();
        float targetPercent;

        if(m_CharacterYAxisPercent < m_ButtonFramePercent || m_CharacterYAxisPercent > m_TopFramePercent){
            m_CatchupTimer = m_CameraCatchupDelay;
        }

        if (velocity > 1e-2)
        {
            if (m_CharacterYAxisPercent > m_MidFramePercent)
            {
                // 玩家越过窗格
                // 根据角色位置进行窗格中间到顶部的插值
                float playerRate = (float)velocity / (m_CameraMaxFollowSpeed * Time.deltaTime);
                targetPercent = Mathf.Lerp(m_MidFramePercent, m_TopFramePercent, playerRate);
                m_CatchupTimer += Time.deltaTime;
            }else{
                targetPercent = m_CharacterYAxisPercent;
            }
        }else{
            m_CatchupTimer -= Time.deltaTime;
            targetPercent = m_ButtonFramePercent;
        }

        m_CatchupTimer = Mathf.Clamp(m_CatchupTimer, 0f, m_CameraCatchupDelay);
        return Mathf.SmoothDamp(m_CharacterYAxisPercent, targetPercent, ref m_RefDeltaPercent, m_CameraCatchupDelay);
    }

    void computeTargetPosition(float playerPercent){
        var velocity = m_Character.getVelocity();
        var camera = m_Controller.getAttachCamera();
        var depth = m_Controller.getZLength();

        Vector3 charPoint = Vector3.zero;
        charPoint.x = Mathf.CeilToInt(m_CenterOffsetPercent * Screen.width);
        charPoint.y = Mathf.CeilToInt(playerPercent * Screen.height);
        charPoint.z = depth;

        Vector3 worldPoint = Vector3.zero;
        worldPoint.x = Mathf.CeilToInt(Screen.width / 2);
        worldPoint.y = Mathf.CeilToInt(Screen.height / 2);
        worldPoint.z = depth;

        Vector3 virtualScreenChar = camera.ScreenToWorldPoint(charPoint);
        Vector3 virtualCameraPoint = camera.ScreenToWorldPoint(worldPoint);

        var offset = virtualCameraPoint - virtualScreenChar;

        Vector3 original = m_Character.getWorldPosition();
        m_TargetCameraPosition = (original + offset);
    }

    void setCameraPosition(){
        m_Controller.setPosition(m_TargetCameraPosition);
    }

    void setupCamera(){
        CameraAttribute attr = CameraAttribute.Empty;
        attr.setPosition(m_Character.getWorldPosition());
        attr.setRotation(Quaternion.Euler(90f, 0f, 0f));
        attr.setFov(60f);
        attr.setZLength(30f);
        m_Controller.setAttribute(attr);
    }

    void Start(){
        setupCamera();
    }

    void LateUpdate(){
        if(m_IsEnableFollow){
            float rate = computeCounterPosition();
            computeTargetPosition(m_ButtonFramePercent);
            setCameraPosition();
        }
    }
}
