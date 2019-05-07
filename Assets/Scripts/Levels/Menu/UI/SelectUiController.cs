using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// TODO: 方块内随机
// 左右分层
// 线连接
class SelectUiController : MonoBehaviour, IStackableUi
{
    private struct SelectInfo
    {
        // public LineRenderer line;
        public Vector3 pointPosition;
        public Button bindBtn;
        public GSceneController.ESceneIndex bindScene;
    }

    private MenuSceneManager m_Manager;

    private GameObject _lineContainer;
    private GameObject m_LineContainer
    {
        get
        {
            if (_lineContainer == null)
            {
                var _obj = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                _obj.transform.SetParent(transform);
                _obj.name = "__line__container__";
                _lineContainer = _obj;
            }
            return _lineContainer;
        }
    }


    [SerializeField]
    private Button m_BtnPrefab;
    [SerializeField]
    private LineRenderer m_LinePrefab;
    [SerializeField]
    private float m_AxisXOffset = 100f;
    [SerializeField]
    private float m_BoxSize;
    [SerializeField]
    private Vector3 m_CenterPoint = Vector3.zero;

    List<SelectInfo> m_SceneInfo;

    private Canvas m_Canvas;


    public void setupManager(MenuSceneManager instance)
    {
        m_Manager = instance;
    }

    Vector3 generateWorldPositionInBox()
    {
        var position = Vector3.one;
        position.x *= Random.Range(-1f, 1f);
        position.y *= Random.Range(-1f, 1f);
        position.z *= Random.Range(-1f, 1f);
        return position;
    }

    // void initalSceneInfo()
    // {
    //     if (m_BtnPrefab == null) return;

    //     m_SceneInfo = new List<SelectInfo>();
    //     var sceneList = GSceneController.instance.getSceneList();
    //     foreach (var item in sceneList)
    //     {
    //         var select = new SelectInfo();
    //         select.bindScene = item.Key;

    //         var btn = Instantiate(m_BtnPrefab).GetComponent<Button>();
    //         btn.transform.SetParent(transform);
    //         btn.transform.localPosition = Vector3.zero;
    //         btn.transform.localScale = Vector3.one;
    //         select.bindBtn = btn;
    //         var btnText = btn.GetComponentInChildren<Text>();
    //         btnText.text = item.Value;
    //         btn.onClick.AddListener(delegate { onBtnClick(select.bindScene); });

    //         select.pointPosition = generateWorldPositionInBox();

    //         // var line = Instantiate(m_LinePrefab);
    //         // select.line = line;
    //         // line.transform.SetParent(m_LineContainer.transform, false);

    //         m_SceneInfo.Add(select);
    //     }
    // }

    void onBtnClick(GSceneController.ESceneIndex index)
    {
        m_Manager.loadSubGame(index);
    }

    void Awake()
    {
        m_Canvas = GetComponentInParent<Canvas>();
    }

    void updateSelectInfo()
    {
        var c_point = Camera.main.WorldToScreenPoint(m_CenterPoint);
        var centerPoint = new Vector2(c_point.x, c_point.y);
        centerPoint = RectTransformUtility.PixelAdjustPoint(centerPoint, transform, m_Canvas);

        foreach (var item in m_SceneInfo)
        {
            var _point = Camera.main.WorldToScreenPoint(item.pointPosition);
            var screenPoint = new Vector2(_point.x, _point.y);
            screenPoint = RectTransformUtility.PixelAdjustPoint(screenPoint, item.bindBtn.transform, m_Canvas);

            var depth = _point.z;

            var offset = screenPoint - centerPoint;
            offset = offset.normalized;
            offset *= m_BoxSize;
            // offset = Vector2.Lerp(-m_BoxSize, m_BoxSize, offset.x);
            // 分层
            offset.x += (offset.x < 0 ? -m_AxisXOffset : m_AxisXOffset);

            var current = item.bindBtn.GetComponent<RectTransform>();
            current.localPosition = Vector2.Lerp(current.localPosition, offset, 0.1f);

        }
    }
    [Space, SerializeField]
    private float m_RotateSpeed;

    void setCameraAnimation()
    {

        var animator = LCameraSystem.CameraAnimator.instance;
        animator.stopAllAnimation();

        var keyframe = LCameraSystem.CameraAttribute.Empty;
        keyframe.setZLength(200f);
        keyframe.setRotation(Quaternion.Euler(0f, -40f, 0f));

        var delta = LCameraSystem.CameraAttribute.Empty;
        delta.setRotation(Quaternion.Euler(0f, -m_RotateSpeed, 0f));

        animator.startKeyframeAnimation(null, keyframe, 0.6f);
        animator.startLoopAnimation(delta);
    }

    void Update()
    {
        updateSelectInfo();
    }

    void OnDestroy()
    {
        if (_lineContainer != null) Destroy(_lineContainer);
        LCameraSystem.CameraAnimator.instance.stopAllAnimation();
    }

    public RectTransform getTransform()
    {
        return transform as RectTransform;
    }
    public void onDidPushToStack(bool animate)
    {

    }
    public void onDidBecomeTop()
    {
        setCameraAnimation();
    }
    public void onWillNotBecomeTop()
    {
        LCameraSystem.CameraAnimator.instance.stopAllAnimation();
    }
    public float onWillRemoveFromStack(bool animate)
    {
        return 0f;
    }

}