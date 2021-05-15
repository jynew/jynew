// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// Replaces the <see cref="SpriteRenderer.sprite"/> with a copy of it that uses a different <see cref="Texture"/>
    /// during every <see cref="LateUpdate"/>.
    /// </summary>
    /// 
    /// <remarks>This script is not specific to Animancer and will work with any animation system.</remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/SpriteRendererTextureSwap
    /// 
    [AddComponentMenu(Strings.MenuPrefix + "Sprite Renderer Texture Swap")]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(SpriteRendererTextureSwap))]
    [DefaultExecutionOrder(32000)]// Execute Last.
    public sealed class SpriteRendererTextureSwap : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The SpriteRenderer that will have its Sprite modified")]
        private SpriteRenderer _Renderer;

        /// <summary>The <see cref="SpriteRenderer"/> that will have its <see cref="Sprite"/> modified.</summary>
        public ref SpriteRenderer Renderer => ref _Renderer;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The replacement for the original Sprite texture")]
        private Texture2D _Texture;

        /// <summary>The replacement for the original <see cref="Sprite.texture"/>.</summary>
        /// <remarks>
        /// If this texture has any <see cref="Sprite"/>s set up in its import settings, they will be completely
        /// ignored because this system creates new <see cref="Sprite"/>s at runtime. The texture doesn't even need to
        /// be set to <see cref="Sprite"/> mode.
        /// <para></para>
        /// Call <see cref="ClearCache"/> before setting this if you want to destroy any sprites created for the
        /// previous texture.
        /// </remarks>
        public Texture2D Texture
        {
            get => _Texture;
            set
            {
                _Texture = value;
                RefreshSpriteMap();
            }
        }

        /************************************************************************************************************************/

        private Dictionary<Sprite, Sprite> _SpriteMap;

        private void RefreshSpriteMap() => _SpriteMap = GetSpriteMap(_Texture);

        /************************************************************************************************************************/

        private void Awake() => RefreshSpriteMap();

        private void OnValidate() => RefreshSpriteMap();

        /************************************************************************************************************************/

        private void LateUpdate()
        {
            if (_Renderer == null)
                return;

            var sprite = _Renderer.sprite;
            if (TrySwapTexture(_SpriteMap, _Texture, ref sprite))
                _Renderer.sprite = sprite;
        }

        /************************************************************************************************************************/

        /// <summary>Destroys all sprites created for the current <see cref="Texture"/>.</summary>
        public void ClearCache()
        {
            DestroySprites(_SpriteMap);
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<Texture2D, Dictionary<Sprite, Sprite>>
            TextureToSpriteMap = new Dictionary<Texture2D, Dictionary<Sprite, Sprite>>();

        /************************************************************************************************************************/

        /// <summary>Returns a cached dictionary mapping original sprites to duplicates using the specified `texture`.</summary>
        public static Dictionary<Sprite, Sprite> GetSpriteMap(Texture2D texture)
        {
            if (texture == null)
                return null;

            if (!TextureToSpriteMap.TryGetValue(texture, out var map))
                TextureToSpriteMap.Add(texture, map = new Dictionary<Sprite, Sprite>());

            return map;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If the <see cref="Sprite.texture"/> is not already using the specified `texture`, this method replaces the
        /// `sprite` with a cached duplicate which uses that `texture` instead.
        /// </summary>
        public static bool TrySwapTexture(Dictionary<Sprite, Sprite> spriteMap, Texture2D texture, ref Sprite sprite)
        {
            if (spriteMap == null ||
                sprite == null ||
                texture == null ||
                sprite.texture == texture)
                return false;

            if (!spriteMap.TryGetValue(sprite, out var otherSprite))
            {
                var pivot = sprite.pivot;
                pivot.x /= sprite.rect.width;
                pivot.y /= sprite.rect.height;

                otherSprite = Sprite.Create(texture,
                    sprite.rect, pivot, sprite.pixelsPerUnit,
                    0, SpriteMeshType.FullRect, sprite.border, false);

#if UNITY_ASSERTIONS
                var name = sprite.name;
                var originalTextureName = sprite.texture.name;
                var index = name.IndexOf(originalTextureName);
                if (index >= 0)
                {
                    var newName =
                        texture.name +
                        name.Substring(index + originalTextureName.Length, name.Length - (index + originalTextureName.Length));

                    if (index > 0)
                        newName = name.Substring(0, index) + newName;

                    name = newName;
                }

                otherSprite.name = name;
#endif

                spriteMap.Add(sprite, otherSprite);
            }

            sprite = otherSprite;
            return true;
        }

        /************************************************************************************************************************/

        /// <summary>Destroys all the <see cref="Dictionary{TKey, TValue}.Values"/>.</summary>
        public static void DestroySprites(Dictionary<Sprite, Sprite> spriteMap)
        {
            if (spriteMap == null)
                return;

            foreach (var sprite in spriteMap.Values)
                Destroy(sprite);

            spriteMap.Clear();
        }

        /************************************************************************************************************************/

        /// <summary>Destroys all sprites created for the `texture`.</summary>
        public static void DestroySprites(Texture2D texture)
        {
            if (TextureToSpriteMap.TryGetValue(texture, out var spriteMap))
            {
                TextureToSpriteMap.Remove(texture);
                DestroySprites(spriteMap);
            }
        }

        /************************************************************************************************************************/
    }
}
