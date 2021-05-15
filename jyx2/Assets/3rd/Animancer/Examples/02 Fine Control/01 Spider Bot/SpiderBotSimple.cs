// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.FineControl
{
    /// <summary>A <see cref="SpiderBot"/> with a single movement animation for demonstration purposes.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/spider-bot">Spider Bot</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.FineControl/SpiderBotSimple
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Fine Control - Spider Bot Simple")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(FineControl) + "/" + nameof(SpiderBotSimple))]
    public sealed class SpiderBotSimple : SpiderBot
    {
        /************************************************************************************************************************/

        protected override bool IsMoving => Input.GetKey(KeyCode.Space);

        /************************************************************************************************************************/

        [SerializeField] private ClipState.Transition _Move;

        protected override ITransition MovementAnimation => _Move;

        /************************************************************************************************************************/
    }
}
