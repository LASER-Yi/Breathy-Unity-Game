using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraAttributeNotNull
{
    public Vector3? position;
    public Quaternion? rotation;
    public float? zlength;
    public float? fov;
}

public struct CameraAttribute
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

public struct CameraDeltaAction
{
    public Vector3? position;
    public Quaternion? rotation;
    public float? zlength;
    public float? fov;
    public CameraAttribute? atoi;

    public static CameraDeltaAction Empty
    {
        get
        {
            return new CameraDeltaAction();
        }
    }
    public void setDeltaPosition(Vector3 val)
    {
        position = val;
    }
    public void setDeltaRotation(Quaternion val)
    {
        rotation = val;

    }
    public void setDeltaZLength(float val)
    {
        zlength = val;

    }
    public void setDeltaFov(float val)
    {
        fov = val;

    }

    public void setAttribute(CameraAttribute attr)
    {
        atoi = attr;

    }
}
