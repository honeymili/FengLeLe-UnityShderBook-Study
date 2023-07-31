//https://zhuanlan.zhihu.com/p/626618386
using UnityEngine.Rendering;

[System.Serializable, VolumeComponentMenu("Gaussian Blur")]
public class BlurVolume : VolumeComponent
{
    public BoolParameter enable = new BoolParameter(false);
    public ClampedFloatParameter downScale = new ClampedFloatParameter(1, 1, 10);
}