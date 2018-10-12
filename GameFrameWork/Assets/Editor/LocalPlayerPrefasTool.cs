using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 管理本地 PlayerPrefas 存储的数据
    /// </summary>
    public class LocalPlayerPrefasTool
    {
#if UNITY_EDITOR
        [MenuItem("Tools/PlayerPrefas管理/清理本地所有的数据")]
        private static void ClearAllLocalPlayerPrefasData()
        {
            string localKeys = PlayerPrefs.GetString(PlayerPrefsMgr.LocalKeyRecord,"");
            if(string.IsNullOrEmpty(localKeys))
            {
                PlayerPrefs.DeleteKey(PlayerPrefsMgr.LocalKeyRecord);
                return;
            }

            Dictionary<string, string> keysInfor = JsonMapper.ToObject<Dictionary<string, string>>(localKeys);
            foreach (var item in keysInfor)
            {
                PlayerPrefs.DeleteKey(item.Key);
            }
            PlayerPrefs.DeleteKey(PlayerPrefsMgr.LocalKeyRecord);
        }


        [MenuItem("Tools/PlayerPrefas管理/清理本地所有登录账户信息")]
        private static void ClearLocalPlayerPrefas_AccountInfor()
        {
            PlayerPrefs.DeleteKey(LocalAccountMgr.LocalAccountKey);
        }
#endif

    }
}