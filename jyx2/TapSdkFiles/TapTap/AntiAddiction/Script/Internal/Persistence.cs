using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using LC.Newtonsoft.Json;
using TapTap.Common;

namespace TapTap.AntiAddiction.Internal 
{
    /// <summary>
    /// 通用 JSON 序列化工具
    /// </summary>
    internal class Persistence 
    {
        private readonly string _filePath;

        internal Persistence(string path) 
        {
            _filePath = path;
        }

        internal async Task<T> Load<T>() where T : class 
        {
            TapLogger.Debug(_filePath);
            if (!File.Exists(_filePath)) 
            {
                return null;
            }

            string text;
            using (FileStream fs = File.OpenRead(_filePath)) 
            {
                byte[] buffer = new byte[fs.Length];
                await fs.ReadAsync(buffer, 0, (int)fs.Length);
                text = Encoding.UTF8.GetString(buffer);
            }
            try 
            {
                return JsonConvert.DeserializeObject<T>(text);
            } 
            catch (Exception e) 
            {
                TapLogger.Error(e);
                Delete();
                return null;
            }
        }

        internal async Task Save<T>(T obj) 
        {
            if (obj == null) 
            {
                TapLogger.Error("Saved object is null.");
                return;
            }

            string text;
            try 
            {
                text = JsonConvert.SerializeObject(obj);
            } 
            catch (Exception e) 
            {
                TapLogger.Error(e);
                return;
            }

            string dirPath = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath)) 
            {
                Directory.CreateDirectory(dirPath);
            }

            using (FileStream fs = File.Create(_filePath))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        internal void Delete() 
        {
            if (!File.Exists(_filePath)) 
            {
                return;
            }

            File.Delete(_filePath);
        }
    }
}
