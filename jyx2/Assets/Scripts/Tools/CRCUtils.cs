using System;
using System.Security.Cryptography;
using System.Text;

namespace Jyx2
{
    /// <summary>
    /// 从原来的SaveManager中移过来。GG 2018-0115
    /// </summary>
    public static class CRCUtils
    {
        static private string PwKey = "zbl06";

#region secure
        //原始代码如此，不敢改 GG
        static private string saltValue = "fuck1" + PwKey;
        static private string pwdValue = PwKey + "fuck1";

        /// <summary>
        /// 带CRC的加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string crcjm(string input)
        {
            string enc = jm(input);
            string crc = CRC16_C(input);
            return crc + "@" + enc;
        }

        /// <summary>
        /// 带CRC的解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string crcm(string input)
        {
            string[] sp = input.Split(new char[] { '@' });
            if (sp.Length != 2)
            {
                return "";
            }
            else
            {
                string crc = sp[0];
                string denc = m(sp[1]);
                string crc_check = CRC16_C(denc);
                if (crc != crc_check)
                    return "";
                return denc;
            }
        }

        #region 加密

        private static string jm(string Message)
        {
            byte[] buffer;
            UTF8Encoding encoding = new UTF8Encoding();
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] buffer2 = provider.ComputeHash(encoding.GetBytes("$t611@"));
            TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider
            {
                Key = buffer2,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] bytes = encoding.GetBytes(Message);
            try
            {
                buffer = provider2.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            }
            finally
            {
                provider2.Clear();
                provider.Clear();
            }
            return Convert.ToBase64String(buffer);
        }

        #endregion

        #region 解密

        private static string m(string Message)
        {
            byte[] buffer;
            UTF8Encoding encoding = new UTF8Encoding();
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] buffer2 = provider.ComputeHash(encoding.GetBytes("$t611@"));
            TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider
            {
                Key = buffer2,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] inputBuffer = Convert.FromBase64String(Message);
            try
            {
                buffer = provider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            }
            finally
            {
                provider2.Clear();
                provider.Clear();
            }
            return encoding.GetString(buffer);
        }

        #endregion

        #region CRC

        static private string CRC16_C(string str)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
            byte CRC16Lo;
            byte CRC16Hi;   //CRC寄存器 
            byte CL;
            byte CH;       //多项式码&HA001 
            byte SaveHi;
            byte SaveLo;
            byte[] tmpData;
            int Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x01;
            CH = 0xA0;
            tmpData = data;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位 
                    CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位 
                    if ((SaveHi & 0x01) == 0x01)
                    { //如果高位字节最后一位为1 
                        CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1 
                    }             //否则自动补0 
                    if ((SaveLo & 0x01) == 0x01)
                    { //如果LSB为1，则与多项式码进行异或 
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }
            return string.Format("{0}{1}", CRC16Hi, CRC16Lo);
        }
        #endregion
        #endregion
    }
}
