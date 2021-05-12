using System.Collections;
using UnityEngine;

namespace ThreeEyedGames.DecaliciousExample
{
    public class ChangeDecal : IInteract
    {
        public Decal Target;
        public float LerpSeconds;

        public override void Interact()
        {
            StartCoroutine(SetDecalColor(GetComponent<MeshRenderer>().material.color));
        }

        IEnumerator SetDecalColor(Color targetColor)
        {
            Material material = Target.Material;
            Color from = material.color;
            for (float t = 0.0f; t <= 1.0f; t += Time.deltaTime / LerpSeconds)
            {
                material.color = Color.Lerp(from, targetColor, t);
                yield return null;
            }
        }
    }
}
