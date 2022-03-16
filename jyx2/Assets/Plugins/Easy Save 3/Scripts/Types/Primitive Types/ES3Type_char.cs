namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_char : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_char() : base(typeof(char))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((char)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)reader.Read_char();
		}
	}
		public class ES3Type_charArray : ES3ArrayType
		{
			public static ES3Type Instance;

			public ES3Type_charArray() : base(typeof(char[]), ES3Type_char.Instance)
			{
				Instance = this;
			}
	}
}