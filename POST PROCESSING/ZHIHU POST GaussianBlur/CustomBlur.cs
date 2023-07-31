//https://zhuanlan.zhihu.com/p/626618386
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomBlur : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
        private int _rtWidth, _rtHeight;
        private BlurVolume _volume;

        public CustomRenderPass()
        {
            _material = new Material(Shader.Find("Hidden/PostProcessing/Blur/GaussianBlur"));
            _volume = VolumeManager.instance.stack.GetComponent<BlurVolume>();
        }


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_volume == null) return;
            if (!_volume.enable.value) return;

            var cmd = CommandBufferPool.Get("Blur");
            _rtWidth = (int)(Screen.width / _volume.downScale.value);
            _rtHeight = (int)(Screen.height / _volume.downScale.value);
            int tempRT = Shader.PropertyToID("_TempRT_Blur");
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(BufferRT1, _rtWidth, _rtHeight, 0, FilterMode.Bilinear);
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, BufferRT1);
            cmd.Blit(BufferRT1, renderingData.cameraData.renderer.cameraColorTarget, _material);
            context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);
            cmd.ReleaseTemporaryRT(tempRT);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;

    public RenderPassEvent _event = RenderPassEvent.BeforeRenderingPostProcessing;
    // public float downScale;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass
        {
            renderPassEvent = _event
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // m_ScriptablePass.Setup(downScale);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}