using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackableUi
{
    RectTransform getTransform();
    void onPushToStack(bool animate);
    // UI 得到当前交互的顶层
    void onBecomeTop();
    // UI 被其他元素覆盖
    void onNotBecomeTop();
    // 返回进行动画的时间
    float onRemoveFromStack(bool animate);
}
