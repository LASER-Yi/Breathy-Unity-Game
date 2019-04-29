using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MarketPawn : MonoBehaviour, IGamePawnBaseController
{
    private Rigidbody m_Rigidbody;
    [SerializeField]
    private float m_MoveStep = 1f;
    [SerializeField]
    private float m_MoveFloatHeight = 1f;

    void setupRigidbody(){
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.freezeRotation = true;
    }

    void Awake()
    {
        setupRigidbody();
    }

    public void updateUserInput(Vector3 input)
    {
        var direction = input.z;
        if(direction > 0.5f){
            var newPos = transform.position;
            newPos += Vector3.forward * m_MoveStep;
            newPos.y = m_MoveFloatHeight;
            m_Rigidbody.MovePosition(newPos);
        }
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
