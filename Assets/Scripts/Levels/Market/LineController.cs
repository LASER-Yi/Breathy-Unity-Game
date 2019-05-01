using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 前后两个队列
// 前方的队列自行移动
// 后方队列根据主角移动
// 前方队列自动填充直到摄像机看不见目标
public class LineController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> m_PersonPrefabs;

    private GameObject m_FrontLine;
    private GameObject m_BackLine;

}
