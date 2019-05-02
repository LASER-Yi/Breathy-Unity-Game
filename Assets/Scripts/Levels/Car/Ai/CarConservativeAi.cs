using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarAiAttribute;

public class CarConservativeAi
{

    // 车辆在跟随前车时使用的计时器
    private float m_FollowThreshold = 5f;
    private float m_FollowTimer;

    // 车辆在被停止时使用的计时器
    private float m_StopThreshold = 10f;
    private float m_StopTimer;

    // State Machine
    // Follow -> Override
    // Override -> Follow
    private bool m_OverrideState = true;
    private bool m_FollowState = false;
    private bool m_StopState = false;

    private float m_AiWarnDistance;
    private float m_AiSafeDistance;

    public void updateThreshold(float warn, float safe)
    {
        m_AiWarnDistance = warn;
        m_AiSafeDistance = safe;
    }

    private void moveToStopState()
    {
        m_FollowState = false;
        m_StopState = true;
        m_OverrideState = false;
    }

    private void moveToFollowState()
    {
        m_FollowState = true;
        m_StopState = false;
        m_OverrideState = false;
    }
    private void moveToOverrideState()
    {
        m_FollowState = false;
        m_StopState = false;
        m_OverrideState = true;
    }
    public Strategic compute(in Strategic prev, in Environment env)
    {
        var stra = prev;

        if (m_FollowState)
        {
            if (isRemainFollow(in prev, in env))
            {
                stra = generateFollowAction(in prev, in env);
            }
            else
            {
                moveToOverrideState();
                stra = generateOverrideAction(in prev, in env);
            }

            if (isDetectDanger(in env))
            {
                moveToStopState();
            }
        }
        else if (m_OverrideState)
        {
            if (prev.targetRoadNumber == env.currentRoadNumber)
            {
                // 超车完成
                moveToFollowState();
                m_FollowTimer = 0f;
            }
            else
            {
                // 正在超车
                if (env.frontDistance.HasValue && env.frontDistance.Value < m_AiSafeDistance)
                {
                    moveToFollowState();
                }
                stra = generateOverrideAction(in prev, in env);
            }

            if (isDetectDanger(in env))
            {
                moveToStopState();
            }
        }
        else if (m_StopState)
        {
            if (isDetectDanger(in env))
            {
                stra = generateStopAction(in env);
                m_StopTimer += Time.deltaTime;
            }
            else
            {
                moveToFollowState();
            }
        }

        return stra;
    }

    Strategic generateFollowAction(in Strategic prev, in Environment env)
    {
        var stra = prev;
        stra.brake = false;
        if (env.frontDistance.HasValue)
        {
            var fwd = env.frontDistance.Value;
            var percent = (fwd - m_AiWarnDistance) / (m_AiSafeDistance - m_AiWarnDistance);
            stra.power = Mathf.Lerp(0f, 0.8f, percent);
        }
        else
        {
            stra.power = 0.8f;
        }
        return stra;
    }

    Strategic generateOverrideAction(in Strategic prev, in Environment env)
    {
        var stra = prev;
        stra.brake = false;
        stra.power = 1.0f;
        stra.targetRoadNumber = env.currentRoadNumber;
        // 尝试超车
        if (env.leftDistance.HasValue)
        {
            var leftDist = env.leftDistance.Value;
            var distance = Mathf.Abs(leftDist);
            var threshold = m_AiSafeDistance;

            bool leftCarBehind = leftDist < 0;
            bool fillfulDistance = distance > threshold;
            bool roadAvaliable = isRoadAvaliable(env.currentRoadNumber - 1, env.roadNumberCount);

            if (leftCarBehind && fillfulDistance && roadAvaliable)
            {
                stra.targetRoadNumber = env.currentRoadNumber - 1;
                return stra;
            }
        }
        else
        {
            bool roadAvaliable = isRoadAvaliable(env.currentRoadNumber - 1, env.roadNumberCount);
            if (roadAvaliable)
            {
                stra.targetRoadNumber = env.currentRoadNumber - 1;
                return stra;
            }
        }

        if (env.rightDistance.HasValue)
        {
            var rightDist = env.rightDistance.Value;
            var distance = Mathf.Abs(rightDist);
            var threshold = m_AiWarnDistance + (m_AiSafeDistance - m_AiWarnDistance) / 2f;

            bool rightCarBehind = rightDist < 0;
            bool fillfulDistance = distance > threshold;
            bool roadAvaliable = isRoadAvaliable(env.currentRoadNumber + 1, env.roadNumberCount);

            if (rightCarBehind && fillfulDistance && roadAvaliable)
            {
                stra.targetRoadNumber = env.currentRoadNumber + 1;
                return stra;
            }
        }
        else
        {
            bool roadAvaliable = isRoadAvaliable(env.currentRoadNumber + 1, env.roadNumberCount);
            if (roadAvaliable)
            {
                stra.targetRoadNumber = env.currentRoadNumber + 1;
                return stra;
            }
        }

        return stra;
    }

    Strategic generateStopAction(in Environment env)
    {
        var stra = Strategic.Default;
        stra.brake = true;
        stra.power = 0f;
        stra.targetRoadNumber = env.currentRoadNumber;
        return stra;
    }

    bool isRemainFollow(in Strategic prev, in Environment env)
    {

        if (env.frontDistance.HasValue)
        {
            bool isReachThreshold = env.frontDistance.Value < m_AiSafeDistance;
            bool isRemainSameRoad = prev.targetRoadNumber == env.currentRoadNumber;
            if (isReachThreshold && isRemainSameRoad)
            {
                m_FollowTimer += Time.deltaTime;
            }
            else
            {
                m_FollowTimer = 0f;
            }
        }
        else
        {
            m_FollowTimer = 0f;
        }

        if (m_FollowTimer > m_FollowThreshold)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool isDetectDanger(in Environment env)
    {
        if (env.frontDistance.HasValue)
        {
            var fwd = env.frontDistance.Value;
            if (fwd < m_AiWarnDistance)
            {
                return true;
            }
        }
        return false;
    }

    bool isRoadAvaliable(int roadNumber, int max)
    {
        return roadNumber >= 0 && roadNumber < max;
    }
}
