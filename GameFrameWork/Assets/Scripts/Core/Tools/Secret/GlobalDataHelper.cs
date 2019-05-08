using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace GameFrameWork
{
    public class GlobalDataHelper
    {
        private const string DATA_ENCRYPT_KEY = "a125431235125veqwrqw442312311233";
        private static RijndaelManaged _encryptAlgorithm = null;

        /// <summary>
        /// 数据加密算法
        /// </summary>
        /// <returns></returns>
        public static RijndaelManaged DataEncryptAlgorithm()
        {
            _encryptAlgorithm = new RijndaelManaged();
            _encryptAlgorithm.Key = Encoding.UTF8.GetBytes(DATA_ENCRYPT_KEY);
            _encryptAlgorithm.Mode = CipherMode.ECB;
            _encryptAlgorithm.Padding = PaddingMode.PKCS7;
            return _encryptAlgorithm;
        }
    }
}