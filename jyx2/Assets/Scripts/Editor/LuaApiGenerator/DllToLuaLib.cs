using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

public class DllToLuaLib : Editor {

	private static string[] LUA_KEYWORDS = { "local", "function", "end", "then" };
	private static string[] DLL_NAMES = {
		"mscorlib",
#if UNITY_2017_1_OR_NEWER
		"UnityEngine.CoreModule",
#else
		"UnityEngine",
#endif
		"Assembly-CSharp",
	};

	[MenuItem("XLua/DllToLuaLib", false, -100)]
	private static void GenDlls()
	{
		Dictionary<string, Type[]> dllNameDict = new Dictionary<string, Type[]>();
		List<MethodInfo> allExtensionMethodList = new List<MethodInfo>();
		foreach (string dllName in DLL_NAMES)
		{
			Assembly assembly = null;
			try
			{
				assembly = Assembly.Load(dllName);
			}
			catch (FileNotFoundException) { }

			if (assembly != null)
			{
				Type[] types = assembly.GetTypes();
				dllNameDict[dllName] = types;
				foreach (Type type in types)
				{
					if (type.IsDefined(typeof(ExtensionAttribute), false))
					{
						MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
						foreach (MethodInfo method in methods)
						{
							if (method.IsDefined(typeof(ExtensionAttribute), false))
							{
								allExtensionMethodList.Add(method);
							}
						}
					}
				}
			}
			else
			{
				Debug.LogError(dllName + " assembly is not exist!");
			}
		}

		foreach (string dllName in dllNameDict.Keys)
		{
			Dictionary<string, byte[]> fileDict = new Dictionary<string, byte[]>();
			Type[] types = dllNameDict[dllName];
			foreach (Type type in types)
			{
				string fileName;
				string content;
				GenType(type, GetExtensionMethods(type, allExtensionMethodList), out fileName, out content);
				fileDict[fileName] = Encoding.UTF8.GetBytes(content);

				Type baseType = type.BaseType;
				while (baseType != null && baseType.IsGenericType && !baseType.IsGenericTypeDefinition)
				{
					string baseFileName;
					string baseContent;
					GenType(baseType, GetExtensionMethods(baseType, allExtensionMethodList), out baseFileName, out baseContent);
					fileDict[baseFileName] = Encoding.UTF8.GetBytes(baseContent);
					baseType = baseType.BaseType;
				}
			}
			HashSet<string> nameSpaceSet = new HashSet<string>();
			foreach (Type type in types)
			{
				string nameSpace = type.Namespace;
				if (nameSpace != null && !nameSpaceSet.Contains(nameSpace))
				{
					nameSpaceSet.Add(nameSpace);
				}
			}
			foreach (string nameSpace in nameSpaceSet)
			{
				string fileName = nameSpace + ".ns.lua";
				StringBuilder contentSb = new StringBuilder();
				contentSb.Append("---@class ");
				contentSb.Append(nameSpace);
				contentSb.AppendLine();
				contentSb.Append(nameSpace);
				contentSb.Append(" = {}");
				string content = contentSb.ToString();
				fileDict[fileName] = Encoding.UTF8.GetBytes(content);
			}
			string zipFileName = Application.dataPath + "/../LuaLib/" + dllName + ".zip";
			WriteZip(zipFileName, fileDict);
			Debug.Log(dllName + ".zip generating is complete!");
		}
	}

	private static List<MethodInfo> GetExtensionMethods(Type extendedType, List<MethodInfo> allExtensionMethodList)
	{
		List<MethodInfo> extensionMethodList = new List<MethodInfo>();
		foreach (MethodInfo extensionMethod in allExtensionMethodList)
		{
			ParameterInfo[] parameters = extensionMethod.GetParameters();
			if (parameters[0].ParameterType == extendedType)
			{
				extensionMethodList.Add(extensionMethod);
			}
		}
		return extensionMethodList;
	}

	private static void GenType(Type type, List<MethodInfo> extensionMethodList, out string fileName, out string content)
	{
		string typeName = TypeToString(type, false, true);
		string typeFileName = typeName + ".lua";
		StringBuilder typeScriptSb = new StringBuilder();
		typeScriptSb.Append("---@class ");
		typeScriptSb.Append(typeName);
		typeScriptSb.Append(" : ");
		if (type.BaseType != null)
		{
			typeScriptSb.Append(TypeToString(type.BaseType, false, true));
		}
		else
		{
			typeScriptSb.Append("table");
		}
		typeScriptSb.AppendLine();

		FieldInfo[] staticFields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		foreach (FieldInfo field in staticFields)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(field.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(field.FieldType));
			typeScriptSb.Append(" @static");
			typeScriptSb.AppendLine();
		}
		PropertyInfo[] staticProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		foreach (PropertyInfo property in staticProperties)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(property.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(property.PropertyType));
			typeScriptSb.Append(" @static");
			typeScriptSb.AppendLine();
		}

		FieldInfo[] instanceFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach (FieldInfo field in instanceFields)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(field.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(field.FieldType));
			typeScriptSb.AppendLine();
		}
		PropertyInfo[] instanceProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach (PropertyInfo property in instanceProperties)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(property.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(property.PropertyType));
			typeScriptSb.AppendLine();
		}

		typeScriptSb.Append("local m = {}");
		typeScriptSb.AppendLine();

		Dictionary<string, List<MethodInfo>> methodNameDict = new Dictionary<string, List<MethodInfo>>();
		MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach (MethodInfo method in methods)
		{
			string methodName = method.Name;
			if (!methodName.StartsWith("get_") && !methodName.StartsWith("set_"))
			{
				if (!methodNameDict.ContainsKey(methodName))
				{
					methodNameDict.Add(methodName, new List<MethodInfo>());
				}
				methodNameDict[methodName].Add(method);
			}
		}
		foreach (MethodInfo method in extensionMethodList)
		{
			string methodName = method.Name;
			if (!methodName.StartsWith("get_") && !methodName.StartsWith("set_"))
			{
				if (!methodNameDict.ContainsKey(methodName))
				{
					methodNameDict.Add(methodName, new List<MethodInfo>());
				}
				methodNameDict[methodName].Add(method);
			}
		}

		foreach (string methodName in methodNameDict.Keys)
		{
			typeScriptSb.AppendLine();

			List<MethodInfo> methodList = methodNameDict[methodName];
			List<List<ParameterInfo>> paramListList = new List<List<ParameterInfo>>();
			List<List<ParameterInfo>> returnListList = new List<List<ParameterInfo>>();
			List<MethodInfo> fromMethodList = new List<MethodInfo>();
			for (int methodIndex = 0; methodIndex < methodList.Count; methodIndex++)
			{
				MethodInfo method = methodList[methodIndex];
				List<ParameterInfo> paramList = new List<ParameterInfo>();
				List<ParameterInfo> returnList = new List<ParameterInfo>();
                if (method.ReturnParameter.ParameterType != typeof(void))
				{
					returnList.Add(method.ReturnParameter);
				}
				ParameterInfo[] parameters = method.GetParameters();
				for (int paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
				{
					if (!extensionMethodList.Contains(method) || paramIndex != 0)
					{
						ParameterInfo param = parameters[paramIndex];
						if (!param.IsOut)
						{
							paramList.Add(param);
						}
						if (param.ParameterType.IsByRef)
						{
							returnList.Add(param);
						}
					}
				}
				paramListList.Add(paramList);
				returnListList.Add(returnList);
				fromMethodList.Add(method);
				for (int paramIndex = paramList.Count - 1; paramIndex >= 0; paramIndex--)
				{
					ParameterInfo param = paramList[paramIndex];
					if (param.IsOptional || param.IsDefined(typeof(ParamArrayAttribute), false))
					{
						List<ParameterInfo> overloadParamList = new List<ParameterInfo>();
						for (int index = 0; index < paramIndex; index++)
						{
							overloadParamList.Add(paramList[index]);
						}
						paramListList.Add(overloadParamList);
						returnListList.Add(returnList);
						fromMethodList.Add(method);
					}
				}
			}
			for (int overloadIndex = 1; overloadIndex < paramListList.Count; overloadIndex++)
			{
				typeScriptSb.Append("---@overload ");
				typeScriptSb.Append(MethodToString(paramListList[overloadIndex], returnListList[overloadIndex]));
				MethodInfo method = fromMethodList[overloadIndex];
				if (method.IsStatic)
				{
					if (extensionMethodList.Contains(method))
					{
						typeScriptSb.Append(" @extension");
					}
					else
					{
						typeScriptSb.Append(" @static");
					}
				}
				if (method.IsAbstract)
				{
					typeScriptSb.Append(" @abstract");
				}
				else if (method.IsVirtual)
				{
					typeScriptSb.Append(" @virtual");
				}
				typeScriptSb.AppendLine();
			}
			{
				List<ParameterInfo> paramList = paramListList[0];
				List<ParameterInfo> returnList = returnListList[0];
				MethodInfo method = fromMethodList[0];
				if (method.IsStatic)
				{
					if (extensionMethodList.Contains(method))
					{
						typeScriptSb.AppendLine("---@extension");
					}
					else
					{
						typeScriptSb.AppendLine("---@static");
					}
				}
				if (method.IsAbstract)
				{
					typeScriptSb.AppendLine("---@abstract");
				}
				else if (method.IsVirtual)
				{
					typeScriptSb.AppendLine("---@virtual");
				}
				for (int paramIndex = 0; paramIndex < paramList.Count; paramIndex++)
				{
                    ParameterInfo param = paramList[paramIndex];
                    typeScriptSb.Append("---@param ");
					if (param.IsDefined(typeof(ParamArrayAttribute), false))
					{
						typeScriptSb.Append("... ");
						typeScriptSb.Append(TypeToString(param.ParameterType.GetElementType()));
						typeScriptSb.Append("|");
					}
					else
					{
						typeScriptSb.Append(GetParamName(param));
						typeScriptSb.Append(" ");
					}
					typeScriptSb.Append(TypeToString(param.ParameterType));
					typeScriptSb.AppendLine();
				}
				if (returnList.Count > 0)
				{
					typeScriptSb.Append("---@return ");
					typeScriptSb.Append(TypeToString(returnList[0].ParameterType));
					for (int returnIndex = 1; returnIndex < returnList.Count; returnIndex++)
					{
						typeScriptSb.Append(", ");
						typeScriptSb.Append(TypeToString(returnList[returnIndex].ParameterType));
					}
					typeScriptSb.AppendLine();
				}

				typeScriptSb.Append("function m");
				if (method.IsStatic)
				{
					typeScriptSb.Append(".");
				}
				else
				{
					typeScriptSb.Append(":");
				}
				typeScriptSb.Append(methodName);
				typeScriptSb.Append("(");
				for (int paramIndex = 0; paramIndex < paramList.Count; paramIndex++)
				{
					if (paramIndex > 0)
					{
						typeScriptSb.Append(", ");
					}
					ParameterInfo param = paramList[paramIndex];
					typeScriptSb.Append(param.IsDefined(typeof(ParamArrayAttribute), false) ? "..." : GetParamName(param));
				}
				typeScriptSb.Append(") end");
				typeScriptSb.AppendLine();
			}
		}

		typeScriptSb.AppendLine();
		typeScriptSb.Append(typeName);
		typeScriptSb.AppendLine(" = m");
		typeScriptSb.AppendLine("return m");

		fileName = typeFileName;
		content = typeScriptSb.ToString();
	}

	private static string TypeToString(Type type, bool inFun = false, bool classDefine = false)
	{
		if (!classDefine)
        {
            if (type == typeof(object))
            {
                return "any";
            }

            if (type == typeof(sbyte) || type == typeof(byte) ||
				type == typeof(short) || type == typeof(ushort) ||
				type == typeof(int) || type == typeof(uint) ||
				type == typeof(long) || type == typeof(ulong) ||
				type == typeof(float) || type == typeof(double) ||
				type == typeof(char))
			{
				return "number";
			}

			if (type == typeof(string) || type == typeof(byte[]))
			{
				return "string";
			}

			if (type == typeof(bool))
			{
				return "boolean";
			}

			if (type.IsArray)
			{
				return TypeToString(type.GetElementType(), inFun) + "[]";
            }

            if (type.IsGenericType)
            {
                Type[] genericArgTypes = type.GetGenericArguments();
                if (genericArgTypes.Length == 1 && typeof(IList<>).MakeGenericType(genericArgTypes).IsAssignableFrom(type))
                {
                    return TypeToString(genericArgTypes[0], inFun) + "[]";
                }

                if (genericArgTypes.Length == 2 && typeof(IDictionary<,>).MakeGenericType(genericArgTypes).IsAssignableFrom(type))
                {
                    if (genericArgTypes[0] != typeof(string))
                    {
                        return "table<" + TypeToString(genericArgTypes[0]) + ", " + TypeToString(genericArgTypes[1]) + ">";
                    }
                }
            }

			if (typeof(Delegate).IsAssignableFrom(type))
			{
				MethodInfo method = type == typeof(Delegate) || type == typeof(MulticastDelegate) ?
					type.GetMethod("DynamicInvoke") : type.GetMethod("Invoke");
				return MethodToString(method, inFun);
			}
		}

		if (type.FullName == null)
		{
			//GenericTypeDefinition like T
			return TypeToString(type.BaseType ?? typeof(object), inFun, classDefine);
		}

        char[] typeNameChars = type.ToString().ToCharArray();
        StringBuilder sb = new StringBuilder();
        int brackets = 0;
        for (int index = 0; index < typeNameChars.Length; index++)
        {
            // Generic: “`[,]”，ByRef：“&”，Nested：“+”，Other：“<>$=”
            // We want no “.” in “[]” or "<>"
            char c = typeNameChars[index];
            if (c == '[' || c == '<')
            {
                brackets++;
                c = '_';
            }
            else if (c == ']' || c == '>')
            {
                brackets--;
                c = '_';
            }
            else if (c == '.' || c == '+')
            {
                if (brackets > 0)
                {
                    c = '_';
                }
                else
                {
                    c = '.';
                }
            }
            else if (c == '`' || c == ',' || c == '$' || c == '=')
            {
                c = '_';
            }
            if (c != '&')
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
	}

	private static string MethodToString(MethodInfo method, bool inFun = false)
	{
		if (method == null)
		{
			return "any";
		}

		List<ParameterInfo> paramList = new List<ParameterInfo>();
		List<ParameterInfo> returnList = new List<ParameterInfo>();
		if (method.ReturnParameter.ParameterType != typeof(void))
		{
			returnList.Add(method.ReturnParameter);
		}
		ParameterInfo[] parameters = method.GetParameters();
		for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
		{
			ParameterInfo param = parameters[parameterIndex];
			if (!param.IsOut)
			{
				// !out
				paramList.Add(param);
			}
			if (param.ParameterType.IsByRef)
			{
				// out | ref
				returnList.Add(param);
			}
		}

		return MethodToString(paramList, returnList, inFun);
	}

	private static string MethodToString(List<ParameterInfo> paramList, List<ParameterInfo> returnList, bool inFun = false)
	{
		StringBuilder sb = new StringBuilder();
		if (inFun && returnList.Count > 0)
		{
			sb.Append("(");
		}
		sb.Append("fun(");
		for (int paramIndex = 0; paramIndex < paramList.Count; paramIndex++)
		{
			if (paramIndex > 0)
			{
				sb.Append(", ");
			}
			ParameterInfo param = paramList[paramIndex];
			if (param.IsDefined(typeof(ParamArrayAttribute), false))
			{
				sb.Append("...:");
				sb.Append(TypeToString(param.ParameterType.GetElementType()));
				sb.Append("|");
			}
			else
			{
				sb.Append(GetParamName(param));
				sb.Append(":");
			}
			sb.Append(TypeToString(param.ParameterType));
		}
		sb.Append(")");
		if (returnList.Count > 0)
		{
			sb.Append(":");
			if (returnList.Count > 1)
			{
				sb.Append("(");
			}
			for (int returnIndex = 1; returnIndex < returnList.Count; returnIndex++)
			{
				if (returnIndex > 0)
				{
					sb.Append(", ");
				}
				sb.Append(TypeToString(returnList[returnIndex].ParameterType, returnList.Count == 1));
			}
			if (returnList.Count > 1)
			{
				sb.Append(")");
			}
			if (inFun)
			{
				sb.Append(")");
			}
		}
		return sb.ToString();
	}

	private static string GetParamName(ParameterInfo param)
	{
		string paramName = param.Name;
		for (int index = 0; index < LUA_KEYWORDS.Length; index++)
		{
			if (string.Equals(paramName, LUA_KEYWORDS[index]))
			{
				paramName = "_" + paramName;
				break;
			}
		}
		return paramName;
	}

	private static void WriteZip(string zipFileName, Dictionary<string, byte[]> fileDict, int compressionLevel = 9)
	{
		FileInfo zipFile = new FileInfo(zipFileName);
		DirectoryInfo dir = zipFile.Directory;
		if (!dir.Exists)
		{
			dir.Create();
		}
		FileStream fileStream = zipFile.Create();
		ZipOutputStream zipStream = new ZipOutputStream(fileStream);
		zipStream.SetLevel(compressionLevel);
		foreach (string fileName in fileDict.Keys)
		{
			zipStream.PutNextEntry(new ZipEntry(fileName));
			byte[] buffer = fileDict[fileName];
			zipStream.Write(buffer, 0, buffer.Length);
		}
		zipStream.Close();
		fileStream.Close();
	}
}
