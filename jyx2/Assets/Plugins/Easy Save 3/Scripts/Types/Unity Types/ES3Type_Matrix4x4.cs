using System;
using UnityEngine;
using System.Collections.Generic;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("col0", "col1", "col2", "col3")]
	public class ES3Type_Matrix4x4 : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Matrix4x4() : base(typeof(Matrix4x4))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Matrix4x4 casted = (Matrix4x4)obj;
			writer.WriteProperty("col0", casted.GetColumn(0), ES3Type_Vector4.Instance);
			writer.WriteProperty("col1", casted.GetColumn(1), ES3Type_Vector4.Instance);
			writer.WriteProperty("col2", casted.GetColumn(2), ES3Type_Vector4.Instance);
			writer.WriteProperty("col3", casted.GetColumn(3), ES3Type_Vector4.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var matrix = new Matrix4x4();
			matrix.SetColumn(0,	reader.ReadProperty<Vector4>(ES3Type_Vector4.Instance));
			matrix.SetColumn(1,	reader.ReadProperty<Vector4>(ES3Type_Vector4.Instance));
			matrix.SetColumn(2,	reader.ReadProperty<Vector4>(ES3Type_Vector4.Instance));
			matrix.SetColumn(3,	reader.ReadProperty<Vector4>(ES3Type_Vector4.Instance));
			return matrix;
		}
	}

		public class ES3Type_Matrix4x4Array : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_Matrix4x4Array() : base(typeof(Matrix4x4[]), ES3Type_Matrix4x4.Instance)
		{
			Instance = this;
		}
	}
}