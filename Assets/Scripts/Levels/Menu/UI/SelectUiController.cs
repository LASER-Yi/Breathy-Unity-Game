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
        public SceneController.ESceneIndex bindScene;
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

    List<SelectInfo> m_SceneInfo;

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
        var sceneList = SceneController.instance.getSceneList();
        foreach (var item in sceneList)
        {
            var select = new SelectInfo();
            select.bindScene = item.Key;

            var btn = UiCanvasController.instance.directOverlay(m_BtnPrefab.GetComponent<RectTransform>()).GetComponent<Button>();
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
    }

    void updateSelectInfo()
    {
        var centerPoint = Vector2.zero;
        foreach (var item in m_SceneInfo)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(item.pointPosition);
            centerPoint.x += screenPoint.x;
            centerPoint.y += screenPoint.y;
        }

        centerPoint /= (float)m_SceneInfo.Count;

        foreach (var item in m_SceneInfo)
        {
            var _point = Camera.main.WorldToScreenPoint(item.pointPosition);
            var screenPoint = new Vector2(_point.x, _point.y);
            var depth = _point.z;

            var offset = screenPoint - centerPoint;
            offset *= m_ScaleRate;
            // 分层

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