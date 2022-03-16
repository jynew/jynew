using System;
using System.ComponentModel;

namespace ES3Internal
{
	public class ES3Member
	{
		public string name;
		public Type type;
		public bool isProperty;
		public ES3Reflection.ES3ReflectedMember reflectedMember;
		public bool useReflection = false;

		public ES3Member(string name, Type type, bool isProperty)
		{
			this.name = name;
			this.type = type;
			this.isProperty = isProperty;
	 	}

		public ES3Member(ES3Reflection.ES3ReflectedMember reflectedMember)
		{
			this.reflectedMember = reflectedMember;
			this.name = reflectedMember.Name;
			this.type = reflectedMember.MemberType;
			this.isProperty = reflectedMember.isProperty;
			this.useReflection = true;
		}
	}
}
