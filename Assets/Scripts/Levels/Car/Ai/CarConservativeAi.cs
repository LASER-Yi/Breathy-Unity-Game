using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarAiAttribute;

// 三种状态 -> 直行、变道、刹车
public class CarConservativeAi
{

    enum EAiState
    {
        Follow,
        Change,
        Stop,
    }

    enum EDirection
    {
        front,
        back,
        left,
        right,
    }

    // 车辆在跟随前车时使用的计时器
    private float m_FollowThreshold = 10f;
    private float m_FollowTimer;

    // State Machine
    // Follow -> Override
    // Override -> Follow
    private EAiState m_State = EAiState.Follow;

    private bool m_IsComputedRoad = false;

    private float m_AiWarnDistance;
    private float m_AiSafeDistance;

    private EDirection m_LastOverrideDirection = EDirection.right;

    private Strategic m_PrevStrategic = Strategic.Default;
    private Environment m_PrevEnvironment;
    private int m_TotalRoadNum = 0;

    public void updateThreshold(float warn, float safe)
    {
        m_AiWarnDistance = warn;
        m_AiSafeDistance = safe;
    }

    public void updateRoadState(int totalRoad)
    {
        m_TotalRoadNum = totalRoad;
    }

    public Strategic tick(in Environment env)
    {
        var stra = m_PrevStrategic;

        switch (m_State)
        {
            case EAiState.Follow:
                {
                    stra.brake = false;
                    stra.power = computeFollowOutput(in env);
                    stra.targetRoadNumber = env.roadNumber;
                    m_IsComputedRoad = false;

                    var relativeSpeed = Mathf.Abs(computeRelativeSpeed(in env, EDirection.front));

                    if (stra.power < 0.8f && relativeSpeed < 1f)
                    {
                        m_FollowTimer += Time.deltaTime;
                    }
                    else
                    {
                        m_FollowTimer = 0f;
                    }

                    if (m_FollowTimer > m_FollowThreshold)
                    {
                        m_State = EAiState.Change;
                    }
                    break;
                }

            case EAiState.Change:
                {
                    if (!m_IsComputedRoad)
                    {
                        stra.targetRoadNumber = computeNewRoadNumber(in env);
                        m_IsComputedRoad = true;
                    }

                    if (stra.targetRoadNumber == env.roadNumber)
                    {
                        m_State = EAiState.Follow;
                        m_FollowTimer = 0f;
                    }
                    break;
                }

            case EAiState.Stop:
                {
                    stra.brake = true;
                    stra.targetRoadNumber = env.roadNumber;
                    stra.power = -1f;

                    if (env.frontDistance.HasValue && env.frontDistance.Value > m_AiWarnDistance)
                    {
                        m_State = EAiState.Follow;
                    }
                    else if (!env.frontDistance.HasValue)
                    {
                        m_State = EAiState.Follow;
                    }
                    break;
                }
        }

        m_PrevStrategic = stra;
        m_PrevEnvironment = env;
        return stra;
    }

    float computeRelativeSpeed(in Environment curr, EDirection dir)
    {
        var relativeSpeed = 100f;

        switch (dir)
        {
            case EDirection.front:
                {
                    if (curr.frontDistance.HasValue)
                    {
                        if (m_PrevEnvironment.frontDistance.HasValue)
                        {
                            relativeSpeed = (curr.frontDistance.Value - m_PrevEnvironment.frontDistance.Value);
                        }
                        else
                        {
                            relativeSpeed = (curr.frontDistance.Value - m_AiSafeDistance);
                        }
                    }
                }
                break;
            case EDirection.back:
                {
                    {
                        if (curr.backDistance.HasValue)
                        {
                            if (m_PrevEnvironment.backDistance.HasValue)
                            {
                                relativeSpeed = (curr.backDistance.Value - m_PrevEnvironment.backDistance.Value);
                            }
                            else
                            {
                                relativeSpeed = (curr.backDistance.Value - m_AiSafeDistance);
                            }
                        }
                        break;
                    }
                }
        }

        var deltaTime = curr.timestamp - m_PrevEnvironment.timestamp;

        relativeSpeed /= deltaTime;

        return relativeSpeed;
    }

    // 目标：保持现有前后距离
    //      让物体尽量接近目标位置
    float computeFollowOutput(in Environment env)
    {
        var output = m_PrevStrategic.power;

        if (output < 0f && env.speed == 0)
        {
            output = 0f;
        }

        if (env.frontDistance.HasValue)
        {
            var deltaOutput = 0f;

            var relativeSpeed = computeRelativeSpeed(in env, EDirection.front);
            var relativePercent = Mathf.Abs(relativeSpeed / (env.speed != 0 ? env.speed : 10f));

            if (relativeSpeed != 0)
            {
                // 稳定速度
                if (relativeSpeed > 0)
                {
                    deltaOutput += Mathf.Lerp(0f, 4f, relativePercent);
                }
                else
                {
                    deltaOutput -= Mathf.Lerp(0f, 4f, relativePercent);
                }
            }

            // 调整与前车间距
            var fwd = env.frontDistance.Value;
            var current = (fwd - m_AiWarnDistance) / (m_AiSafeDistance - m_AiWarnDistance);
            current = Mathf.Clamp01(current);
            // 计算目标位置
            var target = Mathf.Lerp(0.1f, 0.9f, env.speed / 10f);

            var offsetPercent = Mathf.Abs(current - target);

            // 目标远于预期
            if (target < current)
            {
                // 目标正在远离
                if (relativeSpeed > 0)
                {
                    deltaOutput += Mathf.Lerp(0f, 0.1f, relativePercent);
                }
                deltaOutput += Mathf.Lerp(0f, 0.1f, offsetPercent);
            }
            // 目标近于预期
            else if (target > current)
            {
                // 目标正在接近
                if (relativeSpeed < 0)
                {
                    deltaOutput -= Mathf.Lerp(0f, 0.1f, relativePercent);

                }
                deltaOutput -= Mathf.Lerp(0f, 0.1f, offsetPercent);
            }

            output += deltaOutput * Time.deltaTime;
        }
        else
        {
            // 巡航速度
            output += Time.deltaTime;
        }

        output = Mathf.Clamp(output, -1f, 0.8f);
        return output;
    }

    bool checkLeftRoad(in Environment env)
    {
        if (env.leftDistance.HasValue)
        {
            var leftDist = env.leftDistance.Value;
            var distance = Mathf.Abs(leftDist);
            var threshold = m_AiWarnDistance;

            bool fillfulDistance = distance > threshold;
            bool roadAvaliable = isRoadAvaliable(env.roadNumber - 1, m_TotalRoadNum);

            if (fillfulDistance && roadAvaliable)
            {
                return true;
            }
        }
        else
        {
            bool roadAvaliable = isRoadAvaliable(env.roadNumber - 1, m_TotalRoadNum);
            if (roadAvaliable)
            {
                return true;
            }
        }
        return false;
    }

    bool checkRightRoad(in Environment env)
    {
        if (env.rightDistance.HasValue)
        {
            var rightDist = env.rightDistance.Value;
            var distance = Mathf.Abs(rightDist);
            var threshold = m_AiWarnDistance;

            bool fillfulDistance = distance > threshold;
            bool roadAvaliable = isRoadAvaliable(env.roadNumber + 1, m_TotalRoadNum);

            if (fillfulDistance && roadAvaliable)
            {
                return true;
            }
        }
        else
        {
            bool roadAvaliable = isRoadAvaliable(env.roadNumber + 1, m_TotalRoadNum);
            if (roadAvaliable)
            {
                return true;
            }
        }
        return false;
    }

    int computeNewRoadNumber(in Environment env)
    {
        var number = env.roadNumber;
        // 尝试变道
        if (m_LastOverrideDirection == EDirection.right)
        {
            if (checkLeftRoad(in env))
            {
                return number - 1;
            }
            m_LastOverrideDirection = EDirection.left;
            if (checkRightRoad(in env))
            {
                return number + 1;
            }
        }
        else if (m_LastOverrideDirection == EDirection.left)
        {
            if (checkRightRoad(in env))
            {
                return number + 1;
            }
            m_LastOverrideDirection = EDirection.right;
            if (checkLeftRoad(in env))
            {
                return number - 1;
            }
        }

        return number;
    }

    bool isRemainFollow(in Strategic prev, in Environment env)
    {
        if (env.frontDistance.HasValue)
        {
            bool isReachThreshold = env.frontDistance.Value < m_AiSafeDistance;
            bool isRemainSameRoad = prev.targetRoadNumber == env.roadNumber;
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

    bool isDanger(in Environment env)
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
