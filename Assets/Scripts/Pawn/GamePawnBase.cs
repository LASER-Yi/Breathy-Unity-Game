using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 玩家和各种AI的接口
public interface IGamePawnBase
{
    // +z -> forward
    void movePawnBy(Vector3 localDirection);
}
