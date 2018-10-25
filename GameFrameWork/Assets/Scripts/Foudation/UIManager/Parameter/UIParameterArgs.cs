using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{

    /// <summary>
    /// UI 之间传递参数的桥梁
    /// </summary>
    public class UIParameterArgs
    {
        /// <summary>
        /// 包含的参数个数
        /// </summary>
        public int ParemeterCount { get { return m_Parameter.Count; } }
        private List<object> m_Parameter = new List<object>();  //包含真是的参数

        /// <summary>
        ///  包含真是的参数
        /// </summary>
        public object[] Parameter { get { return m_Parameter.ToArray(); } }

        /// <summary>
        /// 私有的构造函数 避免外界使用构造函数创建
        /// </summary>
        private UIParameterArgs() { }


        #region  获取创建接口
        /// <summary>
        /// 获取一个参数对象
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns></returns>
        public static UIParameterArgs Create()
        {
            UIParameterArgs result = new UIParameterArgs();
            return result;
        }

        /// <summary>
        /// 获取一个参数对象
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns></returns>
        public static UIParameterArgs Create(object obj1)
        {
            UIParameterArgs result = new UIParameterArgs();
            result.m_Parameter.Add(obj1);
            return result;
        }
        /// <summary>
        /// 获取一个参数对象
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static UIParameterArgs Create(object obj1, object obj2)
        {
            UIParameterArgs result = new UIParameterArgs();
            result.m_Parameter.Add(obj1);
            result.m_Parameter.Add(obj2);
            return result;
        }
        /// <summary>
        /// 获取一个参数对象
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <returns></returns>
        public static UIParameterArgs Create(object obj1, object obj2, object obj3)
        {
            UIParameterArgs result = new UIParameterArgs();
            result.m_Parameter.Add(obj1);
            result.m_Parameter.Add(obj2);
            result.m_Parameter.Add(obj3);
            return result;
        }
        /// <summary>
        /// 获取一个参数对象
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <param name="obj4"></param>
        /// <returns></returns>
        public static UIParameterArgs Create(object obj1, object obj2, object obj3, object obj4)
        {
            UIParameterArgs result = new UIParameterArgs();
            result.m_Parameter.Add(obj1);
            result.m_Parameter.Add(obj2);
            result.m_Parameter.Add(obj3);
            result.m_Parameter.Add(obj4);

            return result;
        }
        /// <summary>
        /// 获取一个参数对象
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <param name="obj4"></param>
        /// <param name="obj5"></param>
        /// <returns></returns>
        public static UIParameterArgs Create(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            UIParameterArgs result = new UIParameterArgs();
            result.m_Parameter.Add(obj1);
            result.m_Parameter.Add(obj2);
            result.m_Parameter.Add(obj3);
            result.m_Parameter.Add(obj4);
            result.m_Parameter.Add(obj5);

            return result;
        }



        /// <summary>
        /// 最好不要使用 （仅在参数合并时候使用）
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static UIParameterArgs Create( object[] objs)
        {
            UIParameterArgs result = new UIParameterArgs();
            if (objs == null || objs.Length == 0) return result;
            result.m_Parameter.AddRange(objs);
            return result;
        }
        #endregion


        #region 删除裁剪参数 

        /// <summary>
        /// 根据索引ID 获取数据
        /// </summary>
        /// <param name="dex"></param>
        /// <returns></returns>
        public object GetParameterByIndex(int dex)
        {
            if(dex<0||dex>= ParemeterCount)
            {
                throw new System.Exception(string.Format("参数异常  数据源个数={0} 获取索引={1}", ParemeterCount, dex));
            }

            return m_Parameter[dex];
        }

        /// <summary>
        /// 从beginIndex 开始 ,去除掉 count 个参数 
        /// </summary>
        /// <param name="beginIndex">开始的索引</param>
        /// <param name="count">去除参数的个数</param>
        public void CutParameterEnd(int beginIndex,int count)
        {
            if (beginIndex < 0 || beginIndex >= ParemeterCount)
            {
                throw new System.Exception(string.Format("参数异常  数据源个数={0} 获取索引={1}", ParemeterCount, beginIndex));
            }

            List<object> result = m_Parameter.GetRange(0, beginIndex + 1);
            if (result == null)
                result = new List<object>();

            for (int dex= beginIndex;dex    < ParemeterCount;++dex)
            {
                result.Add(m_Parameter[dex]);
            }
            m_Parameter = result;
        }

        /// <summary>
        /// 从beginIndex 开始向后 去除掉 count 个参数
        /// </summary>
        /// <param name="beginIndex">开始的索引</param>
        public void CutParameterEnd(int beginIndex)
        {
            if (beginIndex < 0 || beginIndex >= ParemeterCount)
            {
                throw new System.Exception(string.Format("参数异常  数据源个数={0} 获取索引={1}", ParemeterCount, beginIndex));
            }

            CutParameterEnd(beginIndex, ParemeterCount- beginIndex);
        }

        /// <summary>
        ///  删除从开始位置到 endinIndex 位置的参数
        /// </summary>
        /// <param name="endinIndex">删除从开始到endinIndex的参数并返回</param>
        public void CutParameterToFront(int endinIndex)
        {
            if (endinIndex < 0 || endinIndex >= ParemeterCount)
            {
                throw new System.Exception(string.Format("参数异常  数据源个数={0} 获取索引={1}", ParemeterCount, endinIndex));
            }

            m_Parameter = m_Parameter.GetRange(endinIndex+1, ParemeterCount-1 - endinIndex);
        }
        #endregion

        #region 增加/插入参数
        /// <summary>
        /// 在指定位置插入指定的参数 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="dex"></param>
        public void InsertParameter(object obj1,int dex)
        {
            if (dex < 0 || dex >= ParemeterCount)
            {
                throw new System.Exception(string.Format("参数异常  数据源个数={0} 获取索引={1}", ParemeterCount, dex));
            }
            m_Parameter.Insert(dex, obj1);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="dex"></param>
        public void AddParameter(object obj1, int dex)
        {
            m_Parameter.Add(obj1);
        }
        #endregion


    }
}