using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 本地账户存储信息
    /// </summary>
    [System.Serializable]
    public class LocalAccountInfor
    {
        public string AccountName;
        public string AccountPassworld;//加密后的密码
        public long LastLoginTime; //上一次登录时候的时间
       
        /// <summary>
        /// 标识是否加密了
        /// </summary>
        public bool IsEncrypt { get; private set; }

        /// <summary>
        /// 为了Json 反序列化
        /// </summary>
        public LocalAccountInfor()
        {
            IsEncrypt = false;
        }

        public LocalAccountInfor(string accountName,string passworld)
        {
            AccountName = accountName;
            IsEncrypt = false;
            AccountPassworld=passworld;
            LastLoginTime = TimeHelper.Instance.DateTime2Second_Now();
        }

        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="passworld"></param>
        public void EncryptPassworld( )
        {
            if(IsEncrypt)
            {
                Debug.LogError("当前已经进行了加密处理");
                return;
            }
            IsEncrypt = true;
            AccountPassworld = DataProcessor.Instance.EncryptData(AccountPassworld);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="passworld"></param>
        public void DecryptPassworld()
        {
            if (IsEncrypt == false)
            {
                Debug.LogError("当前密码没有加密");
                return;
            }
            IsEncrypt = false;
            AccountPassworld = DataProcessor.Instance.DecryptData(AccountPassworld);
        }

    }

    /// <summary>
    /// 本地所有的账户信息
    /// </summary>
    [System.Serializable]
    public class LocalAccount
    {
        public List<LocalAccountInfor> AllUseLocalAccount = new List<LocalAccountInfor>();
    }

}