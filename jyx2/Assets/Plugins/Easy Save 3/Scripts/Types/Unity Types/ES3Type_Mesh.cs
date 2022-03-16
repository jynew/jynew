using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("bounds", "subMeshCount", "boneWeights", "bindposes", "vertices", "normals", "tangents", "uv", "uv2", "uv3", "uv4", "colors32", "triangles", "subMeshes")]
	public class ES3Type_Mesh : ES3UnityObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_Mesh() : base(typeof(UnityEngine.Mesh))
		{
			Instance = this;
		}

		protected override void WriteUnityObject(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Mesh)obj;

            if(!instance.isReadable)
            {
                ES3Internal.ES3Debug.LogWarning("Easy Save cannot save the vertices for this Mesh because it is not marked as readable, so it will be stored by reference. To save the vertex data for this Mesh, check the 'Read/Write Enabled' checkbox in its Import Settings.", instance);
                return;
            }

			#if UNITY_2017_3
			writer.WriteProperty("indexFormat", instance.indexFormat);
			#endif
			writer.WriteProperty("vertices", instance.vertices, ES3Type_Vector3Array.Instance);
			writer.WriteProperty("triangles", instance.triangles, ES3Type_intArray.Instance);
			writer.WriteProperty("bounds", instance.bounds, ES3Type_Bounds.Instance);
			writer.WriteProperty("boneWeights", instance.boneWeights, ES3Type_BoneWeightArray.Instance);
			writer.WriteProperty("bindposes", instance.bindposes, ES3Type_Matrix4x4Array.Instance);
			writer.WriteProperty("normals", instance.normals, ES3Type_Vector3Array.Instance);
			writer.WriteProperty("tangents", instance.tangents, ES3Type_Vector4Array.Instance);
			writer.WriteProperty("uv", instance.uv, ES3Type_Vector2Array.Instance);
			writer.WriteProperty("uv2", instance.uv2, ES3Type_Vector2Array.Instance);
			writer.WriteProperty("uv3", instance.uv3, ES3Type_Vector2Array.Instance);
			writer.WriteProperty("uv4", instance.uv4, ES3Type_Vector2Array.Instance);
			writer.WriteProperty("colors32", instance.colors32, ES3Type_Color32Array.Instance);
			writer.WriteProperty("subMeshCount", instance.subMeshCount, ES3Type_int.Instance);
			for(int i=0; i<instance.subMeshCount; i++)
				writer.WriteProperty("subMesh"+i, instance.GetTriangles(i), ES3Type_intArray.Instance);
		}

		protected override object ReadUnityObject<T>(ES3Reader reader)
		{
			var obj = new Mesh();
			ReadUnityObject<T>(reader, obj);
			return obj;
		}

		protected override void ReadUnityObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Mesh)obj;
            if (instance == null)
                return;

            if (!instance.isReadable)
                ES3Internal.ES3Debug.LogWarning("Easy Save cannot load the vertices for this Mesh because it is not marked as readable, so it will be loaded by reference. To load the vertex data for this Mesh, check the 'Read/Write Enabled' checkbox in its Import Settings.", instance);

            foreach (string propertyName in reader.Properties)
			{
                // If this Mesh isn't readable, we should skip past all of its properties.
                if (!instance.isReadable)
                {
                    reader.Skip();
                    continue;
                }

                switch (propertyName)
				{
					#if UNITY_2017_3
					case "indexFormat":
						instance.indexFormat = reader.Read<UnityEngine.Rendering.IndexFormat>();
						break;
					#endif
					case "bounds":
						instance.bounds = reader.Read<UnityEngine.Bounds>(ES3Type_Bounds.Instance);
						break;
					case "boneWeights":
						instance.boneWeights = reader.Read<UnityEngine.BoneWeight[]>(ES3Type_BoneWeightArray.Instance);
						break;
					case "bindposes":
						instance.bindposes = reader.Read<UnityEngine.Matrix4x4[]>(ES3Type_Matrix4x4Array.Instance);
						break;
					case "vertices":
						instance.vertices = reader.Read<UnityEngine.Vector3[]>(ES3Type_Vector3Array.Instance);
						break;
					case "normals":
						instance.normals = reader.Read<UnityEngine.Vector3[]>(ES3Type_Vector3Array.Instance);
						break;
					case "tangents":
						instance.tangents = reader.Read<UnityEngine.Vector4[]>(ES3Type_Vector4Array.Instance);
						break;
					case "uv":
						instance.uv = reader.Read<UnityEngine.Vector2[]>(ES3Type_Vector2Array.Instance);
						break;
					case "uv2":
						instance.uv2 = reader.Read<UnityEngine.Vector2[]>(ES3Type_Vector2Array.Instance);
						break;
					case "uv3":
						instance.uv3 = reader.Read<UnityEngine.Vector2[]>(ES3Type_Vector2Array.Instance);
						break;
					case "uv4":
						instance.uv4 = reader.Read<UnityEngine.Vector2[]>(ES3Type_Vector2Array.Instance);
						break;
					case "colors32":
						instance.colors32 = reader.Read<UnityEngine.Color32[]>(ES3Type_Color32Array.Instance);
						break;
					case "triangles":
						instance.triangles = reader.Read<System.Int32[]>(ES3Type_intArray.Instance);
						break;
					case "subMeshCount":
						instance.subMeshCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						for(int i=0; i<instance.subMeshCount; i++)
							instance.SetTriangles(reader.ReadProperty<int[]>(ES3Type_intArray.Instance), i);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
