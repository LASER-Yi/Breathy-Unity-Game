using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MarketPawn : MonoBehaviour, IGamePawnBaseController
{
    private Rigidbody m_Rigidbody;

    private static float m_MoveStep;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void updateUserInput(Vector3 input)
    {
        var direction = Vector3.zero;
    }

    public float getVelocity()
    {
        return 0f;
    }

    public Vector3 getWorldPosition()
    {
        return transform.position;
    }
}
