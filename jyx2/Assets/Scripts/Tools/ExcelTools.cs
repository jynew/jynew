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
        public static void GenerateConfigsFromExcel<T>(string dirPath) where T: Jyx2ConfigBase
        {
            var files = Directory.GetFiles(dirPath, "*.xlsx", SearchOption.AllDirectories);
            
            Dictionary<Type, Dictionary<int, T>> dataBase =
                new Dictionary<Type, Dictionary<int, T>>();
            
            foreach (var path in files)
            {
                if (path.Contains("~$")) continue; //临时文件
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
                dataBase[type] = new Dictionary<int, T>();

                //创建数据
                //从第4行开始有数据，第一行是类名，第二行是映射变量，第三行是说明。
                for (int i = 3; i < row; ++i)
                {
                    var obj = Activator.CreateInstance(type);

                    if (!(obj is T))
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

                    var config = obj as T;
                    if (config == null)
                    {
                        Debug.LogError($"{type} 没有继承{typeof(T)}类");
                        throw new Exception($"{type} 没有继承{typeof(T)}类");
                    }


                    if (dataBase[type].ContainsKey(config.Id))
                    {
                        Debug.Log($"发现重复的ID，config={type}, id={config.Id}, 覆盖写入");
                    }
                    dataBase[type][config.Id] = config;
                }
            }
            
            //生成二进制文件
            ProtobufSerialize($"{dirPath}/Datas.bytes", dataBase);
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
        
        #region 序列化和反序列化的方法

        /// <summary>
        /// 使用protobuf把对象序列化为Byte数组保存到本地
        /// </summary>
        /// <typeparam name="T">需要反序列化的对象类型，必须声明[ProtoContract]特征，且相应属性必须声明[ProtoMember(序号)]特征</typeparam>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void ProtobufSerialize<T>(string filePath, T obj)
        {
            using (var memory = new MemoryStream())
            {
                Serializer.Serialize(memory, obj);
                File.WriteAllBytes(filePath, memory.ToArray());
            }
        }

        /// <summary>
        /// 使用protobuf反序列化二进制数组为对象
        /// </summary>
        /// <typeparam name="T">需要反序列化的对象类型，必须声明[ProtoContract]特征，且相应属性必须声明[ProtoMember(序号)]特征</typeparam>
        /// <param name="data"></param>
        public static T ProtobufDeserialize<T>(Byte[] data) where T : class
        {
            using (var memory = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(memory);
            }
        }
        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// 缓存所有FileSystemWatcher，防止重复
        /// </summary>
        private static Dictionary<string, FileSystemWatcher> _cacheWatchers;

        /// <summary>
        /// 监听Configs目录下的所有Excel文件，当更改时，触发委托
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="action"></param>
        public static void WatchConfig(string dirPath, Action action)
        {
            if (!Application.isEditor)
            {
                Debug.LogError("[WatchConfig] Available in Unity Editor mode only!");
                return;
            }
            if (_cacheWatchers == null)
                _cacheWatchers = new Dictionary<string, FileSystemWatcher>();
            FileSystemWatcher watcher;
            if (!Directory.Exists(dirPath))
            {
                Debug.LogError($"[WatchConfig] Not found Dir: {dirPath}");
                return;
            }
            if (!_cacheWatchers.TryGetValue(dirPath, out watcher))
            {
                _cacheWatchers[dirPath] = watcher = new FileSystemWatcher(dirPath);
                Debug.Log($"Watching Config Dir: {dirPath}");
            }

            watcher.IncludeSubdirectories = false;
            watcher.Path = dirPath;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*";
            watcher.EnableRaisingEvents = true;
            watcher.InternalBufferSize = 2048;
            watcher.Changed += (sender, e) =>
            {
                Debug.Log($"Config changed: {e.FullPath}");
                action?.Invoke();
            };
        }
#endif
    }
}