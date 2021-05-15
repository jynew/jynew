// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using UnityEngine;

namespace Animancer.Examples.FineControl
{
    /// <summary>An object that can be interacted with.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/doors">Doors</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.FineControl/IInteractable
    /// 
    public interface IInteractable
    {
        /************************************************************************************************************************/

        void Interact();

        /************************************************************************************************************************/
    }

    /// <summary>
    /// Attempts to interact with whatever <see cref="IInteractable"/> the cursor is pointing at when the user clicks
    /// the mouse.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/doors">Doors</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.FineControl/ClickToInteract
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Fine Control - Click To Interact")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(FineControl) + "/" + nameof(ClickToInteract))]
    public sealed class ClickToInteract : MonoBehaviour
    {
        /************************************************************************************************************************/

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                var interactable = raycastHit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                    interactable.Interact();
            }
        }

        /************************************************************************************************************************/
    }
}
