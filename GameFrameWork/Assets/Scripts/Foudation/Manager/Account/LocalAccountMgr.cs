using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 管理本地账户的读取和存储
    /// </summary>
    public class LocalAccountMgr : Singleton_Static<LocalAccountMgr>
    {
        #region Data 
        private const string S_LocalAccountKey = "GameFrame_Account"; //本地账户存储的名称

        private LocalAccount m_LocalAccountData = null;  //本地所有的账户信息
        /// <summary>
        /// 本地所有的账户
        /// </summary>
        public LocalAccount LocalAccountData
        {
            get
            {
                if (m_LocalAccountData == null)
                {
                    string data = PlayerPrefsMgr.Instance.GetString(S_LocalAccountKey);
                    if (string.IsNullOrEmpty(data))
                    {
                        m_LocalAccountData = new LocalAccount();
                    }
                    else
                    {
                        m_LocalAccountData = JsonMapper.ToObject<LocalAccount>(data);
                    }
                }
                return m_LocalAccountData;
            }
        }
        #endregion

        /// <summary>
        /// 获取上一次登录的账户作为默认的账户
        /// </summary>
        /// <returns>返回NULL  说明没有登录过</returns>
        public LocalAccountInfor GetLastLoginInAccount()
        {
            if (LocalAccountData.AllUseLocalAccount.Count == 0)
                return null;
            return LocalAccountData.AllUseLocalAccount[LocalAccountData.AllUseLocalAccount.Count - 1];
        }

        /// <summary>
        /// 更新本地的账户信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passworld"></param>
        public void UpdateLocalAccount(string accountName,string passworld)
        {
            bool isAlradyRecord = false;

            for (int dex=0;dex< LocalAccountData.AllUseLocalAccount.Count;++dex)
            {
                if (LocalAccountData.AllUseLocalAccount[dex].AccountName== accountName)
                {
                    isAlradyRecord = true;
                    LocalAccountData.AllUseLocalAccount[dex].AccountPassworld = DataProcessor.Instance.EncryptData(passworld);
                    break;
                }
            }

            if(isAlradyRecord==false)
            {
                LocalAccountInfor newAccount = new LocalAccountInfor(accountName, passworld);
                LocalAccountData.AllUseLocalAccount.Add(newAccount);
            }

            PlayerPrefsMgr.Instance.SetString(S_LocalAccountKey, JsonMapper.ToJson(LocalAccountData)); //保存数据
           
        }

        /// <summary>
        /// 获取加密的密码的真是密码
        /// </summary>
        /// <param name="passworld"></param>
        /// <returns></returns>
        public string GetAccountPassworld(string passworld)
        {
            return DataProcessor.Instance.DecryptData(passworld);
        }
    }
}