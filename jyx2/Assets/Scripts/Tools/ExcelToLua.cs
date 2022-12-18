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
    public enum LuaFieldType : byte
    {
        l_unknown,
        l_boolean,
        l_number,
        l_string,
    }
    public class ColDesc
    {
        public int index = -1;
        public string comment = "";
        public string typeStr = "";
        public string name = "";
        public List<LuaFieldType> type;
        public bool isArray = false;
    }
    /// <summary>
    /// 负责将Excel配置文件转换为Lua表
    /// </summary>
    public static class ExcelToLua
    {
        private static readonly char[] structSplit = {','};
        private static readonly char[] arraySplit = {'|'};
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
        public static List<ColDesc> GetColDesc(DataRowCollection sheet, int colNum)
        {
            List<ColDesc> colDescList = new List<ColDesc>();
            for (int i = 0; i < colNum; i++)
            {
                string name = sheet[3][i].ToString().Trim();
                if (name.StartsWith("#") || string.IsNullOrEmpty(name)) continue;
                string comment = sheet[1][i].ToString().Trim();
                string typeStr = sheet[2][i].ToString().Trim().ToLower();

                bool isArray = typeStr.Contains("[]");
                typeStr = typeStr.Replace("[]", "");
                List<LuaFieldType> fieldType = new List<LuaFieldType>();
                try{
                    var typeList = typeStr.Split(structSplit, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string t in typeList)
                    {
                        fieldType.Add((LuaFieldType)Enum.Parse(typeof(LuaFieldType), t));
                    }
                }
                catch{
                    Debug.LogError($"Lua field type {typeStr} not found, at (3,{i+1})");
                    fieldType.Add(LuaFieldType.l_string);
                }

                ColDesc colDesc = new ColDesc();
                colDesc.index = i;
                colDesc.name = name;
                colDesc.comment = comment;
                colDesc.typeStr = typeStr;
                colDesc.type = fieldType;
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
            Debug.Log(configType);
            //获取表格字段信息
            List<ColDesc> colDescList = GetColDesc(sheet, colNum);
            StringBuilder sb = new StringBuilder();
            sb.Append("--[[\n本文件由编辑器自动生成，如需修改请先修改Excel表格后再使用Unity生成本文件\n\n金庸群侠传3D重制版\nhttps://github.com/jynew/jynew\n\n这是本开源项目文件头，所有代码均使用MIT协议。\n但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。\n\n金庸老先生千古！\n]]\n");
            if (colDescList == null || colDescList.Count <=0)
            {
                return sb.ToString();
            }

            Dictionary<string, int> fieldIndexMap = new Dictionary<string, int>();
            for (int i = 0; i < colDescList.Count; i++)
            {
                fieldIndexMap[colDescList[i].name] = i + 1;
            }

            sb.Append("local fieldIdx = {}\n");
            foreach (var cur in fieldIndexMap)
            {
                sb.Append(string.Format("fieldIdx.{0} = {1}\n", cur.Key, cur.Value));
            }

            sb.Append("local data = {");
            int startRowIdx = 4;

            for (int i = startRowIdx; i < rowNum; i++)
            {
                var firstElement = sheet[i][0].ToString().Trim();
                if (firstElement.StartsWith("#") || string.IsNullOrEmpty(firstElement)) continue;

                StringBuilder oneRow = new StringBuilder();
                oneRow.Append("{");

                for (int j = 0; j < colDescList.Count; j++)
                {
                    ColDesc curCol = colDescList[j];
                    string content = sheet[i][curCol.index].ToString();

                    if (!curCol.isArray)
                    {
                        content = GetLuaValue(curCol.type, content);
                        oneRow.Append(content);
                    }
                    else
                    {
                        StringBuilder tmpSB = new StringBuilder("{");
                        var tmpStringList = content.Split(arraySplit, StringSplitOptions.RemoveEmptyEntries);
                        for (int k = 0; k < tmpStringList.Length; k++)
                        {
                            tmpStringList[k] = GetLuaValue(curCol.type, tmpStringList[k]);
                            tmpSB.Append(tmpStringList[k]);
                            if (k != tmpStringList.Length - 1)
                            {
                                tmpSB.Append(",");
                            }
                        }

                        oneRow.Append(tmpSB);
                        oneRow.Append("}");
                    }

                    if (j != colDescList.Count - 1)
                    {
                        oneRow.Append(",");
                    }
                }

                oneRow.Append("},");
                sb.Append(string.Format("\n{0}", oneRow));
            }

            sb.Append("}\n");

            string str =
                "local mt = {}\n" +
                "mt.__index = function(a,b)\n" +
                "\tif fieldIdx[b] then\n" +
                "\t\treturn a[fieldIdx[b]]\n" +
                "\tend\n" +
                "\treturn nil\n" +
                "end\n" +
                "mt.__newindex = function(t,k,v)\n" +
                "\terror('do not edit config')\n" +
                "end\n" +
                "mt.__metatable = false\n" +
                "for _,v in ipairs(data) do\n" +
                "\tsetmetatable(v,mt)\n" +
                "end\n" +
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
        private static string GetLuaValue(List<LuaFieldType> fieldType, string valueStr)
        {
            var vList = valueStr.Split(structSplit, fieldType.Count, StringSplitOptions.None);
            StringBuilder sb = new StringBuilder();
            if (vList.Length > 1)
                sb.Append("{");
            for (int i = 0; i < vList.Length; i++)
            {
                var v = vList[i];
                var t = fieldType[i];
                if (t == LuaFieldType.l_string)
                {
                    if (string.IsNullOrWhiteSpace(v))
                    {
                        sb.Append("\"\"");
                    }
                    else
                    {
                        sb.Append(string.Format("[[{0}]]", v));
                    }
                }
                else if (t == LuaFieldType.l_boolean)
                {
                    bool isOk = StringToBoolean(v);
                    sb.Append( isOk ? "true" : "false");
                }
                else
                {
                    sb.Append(string.IsNullOrEmpty(v.Trim()) ? "" : v.Trim());
                }
                if (i < vList.Length - 1)
                {
                    sb.Append(",");
                }
            }
            if (vList.Length > 1)
            {
                sb.Append("}");
            }
            return sb.ToString();
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
        /// <summary>
        /// 将所有Excel转化Lua并储存
        /// </summary>
        /// <param name="inDir">Excel目录</param>
        /// <param name="outDir">Lua目录</param>
        /// <returns></returns>
        public static void ExportAllLuaFile(string inDir, string outDir)
        {
            Debug.Log("xlsx to lua start.");
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
            Directory.CreateDirectory(outDir);
            if (!string.IsNullOrEmpty(content))
            {
                File.WriteAllText(outPath, content);
            }
        }
    }
}
