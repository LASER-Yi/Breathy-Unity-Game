using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

[ExecuteInEditMode]
public class FxRenderer : MonoBehaviour
{
    [SerializeField]
    private Shader m_RenderShader;
    private Material m_Material;
    [SerializeField]
    private CameraEvent m_RenderEvent;
    [SerializeField]
    private string m_CommandName = "FxRenderer";
    [SerializeField, Range(0, 4)]
    private int m_OriginalDownsample;
    [SerializeField, Range(0, 4)]
    private int m_OutputDownsample;
    private Dictionary<Camera, CommandBuffer> m_Buff = new Dictionary<Camera, CommandBuffer>();
    public void cleanup()
    {
        foreach (var item in m_Buff)
        {
            if (item.Key)
            {
                item.Key.RemoveCommandBuffer(m_RenderEvent, item.Value);
            }
        }
        m_Buff.Clear();
        Object.DestroyImmediate(m_Material);
    }

    void Awake()
    {
        cleanup();
    }

    void OnEnable()
    {
        cleanup();
    }

    void OnDisable()
    {
        cleanup();
    }

    void OnWillRenderObject()
    {
        var act = gameObject.activeInHierarchy;
        if (!act)
        {
            cleanup();
            return;
        }

        var cam = Camera.current;
        if (!cam) return;

        CommandBuffer buf = null;
        if (m_Buff.ContainsKey(cam)) return;

        if (!m_Material)
        {
            m_Material = new Material(m_RenderShader);
            m_Material.hideFlags = HideFlags.HideAndDontSave;
        }

        buf = new CommandBuffer();
        buf.name = m_CommandName;
        m_Buff[cam] = buf;

        int screenRt = Shader.PropertyToID("_ScreenRenderTexture");

        int sample = -1 - m_OriginalDownsample;

        buf.GetTemporaryRT(screenRt, sample, sample, 0, FilterMode.Trilinear);

        buf.Blit(BuiltinRenderTextureType.CurrentActive, screenRt);

        int result = renderFxOnRt(ref buf, screenRt);

        buf.SetGlobalTexture("_GrabTexture", result);

        cam.AddCommandBuffer(m_RenderEvent, buf);
    }

    int renderFxOnRt(ref CommandBuffer buf, int screen)
    {
        int result = Shader.PropertyToID("_Temp");
        int downsample = -1 - m_OutputDownsample;
        buf.GetTemporaryRT(result, downsample, downsample, 0, FilterMode.Trilinear);
        buf.Blit(screen, result);
        return result;
    }
}

[CustomEditor(typeof(FxRenderer))]
public class FxRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = target as FxRenderer;
        if (GUILayout.Button("Refresh"))
        {
            script.cleanup();
        }
    }
}
