// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// A variation of the base <see cref="FootstepEvents"/> which responds to Animation Events called "Footstep" by
    /// playing a sound randomly selected from an array, using the Int Parameter of the event as the index to determine
    /// which foot to play the sound on. 0 is the Left Foot and 1 is the Right Foot.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/events/footsteps">Footstep Events</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Events/FootstepEventsAnimation
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Footstep Events - Animation")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Events) + "/" + nameof(FootstepEventsAnimation))]
    public sealed class FootstepEventsAnimation : FootstepEvents
    {
        /************************************************************************************************************************/

        [SerializeField] private AudioSource[] _FootSources;

        /************************************************************************************************************************/

        // Called by Animation Events.
        private void Footstep(int foot)
        {
            PlaySound(_FootSources[foot]);
        }

        /************************************************************************************************************************/
    }
}
