using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Excel;
using Jyx2Configs;
using ProtoBuf;
using UnityEngine;

namespace Jyx2.Middleware
{
    public static class ExcelTools
    {
        /// <summary>
        /// 生成配置
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static void GenerateConfigsFromExcel(string dirPath)
        {
            var files = Directory.GetFiles(dirPath, "*.xlsx", SearchOption.AllDirectories);
            
            Dictionary<Type, Dictionary<int, Jyx2ConfigBase>> dataBase =
                new Dictionary<Type, Dictionary<int, Jyx2ConfigBase>>();
            
            foreach (var path in files)
            { 
                DataRowCollection collection = ReadExcel(path, out int col, out int row);

                //类名
                string classType = collection[0][0].ToString();
                    
                //反射找类
                Type type = Type.GetType(classType);
                if (type == null)
                {
                    Debug.LogError($"找不到{path}定义的数据类:{classType}");
                    throw new Exception($"找不到{path}定义的数据类:{classType}");
                }

                //创建数据库
                dataBase[type] = new Dictionary<int, Jyx2ConfigBase>();

                //创建数据
                //从第4行开始有数据，第一行是类名，第二行是映射变量，第三行是说明。
                for (int i = 3; i < row; ++i)
                {
                    var obj = Activator.CreateInstance(type);

                    if (!(obj is Jyx2ConfigBase))
                    {
                        Debug.LogError($"类{type}没有继承ConfigBase!");
                        throw new Exception($"类{type}没有继承ConfigBase!");
                    }
                    
                    var firstElement = collection[i][0].ToString().Trim();
                    if (firstElement.StartsWith("#") || string.IsNullOrEmpty(firstElement)) //不需要打包的行, #开头或者空ID
                        continue;

                    for (int j = 0; j < col; ++j)    
                    {
                        string colName = collection[1][j].ToString();

                        if (colName.StartsWith("#") || string.IsNullOrEmpty(colName.Trim())) continue; //不需要打包的列，#开头或者空列
                        
                        FieldInfo variable = type.GetField(colName);
                        if (variable == null)
                        {
                            Debug.LogError($"{type}找不到字段{colName}");
                            throw new Exception($"{type}找不到字段{colName}");
                        }
                        variable.SetValue(obj, Convert.ChangeType(collection[i][j], variable.FieldType));
                    }

                    var config = obj as Jyx2ConfigBase;
                    if (config == null)
                    {
                        Debug.LogError($"{type} 没有继承Jyx2ConfigBase类");
                        throw new Exception($"{type} 没有继承Jyx2ConfigBase类");
                    }


                    if (dataBase[type].ContainsKey(config.Id))
                    {
                        Debug.Log($"发现重复的ID，config={type}, id={config.Id}, 覆盖写入");
                    }
                    dataBase[type][config.Id] = config;
                }
            }
            
            //生成二进制文件
            using (FileStream fstream = File.Create($"{dirPath}/StaticDatas.bin"))
            {
                Serializer.Serialize(fstream, dataBase);
            }
        }
        
        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        static DataRowCollection ReadExcel(string filePath, out int col, out int row)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            IExcelDataReader excelReader;
            //1. Reading Excel file
            if (Path.GetExtension(filePath).ToUpper() == ".XLS")
            {
                //1.1 Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else
            {
                //1.2 Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            DataSet result = excelReader.AsDataSet(); 
            //Tables[0] 下标0表示excel文件中第一张表的数据
            col = result.Tables[0].Columns.Count;
            row = result.Tables[0].Rows.Count;
            stream.Close();
            return result.Tables[0].Rows; 
        }

        public static Dictionary<Type, Dictionary<int, Jyx2ConfigBase>> LoadBinFile(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Serializer.Deserialize<Dictionary<Type, Dictionary<int, Jyx2ConfigBase>>>(ms);
            }
        }
    }
}