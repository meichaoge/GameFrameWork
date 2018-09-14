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

        public LocalAccountInfor(string accountName,string passworld)
        {
            AccountName = accountName;
            AccountPassworld = DataProcessor.Instance.EncryptData(passworld);
            LastLoginTime = TimeHelper.Instance.DateTime2Second_Now();
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