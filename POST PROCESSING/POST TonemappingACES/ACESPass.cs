using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ACESPass : ScriptableRenderPass //渲染的一个基础单元
{
    //OnCameraSetup()初始化资源
    //Execute()写渲染逻辑
    //OnCameraCleanup()释放所申请的资源
    public ACESRenderFeatureVolume.Settings settings;
    public Material material = null;
    RenderTargetIdentifier source;
    RenderTargetIdentifier destination;
    public ACESVolume acesVolume;
    int temporaryRTId = Shader.PropertyToID("_TempRT");
    static readonly string RenderTag = "Post Effects"; 
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        source = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        cmd.GetTemporaryRT(temporaryRTId, descriptor);
        destination = new RenderTargetIdentifier(temporaryRTId);

        var stack = VolumeManager.instance.stack;
        acesVolume = stack.GetComponent<ACESVolume>();
    }    
                                        
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();//Get("Tonemapping")
        cmd.name = "MILI POOST Tonemapping ACSE";
        material.SetFloat("_postExposure", ((float)acesVolume.PostExposure));            
        material.SetFloat("_FilmSlope", ((float)acesVolume.slope));                  
        material.SetFloat("_FilmToe", ((float)acesVolume.toe));                           
        material.SetFloat("_FilmShoulder", ((float)acesVolume.shoulder));                
        material.SetFloat("_FilmBlackClip", ((float)acesVolume.blackClip));               
        material.SetFloat("_FilmWhiteClip", ((float)acesVolume.whiteClip));
        Blit(cmd, source, destination, material, 0);
        Blit(cmd, destination, source);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd); 
    }
}
