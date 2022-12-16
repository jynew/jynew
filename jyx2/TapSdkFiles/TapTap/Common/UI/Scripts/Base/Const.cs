using System;
namespace TapSDK.UI
{
    public enum EOpenMode
    {
        Sync,

        Async,
    }

    [Flags]
    public enum EAnimationMode
    {
        None = 0,

        Alpha = 1 << 0,

        Scale = 1 << 1,
        
        RightSlideIn = 1 << 2,

        AlphaAndScale =  Alpha | Scale,
    }
}