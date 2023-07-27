using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExcludeFromPreset]//从预设中排除
[Tooltip("Render Postproseccing Tonemapping-ASCE")]//界面注释
public class TonemappingACESRenderPassFeature : ScriptableRendererFeature
{
    
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Material material;
        public float postExposure = 0.6f; 
        [Range(0.0f,3.0f)]
        public float slope = 2.51f;
        [Range(0.0f,1.0f)]
        public float toe = 0.03f;
        [Range(0.0f,3.0f)]
        public float shoulder = 2.43f;
        [Range(0.0f,1.0f)]
        public float blackClip = 0.59f;
        [Range(0.0f,1.0f)]
        public float whiteClip = 0.14f;
    }
    public Settings settings = new Settings();

    //继承了RenderPass类
    public class TonemappingACESPass : ScriptableRenderPass //渲染的一个基础单元
    {
        //OnCameraSetup()初始化资源
        //Execute()写渲染逻辑
        //OnCameraCleanup()释放所申请的资源
        public TonemappingACESRenderPassFeature.Settings settings;
        public Material material;
        RenderTargetIdentifier source;
        RenderTargetIdentifier destination;
        int temporaryRTId = Shader.PropertyToID("_TempRT");
        static readonly string RenderTag = "Post Effects"; 
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(temporaryRTId, descriptor);
            destination = new RenderTargetIdentifier(temporaryRTId);
        }    
                                           
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();//Get("Tonemapping")
            cmd.name = "MILI POOST Tonemapping ACSE";
            //settings.material.SetFloat("_postExposure", settings.postExposure);//
            material.SetFloat("_postExposure", settings.postExposure);
            material.SetFloat("_FilmSlope", settings.slope);                  
            material.SetFloat("_FilmToe", settings.toe);                           
            material.SetFloat("_FilmShoulder", settings.shoulder);                
            material.SetFloat("_FilmBlackClip", settings.blackClip);               
            material.SetFloat("_FilmWhiteClip", settings.whiteClip);
            Blit(cmd, source, destination, settings.material, 0);
            Blit(cmd, destination, source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd); 
        }
    }
    TonemappingACESPass acesPass; 

    //用来创建刚写好的Renderpass类 即TonemappingACESPass类
    public override void Create()
    {
        this.name = "ACES FEATURE";
        acesPass = new TonemappingACESPass(); 
        acesPass.settings = settings;
        acesPass.material = settings.material; //如果把这行删除 Executel里的 material前要加上settings.
        acesPass.renderPassEvent = settings.renderPassEvent;
        
    }

    //将写好的pass添加到渲染管线中
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
        {
            //Debug.LogError("Material NUll"); //本句可隐藏
            return;
        }
        renderer.EnqueuePass(acesPass);
    }
}