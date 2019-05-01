using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraAttributeNotNull
{
    public Vector3 position;
    public Quaternion rotation;
    public float zlength;
    public float fov;
}

public class CameraAttribute
{
    public Vector3? position;
    public Quaternion? rotation;
    public float? zlength;
    public float? fov;

    public static CameraAttribute Empty
    {
        get
        {
            return new CameraAttribute();
        }
    }
    public void setPosition(Vector3 val)
    {
        position = val;

    }
    public void setRotation(Quaternion val)
    {
        rotation = val;

    }
    public void setZLength(float val)
    {
        zlength = val;

    }
    public void setFov(float val)
    {
        fov = val;

    }
}
