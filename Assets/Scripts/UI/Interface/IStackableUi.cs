using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackableUi
{
    RectTransform getTransform();
    void onDidPushToStack(bool animate);
    // UI 得到当前交互的顶层
    void onDidBecomeTop();
    // UI 被其他元素覆盖
    void onWillNotBecomeTop();
    // 返回进行动画的时间
    float onWillRemoveFromStack(bool animate);
}
