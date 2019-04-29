using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// TODO: 方块内随机
// 左右分层
// 线连接
class SelectUiController : MonoBehaviour
{
    private struct SelectInfo
    {
        // public LineRenderer line;
        public Vector3 pointPosition;
        public Button bindBtn;
        public GSceneController.ESceneIndex bindScene;
    }

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
    private float m_ScaleRate = 4f;

    [SerializeField]
    private Button m_BtnPrefab;
    [SerializeField]
    private LineRenderer m_LinePrefab;
    [SerializeField]
    private float m_AxisXOffset = 100f;
    [SerializeField]
    private float m_AxisYScale = 2f;
    [SerializeField]
    private Vector3 m_CenterPoint = Vector3.zero;

    List<SelectInfo> m_SceneInfo;

    private Canvas m_Canvas;

    Vector3 generateWorldPositionInBox()
    {
        var position = Vector3.one;
        position.x *= Random.Range(-1f, 1f);
        position.y *= Random.Range(-1f, 1f);
        position.z *= Random.Range(-1f, 1f);
        return position;
    }

    void initalSceneInfo()
    {
        if (m_BtnPrefab == null) return;

        m_SceneInfo = new List<SelectInfo>();
        var sceneList = GSceneController.instance.getSceneList();
        foreach (var item in sceneList)
        {
            var select = new SelectInfo();
            select.bindScene = item.Key;

            var btn = GCanvasController.instance.putToCanvas(m_BtnPrefab.GetComponent<RectTransform>()).GetComponent<Button>();
            btn.transform.SetParent(transform);
            btn.transform.localPosition = Vector3.zero;
            btn.transform.localScale = Vector3.one;
            select.bindBtn = btn;
            var btnText = btn.GetComponentInChildren<Text>();
            btnText.text = item.Value;
            btn.onClick.AddListener(onBtnClick);

            select.pointPosition = generateWorldPositionInBox();

            // var line = Instantiate(m_LinePrefab);
            // select.line = line;
            // line.transform.SetParent(m_LineContainer.transform, false);

            m_SceneInfo.Add(select);
        }
    }

    void onBtnClick()
    {

    }

    void Awake()
    {
        initalSceneInfo();
        m_Canvas = GetComponentInParent<Canvas>();
    }

    void updateSelectInfo()
    {
        var centerPoint = Vector2.zero;
        foreach (var item in m_SceneInfo)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(item.pointPosition - m_CenterPoint);
            centerPoint.x += screenPoint.x;
            centerPoint.y += screenPoint.y;
        }

        centerPoint /= (float)m_SceneInfo.Count;
        centerPoint = RectTransformUtility.PixelAdjustPoint(centerPoint, transform, m_Canvas);

        foreach (var item in m_SceneInfo)
        {
            var _point = Camera.main.WorldToScreenPoint(item.pointPosition);
            var screenPoint = new Vector2(_point.x, _point.y);
            screenPoint = RectTransformUtility.PixelAdjustPoint(screenPoint, item.bindBtn.transform, m_Canvas);

            var depth = _point.z;

            var offset = screenPoint - centerPoint;
            offset *= m_ScaleRate;
            // 分层
            offset.x += (offset.x < 0 ? -m_AxisXOffset : m_AxisXOffset);
            offset.y *= m_AxisYScale;

            var current = item.bindBtn.GetComponent<RectTransform>();
            current.localPosition = Vector2.Lerp(current.localPosition, offset, 0.1f);

        }
    }

    void Update()
    {
        updateSelectInfo();
    }

    void OnDestroy()
    {
        if (_lineContainer != null) Destroy(_lineContainer);
    }

}