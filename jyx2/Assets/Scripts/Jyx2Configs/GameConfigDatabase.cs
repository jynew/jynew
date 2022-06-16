using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Excel;
using UnityEngine;
using UnityEngine.Networking;

namespace Jyx2Configs
{
    public class GameConfigDatabase 
    {
        #region Singleton
        public static GameConfigDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameConfigDatabase();
                return _instance;
            }
        }

        private static GameConfigDatabase _instance;
        #endregion

        private readonly Dictionary<Type, Dictionary<int, Jyx2ConfigBase>> _dataBase =
            new Dictionary<Type, Dictionary<int, Jyx2ConfigBase>>();

        private bool _isInited = false;

        public string ModRootDir;

        /// <summary>
        /// 载入配置表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="configOverridePaths"></param>
        /// <returns></returns>
        public async UniTask Init(string rootPath, List<string> configOverridePaths)
        {
            if (_isInited)
                return;

            ModRootDir = rootPath;
            _isInited = true;
            _dataBase.Clear();
            int total = 0;
            
            
            foreach (var overridePath in configOverridePaths)
            {
                var fileName = overridePath.Split('/').Last();
                var sourcePath = Path.Combine(rootPath, "Configs", fileName);
                var destPath = Path.Combine(Application.persistentDataPath, fileName);
                total += await GenerateConfigsFromExcel(sourcePath, destPath);
            }
            
            Debug.Log($"载入完成，总数{total}个配置");
        }

        /// <summary>
        /// 根据ID获取Config
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(string id) where T : Jyx2ConfigBase
        {
            return Get<T>(int.Parse(id));
        }
        
        /// <summary>
        /// 根据ID获取Config
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(int id) where T : Jyx2ConfigBase
        {
            if(_dataBase.TryGetValue(typeof(T), out var configMap))
            {
                if (configMap.TryGetValue(id, out var v))
                {
                    return (T)v;
                }
            }

            Debug.LogError($"找不到Config:{typeof(T)}, id={id}");
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Has<T>(string id) where T : Jyx2ConfigBase
        {
            return Get<T>(id) != null;
        }
        
        /// <summary>
        /// 获取所有的Config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>() where T : Jyx2ConfigBase
        {
            if (_dataBase.TryGetValue(typeof(T), out var configMap))
            {
                foreach (var v in configMap)
                {
                    yield return (T) v.Value;
                }
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            _dataBase.Clear();
            _instance = null;
        }

        /// <summary>
        /// 获取Config的种类数量
        /// </summary>
        /// <returns></returns>
        public int GetConfigTypeCount()
        {
            return _dataBase.Count;
        }

        /// <summary>
        /// 生成配置
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        async UniTask<int> GenerateConfigsFromExcel(string sourcePath, string destPath)
        {
            int total = 0;
            
            await CopyFile(sourcePath, destPath);
            
            DataRowCollection collection = ReadExcel(destPath, out int col, out int row);

            //类名
            string classType = collection[0][0].ToString();
                
            //反射找类
            Type type = Type.GetType(classType);
            if (type == null)
            {
                Debug.LogError($"找不到{sourcePath}定义的数据类:{classType}");
                throw new Exception($"找不到{sourcePath}定义的数据类:{classType}");
            }

            //创建数据库
            _dataBase[type] = new Dictionary<int, Jyx2ConfigBase>();

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


                if (_dataBase[type].ContainsKey(config.Id))
                {
                    Debug.Log($"发现重复的ID，config={type}, id={config.Id}, 覆盖写入");
                }
                _dataBase[type][config.Id] = config;
                total++;
            }

            return total;
        }
        
        /// <summary>
        /// 拷贝文件到指定目录
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        async UniTask CopyFile(string sourcePath, string destPath)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
                {
                    var dh = new DownloadHandlerFile(sourcePath);
                    dh.removeFileOnAbort = true;
                    request.downloadHandler = dh;
                    await request.SendWebRequest();
                }
            }
            else
            {
                File.Copy(sourcePath, destPath, true);
            }
        }

        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        DataRowCollection ReadExcel(string filePath, out int col, out int row)
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
    }
}
