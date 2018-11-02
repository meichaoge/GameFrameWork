using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{

    /// <summary>
    /// Sprite 资源的相对( Resources)目录 
    /// </summary>
    public class Define_SpritePath
    {
 #region  Sprite 路径处理系统

        private static string SpecialSpritePath = "Sprite/{0}{1}/{2}";  //多语言版本的资源相对路径
        private static string NormalSpritePath = "Sprite/{0}";  //普通的资源相对路径

        /// <summary>
        /// Key=sprite  名称key value=对应的地址
        /// </summary>
        private static Dictionary<string, string> S_AllRecordSpriteInfor = new Dictionary<string, string>();

        /// <summary>
        /// 获取Sprite 对应的路径
        /// </summary>
        /// <param name="spriteKey"></param>
        /// <param name="IsSpecialResources"></param>
        /// <returns></returns>
        public static string GetSpriteInfor(string spriteKey, bool IsSpecialResources)
        {
            string result = string.Empty;
            if (S_AllRecordSpriteInfor.TryGetValue(spriteKey, out result))
                return result;

            result = GetSpritePath(spriteKey, IsSpecialResources);
            S_AllRecordSpriteInfor.Add(spriteKey, result);
            return result;
        }


        /// <summary>
        /// 根据当前所选择的语言以及相对的目录给出相对于Resources地址
        /// </summary>
        /// <param name="RelativePath"></param>
        /// <returns></returns>
        private static string GetSpritePath(string RelativePath, bool IsSpecialResources)
        {
            if(IsSpecialResources)
                return string.Format(SpecialSpritePath, ConstDefine.S_UILocalizationPathFileName, LanguageMgr.Instance.GetCurLanguageStr(), RelativePath);

            return string.Format(NormalSpritePath, RelativePath);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 只在编辑器运行状态下运行
        /// </summary>
        /// <param name="spriteKey"></param>
        /// <param name="language"></param>
        /// <param name="IsSpecialResources"></param>
        /// <returns></returns>
        public static string GetSpriteInfor(string spriteKey, Language language, bool IsSpecialResources)
        {
            if (Application.isPlaying )
            {
                Debug.LogError("这个接口只在编辑器非运行情况有效");
                return string.Empty;
            }

            if (IsSpecialResources)
                return string.Format(SpecialSpritePath, ConstDefine.S_UILocalizationPathFileName, LanguageMgr.Instance.GetLanguageStr(language), spriteKey);

            return string.Format(NormalSpritePath, spriteKey);
        }
#endif

        #endregion


        #region 主界面
        public static string S_btn_store {  get{  return GetSpriteInfor("MainMenu/btn_store",true); }  }
        public static string S_icon_paihangbang { get { return GetSpriteInfor("MainMenu/icon_paihangbang", true); } }
        public static string S_icon_zhujiemian_beibao { get { return GetSpriteInfor("MainMenu/icon_zhujiemian_beibao", true); } }
        public static string S_icon_zhujiemian_gonggao { get { return GetSpriteInfor("MainMenu/icon_zhujiemian_gonggao", true); } }
        public static string S_icon_zhujiemian_haoyou { get { return GetSpriteInfor("MainMenu/icon_zhujiemian_haoyou", true); } }
        public static string S_icon_zhujiemian_renwu { get { return GetSpriteInfor("MainMenu/icon_zhujiemian_renwu", true); } }
        public static string S_icon_zhujiemian_youjian { get { return GetSpriteInfor("MainMenu/icon_zhujiemian_youjian", true); } }

        #endregion



    }
}