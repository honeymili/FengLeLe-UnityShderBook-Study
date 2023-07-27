# Postproseccing: 后处理
知乎链接：https://zhuanlan.zhihu.com/p/642579757

01.CustomRenderPassFeatureFloat.cs + ShaderFloat.shader + M_ShaderFloat.mat
渲染器Renderer中添加CustomRenderPassFeatureFloat  在其Settings下添加 材质球 M_ShaderFloat  可在Settings下调节参数

02.CustomRenderPassFeatureVolume.cs + CustomVolumeComponent.cs + ShaderColor Volume.shader + M_ShaderColor Volume.mat
渲染器中添加CustomRenderPassFeatureVolume 
层级面板中创建Volume 在其下边添加已创建好的 CustomVolumeComponent 即可在面板中调节参数

注意报错：
01.NullReferenceException 译为:空引用异常
RendererFeature.cs中 AddRenderPasses函数下 需要将材质球初始化判断
if(settings.material == null)
{
    Debug.LogError("材质球丢失");//本句可隐藏
    return;
}

02.Volume.cs中需要添加以下两个函数
 public bool IsActive()
{
    return active;
}
public bool IsTileCompatible()
{
    return false;
}
