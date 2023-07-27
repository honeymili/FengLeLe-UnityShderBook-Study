# Postproseccing: Tonemapping-ACES后处理

分为:    01.创建一个cs RenderFeature 参数调节和pass都在一个文件里(TonemappingACESRenderPassFeature.cs)
        02.创建三个个cs 使用volume调节参数  feature.cs /pass.cs /volume.cs (ACESRenderFeatureVolume /ACESPass /ACESVolume)

管线渲染器 Renderer 添加feature.cs文件 挂上材质球

注意报错 NullReferenceException 译为:空引用异常

问题常在 Feature类下的Create函数中 需要添加 
01.pass类里的settings = Feature类里的settings 
02.或者pass类里的material = Feature类里的material

