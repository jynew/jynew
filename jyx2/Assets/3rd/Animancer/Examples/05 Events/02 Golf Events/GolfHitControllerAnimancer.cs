// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// An <see cref="GolfHitController"/> that uses Animancer Events configured entirely in the Inspector.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/events/golf">Golf Events</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Events/GolfHitControllerAnimancer
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Golf Events - Animancer")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Events) + "/" + nameof(GolfHitControllerAnimancer))]
    public sealed class GolfHitControllerAnimancer : GolfHitController
    {
        /************************************************************************************************************************/

        // Nothing here.
        // This script is no different from the base GolfHitController.
        // It assumes the events are already fully configured in the Inspector.

        /************************************************************************************************************************/
    }
}
