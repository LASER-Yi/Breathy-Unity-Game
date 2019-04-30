using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 玩家和各种AI的接口
public interface IPawnController
{
    // +z -> forward
    // -1 ~ 1
    void updateUserInput(Vector3 input);
    float getVelocity();
    Vector3 getWorldPosition();
}
