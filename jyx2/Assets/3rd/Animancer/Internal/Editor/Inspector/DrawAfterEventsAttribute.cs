// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] 
    /// Causes an Inspector field in an <see cref="ITransition"/> to be drawn after its events where the events would
    /// normally be drawn last.
    /// </summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/DrawAfterEventsAttribute
    /// 
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawAfterEventsAttribute : Attribute { }
}

#endif

