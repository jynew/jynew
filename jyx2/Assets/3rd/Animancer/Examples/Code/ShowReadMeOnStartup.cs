// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only, Internal] Automatically selects the <see cref="ReadMe"/> on startup.</summary>
    /// <remarks>This script is in the Examples folder so that it gets deleted along with them.</remarks>
    [InitializeOnLoad]
    internal static class ShowReadMeOnStartup
    {
        /************************************************************************************************************************/

        static ShowReadMeOnStartup()
        {
            const string SessionStateKey = "Animancer.HasShownReadMe";

            if (SessionState.GetBool(SessionStateKey, false))
                return;

            EditorApplication.delayCall += () =>
            {
                var asset = FindReadMe();

                if (asset == null ||
                    asset.DontShowOnStartup)
                    return;

                SessionState.SetBool(SessionStateKey, true);

                EditorApplication.delayCall += () =>
                    Selection.activeObject = asset;
            };
        }

        /************************************************************************************************************************/

        private static ReadMe FindReadMe()
        {
            var guids = AssetDatabase.FindAssets("t:ReadMe");
            for (int i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<ReadMe>(assetPath);
                if (asset != null)
                    return asset;
            }

            return null;
        }

        /************************************************************************************************************************/
    }
}

#endif
