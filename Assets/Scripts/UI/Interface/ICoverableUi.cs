using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICoverableUi
{
    RectTransform getTransform();
    void onAddToCanvas(bool animate);

    float onRemoveFromCanvas(bool animate);
}
