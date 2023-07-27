using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("POST/Tonemapping-ACES")]
public class ACESVolume : VolumeComponent, IPostProcessComponent
{
    //float Int 由 FloatParameter 取代 （数值）
     public FloatParameter PostExposure = new FloatParameter(0.6f);
    //Range 由 ClampedIntParameter 取代 （最小值，默认值，最大值）
    //public ClampedFloatParameter Adapted_lum = new ClampedFloatParameter(0f, -10, 10);
    [Tooltip("Film_Slope")]
    public FloatParameter slope = new ClampedFloatParameter(2.51f, 0f, 3f);
    [Tooltip("Film_Toe")]
    public ClampedFloatParameter toe = new ClampedFloatParameter(0.03f, 0.0f, 1.0f);
    [Tooltip("Film_Shoulder")]
    public ClampedFloatParameter shoulder = new ClampedFloatParameter(2.43f, 0.0f, 3.0f);
    [Tooltip("Film_BlackClip")]
    public ClampedFloatParameter blackClip = new ClampedFloatParameter(0.59f, 0.0f, 1.0f);
    [Tooltip("Film_WhiteClip")]
    public ClampedFloatParameter whiteClip = new ClampedFloatParameter(0.14f, 0.0f, 1.0f);

    // public bool IsActive() => PostExposure.value > 0f;

    // public bool IsTileCompatible() => false;
    
    public bool IsActive()
    {
        return active;
    }
    public bool IsTileCompatible()
    {
        return false;
    }
}
