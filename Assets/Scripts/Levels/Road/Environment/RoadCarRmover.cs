using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCarRmover : MonoBehaviour
{
    void OnTriggerEnter(Collider info)
    {
        if (info.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            Destroy(info.gameObject);
        }
    }
}
