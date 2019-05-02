using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarAiAttribute;

public class CarConservativeAi
{
    private float m_ForwardTimer;
    public Strategic computeAi(in Strategic prev, in Environment env, float warnDist, float safeDist)
    {
        var stra = Strategic.Default;
        if (env.frontDistance.HasValue)
        {
            var fwdDistance = env.frontDistance.Value;
            if (fwdDistance < warnDist / 2f)
            {
                // 紧急刹车
                stra.brake = true;
                stra.power = 0f;
            }
            else if (fwdDistance < safeDist)
            {
                // 保持车距
                var percent = (fwdDistance - warnDist) / (safeDist - warnDist);
                stra.power = Mathf.Lerp(0f, 1f, percent);


                if (m_ForwardTimer < 3f)
                {
                    m_ForwardTimer += Time.deltaTime;
                }
                else
                {
                    if (env.leftDistance.HasValue)
                    {
                        var leftDist = env.leftDistance.Value;
                        var distance = Mathf.Abs(leftDist);
                        var threshold = warnDist + (safeDist - warnDist) / 2f;

                        bool leftCarBehind = leftDist < 0;
                        bool fillfulDistance = distance > threshold;
                        bool roadAvaliable = true;
                        if (leftCarBehind && fillfulDistance && roadAvaliable)
                        {

                        }
                    }
                    else if (env.rightDistance.HasValue)
                    {

                    }
                }
            }
        }
        return stra;
    }
}
