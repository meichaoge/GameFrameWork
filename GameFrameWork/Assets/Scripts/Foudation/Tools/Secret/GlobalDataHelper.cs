using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class GlobalDataHelper
{
    private const string DATA_ENCRYPT_KEY = "a125431235125veqwrqw442312311233";
    private static RijndaelManaged _encryptAlgorithm = null;

    public static RijndaelManaged DataEncryptAlgorithm()
    {
        _encryptAlgorithm = new RijndaelManaged();
        _encryptAlgorithm.Key = Encoding.UTF8.GetBytes(DATA_ENCRYPT_KEY);
        _encryptAlgorithm.Mode = CipherMode.ECB;
        _encryptAlgorithm.Padding = PaddingMode.PKCS7;
        return _encryptAlgorithm;
    }
}