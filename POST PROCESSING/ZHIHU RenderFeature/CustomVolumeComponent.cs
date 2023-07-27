////https://zhuanlan.zhihu.com/p/642579757
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("POST/ZH-Color")]
public class CustomVolumeComponent : VolumeComponent
{
    //ColorParameter 等同于color变量
    public ColorParameter cp = new ColorParameter(Color.white, true);
    public bool IsActive()
    {
        return active;
    }
    public bool IsTileCompatible()
    {
        return false;
    }
}
