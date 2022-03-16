using System;
using UnityEngine;
using System.Collections.Generic;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("boneIndex0", "boneIndex1", "boneIndex2", "boneIndex3", "weight0", "weight1", "weight2", "weight3")]
	public class ES3Type_BoneWeight : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_BoneWeight() : base(typeof(BoneWeight))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			BoneWeight casted = (BoneWeight)obj;

			writer.WriteProperty("boneIndex0", casted.boneIndex0, ES3Type_int.Instance);
			writer.WriteProperty("boneIndex1", casted.boneIndex1, ES3Type_int.Instance);
			writer.WriteProperty("boneIndex2", casted.boneIndex2, ES3Type_int.Instance);
			writer.WriteProperty("boneIndex3", casted.boneIndex3, ES3Type_int.Instance);

			writer.WriteProperty("weight0", casted.weight0, ES3Type_float.Instance);
			writer.WriteProperty("weight1", casted.weight1, ES3Type_float.Instance);
			writer.WriteProperty("weight2", casted.weight2, ES3Type_float.Instance);
			writer.WriteProperty("weight3", casted.weight3, ES3Type_float.Instance);

		}

		public override object Read<T>(ES3Reader reader)
		{
			var obj = new BoneWeight();

			obj.boneIndex0 = reader.ReadProperty<int>(ES3Type_int.Instance);
			obj.boneIndex1 = reader.ReadProperty<int>(ES3Type_int.Instance);
			obj.boneIndex2 = reader.ReadProperty<int>(ES3Type_int.Instance);
			obj.boneIndex3 = reader.ReadProperty<int>(ES3Type_int.Instance);

			obj.weight0 = reader.ReadProperty<float>(ES3Type_float.Instance);
			obj.weight1 = reader.ReadProperty<float>(ES3Type_float.Instance);
			obj.weight2 = reader.ReadProperty<float>(ES3Type_float.Instance);
			obj.weight3 = reader.ReadProperty<float>(ES3Type_float.Instance);

			return obj;
		}
	}

	public class ES3Type_BoneWeightArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_BoneWeightArray() : base(typeof(BoneWeight[]), ES3Type_BoneWeight.Instance)
		{
			Instance = this;
		}
	}
}