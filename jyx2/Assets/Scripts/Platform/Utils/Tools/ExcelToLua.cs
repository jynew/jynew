/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Excel;
using UnityEngine;

namespace Jyx2.Middleware
{
    /// <summary>
    /// Lua字段类型枚举
    /// </summary>
    public enum LuaFieldType : byte
    {
        l_boolean,
        l_number,
        l_string,
        l_struct,
    }
    /// <summary>
    /// 配置表列的描述
    /// </summary>
    public class ColDesc
    {
        public int index = -1;
        public string comment = "";
        public string typeStr = "";
        public string name = "";
        public string[] subName;
        public LuaFieldType type;
        public List<LuaFieldType> subType;
        public bool isArray = false;
    }
    /// <summary>
    /// 负责将Excel配置文件转换为Lua表
    /// </summary>
    public static class ExcelToLua
    {
        //struct结构的分隔符
        private static readonly char[] structSplit = {','};
        //array中各项的分隔符
        private static readonly char[] arraySplit = {'|'};
        //Name与subName的分隔符
        private static readonly char[] nameSplit = {'-'};
        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="col">Excel列的数目</param>
        /// <param name="row">Excel行的数目</param>
        /// <returns>以行为元素的表</returns>
        static DataRowCollection ReadExcel(string filePath, out int colNum, out int rowNum)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            IExcelDataReader excelReader;
            DataSet result;
            if (Path.GetExtension(filePath).ToUpper() == ".XLS")
            {
                //Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else
            {
                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            // Use the AsDataSet extension method
            result = excelReader.AsDataSet();
            //Tables[0] 下标0表示excel文件中第一张表的数据
            colNum = result.Tables[0].Columns.Count;
            rowNum = result.Tables[0].Rows.Count;
            stream.Close();
            return result.Tables[0].Rows; 
        }
        /// <summary>
        /// 解析Excel中的表头信息
        /// </summary>
        public static List<ColDesc> GetColDesc(DataRowCollection sheet, int colNum)
        {
            List<ColDesc> colDescList = new List<ColDesc>();
            for (int i = 0; i < colNum; i++)
            {
                string name = sheet[3][i].ToString().Trim();
                if (name.StartsWith("#") || string.IsNullOrEmpty(name)) continue;
                string comment = sheet[1][i].ToString().Trim();
                string typeStr = sheet[2][i].ToString().Trim().ToLower();

                string[] subName = {};
                LuaFieldType fieldType;
                List<LuaFieldType> fieldSubType = new List<LuaFieldType>();

                bool isArray = typeStr.Contains("[]");
                typeStr = typeStr.Replace("[]", "");
                try{
                    var typeList = typeStr.Split(structSplit, StringSplitOptions.RemoveEmptyEntries);
                    if (typeList.Length > 1)
                    {
                        fieldType = LuaFieldType.l_struct;
                        var tmpName = name.Split(nameSplit, StringSplitOptions.RemoveEmptyEntries);
                        if (tmpName.Length > 1)
                        {
                            name = tmpName[0];
                            subName = tmpName[1].Split(structSplit, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string t in typeList)
                            {
                                fieldSubType.Add((LuaFieldType)Enum.Parse(typeof(LuaFieldType), t));
                            }
                        }
                        else
                        {
                            foreach (string t in typeList)
                            {
                                fieldSubType.Add((LuaFieldType)Enum.Parse(typeof(LuaFieldType), t));
                            }
                        }
                    }
                    else
                    {
                        fieldType = (LuaFieldType)Enum.Parse(typeof(LuaFieldType), typeStr);
                    }
                }
                catch{
                    Debug.LogError($"Lua field type {typeStr} not found, at (3,{i+1})");
                    fieldType = LuaFieldType.l_string;
                }

                ColDesc colDesc = new ColDesc();
                colDesc.index = i;
                colDesc.name = name;
                colDesc.subName = subName;
                colDesc.comment = comment;
                colDesc.typeStr = typeStr;
                colDesc.type = fieldType;
                colDesc.subType = fieldSubType;
                colDesc.isArray = isArray;
                colDescList.Add(colDesc);
            }
            return colDescList;
        }
        /// <summary>
        /// 使用表格数据生成Lua字符串
        /// </summary>
        /// <param name="sheet">读取后的表格数据</param>
        /// <param name="colNum">表格列数</param>
        /// <param name="rowNum">表格行数</param>
        /// <returns>字符串形式的Lua表</returns>
        public static string GenLuaFile(DataRowCollection sheet, int colNum, int rowNum)
        {
            //获取表格名字
            string configType = sheet[0][1].ToString();

            //获取表格字段信息
            List<ColDesc> colDescList = GetColDesc(sheet, colNum);
            StringBuilder sb = new StringBuilder();
            sb.Append("--[[\n本文件由编辑器自动生成，如需修改请先修改Excel表格后再使用Unity生成本文件\n\n金庸群侠传3D重制版\nhttps://github.com/jynew/jynew\n\n这是本开源项目文件头，所有代码均使用MIT协议。\n但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。\n\n金庸老先生千古！\n]]\n");
            if (colDescList == null || colDescList.Count <=0)
            {
                return sb.ToString();//如果没有表头，就直接结束
            }

            //编写列名和列序号对应表
            Dictionary<string, int> fieldIndexMap = new Dictionary<string, int>();
            for (int i = 0; i < colDescList.Count; i++)
            {
                fieldIndexMap[colDescList[i].name] = i + 1;
            }

            sb.Append("local fieldIdx = {}\n");
            foreach (var cur in fieldIndexMap)
            {
                sb.AppendFormat("fieldIdx.{0} = {1}\n", cur.Key, cur.Value);
            }

            sb.Append("local data = {\n");

            int startRowIdx = 4;
            for (int i = startRowIdx; i < rowNum; i++)
            {
                //如果开头是#就忽略这行
                var firstElement = sheet[i][0].ToString().Trim();
                if (firstElement.StartsWith("#") || string.IsNullOrEmpty(firstElement)) continue;

                StringBuilder oneRow = new StringBuilder();
                oneRow.Append('{');

                for (int j = 0; j < colDescList.Count; j++)
                {
                    ColDesc curCol = colDescList[j];
                    string content = sheet[i][curCol.index].ToString().Trim();

                    if (!curCol.isArray)
                    {
                        content = GetLuaValue(curCol, content);
                        oneRow.Append(content);
                    }
                    else //如果是序列型
                    {
                        oneRow.Append('{');
                        var tmpStringList = content.Split(arraySplit, StringSplitOptions.RemoveEmptyEntries);
                        for (int k = 0; k < tmpStringList.Length; k++)
                        {
                            tmpStringList[k] = GetLuaValue(curCol, tmpStringList[k]);
                            oneRow.Append(tmpStringList[k]);
                            if (k != tmpStringList.Length - 1)
                            {
                                oneRow.Append(',');
                            }
                        }
                        oneRow.Append('}');
                    }

                    if (j != colDescList.Count - 1)
                    {
                        oneRow.Append(',');
                    }
                }

                oneRow.Append("},");
                sb.Append(oneRow);
                sb.Append('\n');
            }

            sb.Append("}\n");

            sb.AppendFormat("local helper = jy_utils.prequire('Jyx2Configs/{0}Helper')\n", configType);
            string helperstr =
                "\tif helper[b] then\n" +
                "\t\treturn helper[b]\n" +
                "\tend\n";
            string substr =
                "local mt{0} = {{}}\n" +
                "mt{0}.__index = function(a,b)\n" +
                "\tif fieldIdx{0}[b] then\n" +
                "\t\treturn a[fieldIdx{0}[b]]\n" +
                "\tend\n{1}" +
                "\treturn nil\n" +
                "end\n" +
                "mt{0}.__metatable = false\n" +
                "for _,v in pairs(data) do\n";
            sb.AppendFormat(substr,"", helperstr);
            sb.Append("\tsetmetatable(v,mt)\nend\n");
            foreach (var col in colDescList)
            {
                if (col.type == LuaFieldType.l_struct && col.subName.Length > 1)
                {
                    sb.AppendFormat("local fieldIdx{0} = {{}}\n",col.name);
                    for (int j = 0; j < col.subName.Length; j++)
                    {
                        sb.AppendFormat("fieldIdx{0}.{1} = {2}\n", col.name, col.subName[j], j+1);
                    }
                    sb.AppendFormat(substr, col.name, "");
                    sb.AppendFormat("\tfor _,t in pairs(v.{0}) do\n\t\tif type(t) == 'table' then\n\t\t\tsetmetatable(t,mt{0})\n\t\tend\n\tend\nend\n", col.name);
                }
            }

            string str =
                "local configMgr = Jyx2:GetModule('ConfigMgr')\n" +
                $"configMgr:AddConfigTable([[{configType}]], data)";
            sb.Append(str);
            return sb.ToString();
        }
        /// <summary>
        /// 处理字符串，输出标准的lua格式
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetLuaValue(ColDesc col, string valueStr)
        {
            var type = col.type;
            switch (col.type)
            {
                case LuaFieldType.l_string:
                    if (string.IsNullOrWhiteSpace(valueStr))
                    {
                        return "\"\"";
                    }
                    else
                    {
                        return string.Format("[[{0}]]", valueStr);
                    }
                    break;
                case LuaFieldType.l_boolean:
                    bool isOk = StringToBoolean(valueStr);
                    return isOk ? "true" : "false";
                    break;
                case LuaFieldType.l_number:
                    return string.IsNullOrEmpty(valueStr) ? "nil" : valueStr;
                    break;
                default:
                    var typeList = col.subType;
                    var vList = valueStr.Split(structSplit, StringSplitOptions.None);
                    StringBuilder sb = new StringBuilder('{');
                    for (int i = 0; i < vList.Length; i++)
                    {
                        var v = vList[i];
                        var t = typeList[i];
                        if (t == LuaFieldType.l_string)
                        {
                            if (string.IsNullOrWhiteSpace(v))
                            {
                                sb.Append("\"\"");
                            }
                            else
                            {
                                sb.AppendFormat("[[{0}]]", v);
                            }
                        }
                        else if (t == LuaFieldType.l_boolean)
                        {
                            isOk = StringToBoolean(v);
                            sb.Append( isOk ? "true" : "false");
                        }
                        else
                        {
                            sb.Append(string.IsNullOrEmpty(v) ? "nil" : v);
                        }
                        if (i < vList.Length - 1)
                        {
                            sb.Append(',');
                        }
                    }
                    if (vList.Length > 1)
                    {
                        sb.Insert(0, '{');
                        sb.Append("}");
                    }
                    return sb.ToString();
            }
        }

        /// <summary>
        /// 字符串转为bool型，非0和false即为真
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool StringToBoolean(string value)
        {
            value = value.ToLower().Trim();
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            if ("false" == value)
            {
                return false;
            }

            int num = -1;
            if (int.TryParse(value, out num))
            {
                if (0 == num)
                {
                    return false;
                }
            }

            return true;
        }
        public static void Clearfiles(string outDir)
        {
            DirectoryInfo dir = new DirectoryInfo(outDir);
            dir.Delete(true);
        }
        /// <summary>
        /// 将所有Excel转化Lua并储存
        /// </summary>
        /// <param name="inDir">Excel目录</param>
        /// <param name="outDir">Lua目录</param>
        /// <returns></returns>
        public static void ExportAllLuaFile(string inDir, string outDir)
        {
            Debug.Log("xlsx to lua start.");
            Clearfiles(outDir);
            Directory.CreateDirectory(outDir);
            var files = Directory.GetFiles(inDir, "*.xlsx", SearchOption.AllDirectories);
            foreach (var path in files)
            {
                //忽略临时文件
                if (path.Contains("~$")) continue;
                ExportSingleLuaFile(path, outDir);
            }
        }

        public static void ExportSingleLuaFile(string path, string outDir)
        {
            DataRowCollection sheet = ReadExcel(path, out int colNum, out int rowNum);
            //检查是否需要转化
            if (sheet[0][0].ToString() != "LuaConfigGen") return;
            string outPath = Path.Combine(outDir, sheet[0][1].ToString() + "Config.lua");
            string content = GenLuaFile(sheet, colNum, rowNum);
            if (!string.IsNullOrEmpty(content))
            {
                File.WriteAllText(outPath, content);
            }
        }
    }
}
