//https://zhuanlan.zhihu.com/p/642579757
//和Volume配合 CustomVolume

/// ForwardRenderer继承于ScriptableRenderer，它维护了一个ScriptableRenderPass的列表，在每帧前王列表里新增pass，
/// 然后执行pass渲染画面，每帧结束再清空列表。ScriptableRenderer里的核心函数Setup和Execute每帧都会执行，
/// 其中Setup会把要执行的pass加入列表，Execute将列表里的pass按渲染顺序分类提取并执行。
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeatureVolume : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material material;
        public float a = 0.1f;
    }
    public Settings settings = new Settings();
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material material;
        public float a;
        RenderTexture RT;
        RenderTargetIdentifier cameraRT;
        public CustomVolumeComponent volumeComponent;
        
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // RenderTextureDescriptor是一个结构体 里是一些描述RT的属性 新建RT需要一个RenderTextureDescriptor类的参数
            // public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat, int depthBufferBits);
            RenderTextureDescriptor RD = new RenderTextureDescriptor(Camera.main.pixelWidth,Camera.main.pixelHeight,RenderTextureFormat.Default,0);
            // 新建RT
            RT = new RenderTexture(RD);
            //获取相机RT
            cameraRT = renderingData.cameraData.renderer.cameraColorTarget; 

            var stack = VolumeManager.instance.stack;//传入volume数据
            volumeComponent = stack.GetComponent<CustomVolumeComponent>();
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        // Execute 写渲染逻辑 实现渲染逻辑 发出绘图指令 执行命令缓冲区
        // RenderingData类中包含着渲染所需要的一些信息，代码里我们就是用它来获取主相机的RT的
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("ZHIHU");
            cmd.name = "ZHIHU POST";
            material.SetColor("_Color",volumeComponent.cp.value);// _A shader里的变量
            cmd.Blit(cameraRT, RT, material);//对相机里的画面进行一些操作
            cmd.Blit(RT, cameraRT);//将结果写回相机
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        // Cleanup any allocated resources that were created during the execution of this render pass.
        //清除渲染过程中创建的所有已分配资源
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;
    /// <inheritdoc/>
    /// Create 创建自己写的pass类
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();
        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
        m_ScriptablePass.material = settings.material;
        m_ScriptablePass.a = settings.a;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    // 把renderPass 放到管线中
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(settings.material == null)
        {
            Debug.LogError("材质球丢失");//本句可隐藏
            return;
        }
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


