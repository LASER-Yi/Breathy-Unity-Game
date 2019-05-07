using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LCameraSystem
{

    [RequireComponent(typeof(CameraController))]
    public class CameraAnimator : MonoBehaviour
    {
        private static Object _lock = new Object();
        private static CameraAnimator _instance;

        public static CameraAnimator instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var obj = FindObjectOfType<CameraAnimator>();
                        if (obj != null)
                        {
                            _instance = obj;
                        }
                    }
                }
                return _instance;
            }
        }

        private CameraController m_CamController
        {
            get
            {
                return CameraController.instance;
            }
        }

        public delegate float interpolation(float t);

        public delegate void callback();

        private static float vSmoothStep(float t)
        {
            return Mathf.SmoothStep(0f, 1f, t);
        }

        private static float vLerp(float t)
        {
            return t;
        }

        public void stopAllAnimation()
        {
            StopAllCoroutines();
        }

        public void startKeyframeAnimation(CameraAttribute? from, CameraAttribute to, float time)
        {
            if (from != null) m_CamController.setAttribute(from.Value);

            StartCoroutine(ieStartCameraNextKeyframe(to, time, vLerp));
        }

        public void startKeyframeAnimation(CameraAttribute? from, CameraAttribute to, float time, interpolation func)
        {
            if (from != null) m_CamController.setAttribute(from.Value);

            StartCoroutine(ieStartCameraNextKeyframe(to, time, func));
        }

        private UnityEngine.Object intp_lock = new Object();

        public IEnumerator ieStartCameraNextKeyframe(CameraAttribute attr, float time, interpolation func)
        {
            lock (intp_lock)
            {
                float currentTime = 0f;
                float progress = 0f;

                var currentAttr = m_CamController.getAttribute();

                var currPosition = currentAttr.position;
                var currRotation = currentAttr.rotation;
                var currLength = currentAttr.zlength;
                var currFov = currentAttr.fov;

                while (currentTime < time)
                {
                    yield return new WaitForEndOfFrame();
                    float deltaTime = Time.deltaTime;
                    currentTime += deltaTime;
                    progress = currentTime / time;

                    float intp = func(progress);

                    var frame = CameraAttribute.Empty;

                    if (attr.position is Vector3 pos) frame.position = Vector3.Lerp(currPosition, pos, intp);
                    if (attr.rotation is Quaternion rot) frame.rotation = Quaternion.Slerp(currRotation, rot, intp);
                    if (attr.zlength is float length) frame.zlength = Mathf.Lerp(currLength, length, intp);
                    if (attr.fov is float fov) frame.fov = Mathf.Lerp(currFov, fov, intp);

                    m_CamController.setAttribute(frame);
                }
                m_CamController.setAttribute(attr);
                yield return null;
            }
        }

        private Coroutine m_LoopAnimator;

        public void startLoopAnimation(CameraAttribute unscale)
        {
            if (m_LoopAnimator != null)
            {
                StopCoroutine(m_LoopAnimator);
            }
            var coro = StartCoroutine(ieCameraLoopAnimator(unscale));
            m_LoopAnimator = coro;
        }

        private IEnumerator ieCameraLoopAnimator(CameraAttribute unscale)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                var cameraAttr = m_CamController.getAttribute();
                cameraAttr = combineAttribute(cameraAttr, unscale, Time.deltaTime);
                m_CamController.setAttribute(cameraAttr);
            }
        }

        /* Tools */
        private CameraAttributeNotNull combineAttribute(CameraAttributeNotNull attr, CameraAttribute delta, float intp)
        {
            if (delta.position.HasValue) attr.position += delta.position.Value * intp;
            if (delta.rotation.HasValue) attr.rotation *= Quaternion.Slerp(Quaternion.identity, delta.rotation.Value, intp);
            if (delta.zlength.HasValue) attr.zlength += delta.zlength.Value * intp;
            if (delta.fov.HasValue) attr.fov += delta.fov.Value * intp;
            return attr;
        }
    }
}


