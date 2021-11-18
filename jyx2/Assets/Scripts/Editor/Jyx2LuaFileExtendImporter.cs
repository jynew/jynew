using UnityEngine;
using System.IO;
using UnityEditor.AssetImporters;

[ScriptedImporter( 1, "lua" )]
public class H2LuaImporter : ScriptedImporter {
    public override void OnImportAsset( AssetImportContext ctx ) {
        TextAsset subAsset = new TextAsset( File.ReadAllText( ctx.assetPath ) );
        ctx.AddObjectToAsset( "text", subAsset );
        ctx.SetMainObject( subAsset );
    }
}