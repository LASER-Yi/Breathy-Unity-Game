using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMesh : MonoBehaviour
{
    private RoadChunk m_RoadInfo = null;
    void Awake(){
        m_RoadInfo = GetComponentInParent<RoadChunk>();
    }
    void OnBecameVisible(){
        m_RoadInfo.OnRoadVisible();
    }

    void OnBecameInvisible(){
        m_RoadInfo.OnRoadInvisible();
    }
}
