using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ACESRenderFeatureVolume : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEventFeature = RenderPassEvent.BeforeRenderingPostProcessing;
        public Material material;
    }
    public Settings settings = new Settings();
    ACESPass m_ACESSPass;

    public override void Create()
    {
        this.name = "ACES VOLUME";
        m_ACESSPass = new ACESPass();
        m_ACESSPass.material = settings.material;
        //ACESPass中继承的类自带renderPassEvent
        m_ACESSPass.renderPassEvent = settings.renderPassEventFeature;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(settings.material == null)
        {
            Debug.LogError("测试材质球丢失");
            return;
        }
        renderer.EnqueuePass(m_ACESSPass);
    }
}


