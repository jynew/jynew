// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    partial class AnimancerToolsWindow
    {
        /// <summary>[Editor-Only] [Pro-Only] 
        /// A <see cref="Panel"/> for packing multiple <see cref="Texture2D"/>s into a single image.
        /// </summary>
        /// <remarks>
        /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/tools/pack-textures">Pack Textures</see>
        /// </remarks>
        [Serializable]
        public sealed class PackTextures : Panel
        {
            /************************************************************************************************************************/

            [SerializeField] private List<Object> _Textures;
            [SerializeField] private int _Padding;
            [SerializeField] private int _MaximumSize = 8192;

            [NonSerialized] private ReorderableList _TexturesDisplay;

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override string Name => "Pack Textures";

            /// <inheritdoc/>
            public override string HelpURL => Strings.DocsURLs.PackTextures;

            /// <inheritdoc/>
            public override string Instructions
            {
                get
                {
                    if (_Textures.Count == 0)
                        return "Select the textures you want to pack.";

                    return "Set the other details then click Pack and it will ask where you want to save the combined texture.";
                }
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override void OnEnable(int index)
            {
                base.OnEnable(index);
                AnimancerUtilities.NewIfNull(ref _Textures);
                _TexturesDisplay = CreateReorderableObjectList(_Textures, "Textures", true);
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override void DoBodyGUI()
            {
                GUILayout.BeginVertical();
                _TexturesDisplay.DoLayoutList();
                GUILayout.EndVertical();
                HandleDragAndDropIntoList(GUILayoutUtility.GetLastRect(), _Textures, overwrite: false);
                RemoveDuplicates(_Textures);

                BeginChangeCheck();
                var padding = EditorGUILayout.IntField("Padding", _Padding);
                EndChangeCheck(ref _Padding, padding);

                BeginChangeCheck();
                var maximumSize = EditorGUILayout.IntField("Maximum Size", _MaximumSize);
                maximumSize = Math.Max(maximumSize, 16);
                EndChangeCheck(ref _MaximumSize, maximumSize);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    GUI.enabled = _Textures.Count > 0;

                    if (GUILayout.Button("Clear"))
                    {
                        AnimancerGUI.Deselect();
                        RecordUndo();
                        _Textures.Clear();
                    }

                    GUI.enabled = _Textures.Count > 0;

                    if (GUILayout.Button("Pack"))
                    {
                        AnimancerGUI.Deselect();
                        Pack();
                    }
                }
                GUILayout.EndHorizontal();
            }

            /************************************************************************************************************************/

            /// <summary>Removes any items from the `list` that are the same as earlier items.</summary>
            private static void RemoveDuplicates<T>(IList<T> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if (item == null)
                        continue;

                    for (int j = 0; j < i; j++)
                    {
                        if (item.Equals(list[j]))
                        {
                            list.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            /************************************************************************************************************************/

            /// <summary>Combines the <see cref="_Textures"/> into a new one and saves it.</summary>
            private void Pack()
            {
                var textures = GatherTextures();
                if (!MakeTexturesReadable(textures))
                    return;

                var path = GetCommonDirectory(_Textures);

                path = EditorUtility.SaveFilePanelInProject("Save Packed Texture", "PackedTexture", "png",
                    "Where would you like to save the packed texture?", path);

                if (string.IsNullOrEmpty(path))
                    return;

                try
                {
                    const string ProgressTitle = "Packing";
                    EditorUtility.DisplayProgressBar(ProgressTitle, "Packing", 0);

                    var packedTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false);

                    var uvs = packedTexture.PackTextures(textures, _Padding, _MaximumSize);

                    EditorUtility.DisplayProgressBar(ProgressTitle, "Encoding", 0.4f);
                    var bytes = packedTexture.EncodeToPNG();
                    if (bytes == null)
                        return;

                    EditorUtility.DisplayProgressBar(ProgressTitle, "Writing", 0.5f);
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();

                    var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                    importer.maxTextureSize = Math.Max(packedTexture.width, packedTexture.height);
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Multiple;
                    importer.spritesheet = new SpriteMetaData[0];
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();

                    // Use the UV coordinates to set up sprites for the new texture.
                    EditorUtility.DisplayProgressBar(ProgressTitle, "Generating Sprites", 0.7f);

                    var sprites = new List<Sprite>();
                    var spriteSheet = new List<SpriteMetaData>();
                    for (int iTexture = 0; iTexture < textures.Length; iTexture++)
                    {
                        var texture = textures[iTexture];

                        sprites.Clear();
                        GatherSprites(sprites, texture);

                        var rect = uvs[iTexture];
                        rect.x *= packedTexture.width;
                        rect.y *= packedTexture.height;
                        rect.width *= packedTexture.width;
                        rect.height *= packedTexture.height;

                        for (int iSprite = 0; iSprite < sprites.Count; iSprite++)
                        {
                            var sprite = sprites[iSprite];

                            var spriteRect = rect;
                            spriteRect.x += spriteRect.width * sprite.rect.x / sprite.texture.width;
                            spriteRect.y += spriteRect.height * sprite.rect.y / sprite.texture.height;
                            spriteRect.width *= sprite.rect.width / sprite.texture.width;
                            spriteRect.height *= sprite.rect.height / sprite.texture.height;

                            spriteSheet.Add(new SpriteMetaData
                            {
                                name = sprite.name,
                                rect = spriteRect,
                                alignment = (int)GetAlignment(sprite.pivot),
                                pivot = sprite.pivot,
                                border = sprite.border,
                            });
                        }
                    }
                    importer.spritesheet = spriteSheet.ToArray();

                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();

                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            /************************************************************************************************************************/

            private Texture2D[] GatherTextures()
            {
                var textures = new List<Texture2D>();

                for (int i = 0; i < _Textures.Count; i++)
                {
                    var obj = _Textures[i];
                    if (obj is Texture2D texture)
                        textures.Add(texture);
                    if (obj is DefaultAsset)
                        GatherTexturesRecursive(textures, AssetDatabase.GetAssetPath(obj));
                }

                RemoveDuplicates(textures);

                return textures.ToArray();
            }

            private static void GatherTexturesRecursive(List<Texture2D> textures, string path)
            {
                var guids = AssetDatabase.FindAssets($"t:{nameof(Texture2D)}", new string[] { path });
                for (int i = 0; i < guids.Length; i++)
                {
                    path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (texture != null)
                        textures.Add(texture);
                }
            }

            /************************************************************************************************************************/

            private static void GatherSprites(List<Sprite> sprites, Texture2D texture)
            {
                var path = AssetDatabase.GetAssetPath(texture);
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                var foundSprite = false;
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i] is Sprite sprite)
                    {
                        sprites.Add(sprite);
                        foundSprite = true;
                    }
                }

                if (!foundSprite)
                {
                    var sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    sprite.name = texture.name;
                    sprites.Add(sprite);
                }
            }

            /************************************************************************************************************************/

            private static bool MakeTexturesReadable(Texture2D[] textures)
            {
                var hasAsked = false;

                for (int i = 0; i < textures.Length; i++)
                {
                    var texture = textures[i];
                    var path = AssetDatabase.GetAssetPath(texture);
                    var importer = (TextureImporter)AssetImporter.GetAtPath(path);

                    if (importer.isReadable &&
                        importer.textureCompression == TextureImporterCompression.Uncompressed)
                        continue;

                    if (!hasAsked)
                    {
                        if (!EditorUtility.DisplayDialog("Make Textures Readable and Uncompressed?",
                            "This tool requires the source textures to be marked as readable and uncompressed in their import settings.",
                            "Make Textures Readable and Uncompressed", "Cancel"))
                            return false;
                        hasAsked = true;
                    }

                    importer.isReadable = true;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.SaveAndReimport();
                }

                return true;
            }

            /************************************************************************************************************************/

            private static string GetCommonDirectory<T>(IList<T> objects) where T : Object
            {
                if (objects == null)
                    return null;

                var count = objects.Count;
                if (count == 0)
                    return null;

                var path = AssetDatabase.GetAssetPath(objects[0]);
                path = Path.GetDirectoryName(path);

                for (int i = 1; i < count; i++)
                {
                    var otherPath = AssetDatabase.GetAssetPath(objects[i]);
                    otherPath = Path.GetDirectoryName(otherPath);

                    while (string.Compare(path, 0, otherPath, 0, path.Length) != 0)
                    {
                        path = Path.GetDirectoryName(path);
                    }
                }

                return path;
            }

            /************************************************************************************************************************/

            private static SpriteAlignment GetAlignment(Vector2 pivot)
            {
                switch (pivot.x)
                {
                    case 0:
                        switch (pivot.y)
                        {
                            case 0: return SpriteAlignment.BottomLeft;
                            case 0.5f: return SpriteAlignment.BottomCenter;
                            case 1: return SpriteAlignment.BottomRight;
                        }
                        break;
                    case 0.5f:
                        switch (pivot.y)
                        {
                            case 0: return SpriteAlignment.LeftCenter;
                            case 0.5f: return SpriteAlignment.Center;
                            case 1: return SpriteAlignment.RightCenter;
                        }
                        break;
                    case 1:
                        switch (pivot.y)
                        {
                            case 0: return SpriteAlignment.TopLeft;
                            case 0.5f: return SpriteAlignment.TopCenter;
                            case 1: return SpriteAlignment.TopRight;
                        }
                        break;
                }

                return SpriteAlignment.Custom;
            }

            /************************************************************************************************************************/
        }
    }
}

#endif

