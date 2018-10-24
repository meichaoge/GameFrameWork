using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    public class Helper : Singleton_Static<Helper>
    {

        #region 参数合并
        /// <summary>
        /// 合并两个参数
        /// </summary>
        /// <param name="agr0"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object[] MegerParameter(object agr0,  object[] parameter)
        {
            object[] result;
            if (parameter == null || parameter.Length == 0)
            {
                result = new object[] { agr0 };
            }
            else
            {
                result = new object[parameter.Length + 1];
                result[0] = agr0;
                System.Array.Copy(parameter, 0, result, 1, parameter.Length);
            }

            return result;
        }

        /// <summary>
        /// 合并两个列表参数
        /// </summary>
        /// <param name="agr0"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object[] MegerParameter(object[] agr0,  object[] parameter)
        {
            object[] result;
            if (parameter == null || parameter.Length == 0)
            {
                result = agr0;
            }
            else
            {
                if (agr0 == null || agr0.Length == 0)
                    result = parameter;
                else
                {
                    result = new object[agr0.Length + parameter.Length];
                    System.Array.Copy(agr0, 0, result, 0, agr0.Length);
                    System.Array.Copy(parameter, 0, result, agr0.Length + 1, parameter.Length);
                }
            }

            return result;
        }

        /// <summary>
        /// 对参数一的参数个数进行填充
        /// </summary>
        /// <param name="agr0"></param>
        /// <param name="parameterCount">期待的参数个数 </param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object[] AddParameter(object[] agr0, uint parameterCount, object[] parameter)
        {
            if (agr0 != null && agr0.Length > parameterCount) return agr0;
            object[] result = new object[parameterCount];
            if (agr0 != null)
                System.Array.Copy(agr0, 0, result, 0, agr0.Length);

            if(parameter.Length< parameterCount- agr0.Length)
            {
                Debug.LogError("参数个数异常  " + parameter.Length + "  agr0=" + agr0.Length + "  parameterCount=" + parameterCount);
            }
            else
            {
                System.Array.Copy(agr0, 0, result, agr0.Length,( parameterCount- agr0.Length));
            }
            return result;
        }
        #endregion



        #region 合并参数
        /// <summary>
        /// 合并两个参数
        /// </summary>
        /// <param name="agr0"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public UIParameterArgs MegerParameter(UIParameterArgs agr0, UIParameterArgs agr1)
        {
            List<object> parameter = new List<object>(agr0.ParemeterCount + agr1.ParemeterCount);
            parameter.AddRange(agr0.Parameter);
            parameter.AddRange(agr1.Parameter);

            return UIParameterArgs.Create(parameter.ToArray());
        }



        /// <summary>
        /// 合并两个参数
        /// </summary>
        /// <param name="agr0"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public UIParameterArgs MegerParameter(UIParameterArgs agr0, uint beginIndex0, uint count0, UIParameterArgs agr1, uint beginIndex1, uint count1)
        {
            #region  异常处理
            if (beginIndex0 >= agr0.ParemeterCount||(beginIndex0+ count0)> agr0.ParemeterCount)
            {
                throw new System.Exception(string.Format("参数越界  数据源个数={0} ,beginIndex0={1}  ,count0={2}", agr0.ParemeterCount, beginIndex0, count0));
            }
            if (beginIndex1 >= agr1.ParemeterCount || (beginIndex1 + count1) > agr1.ParemeterCount)
            {
                throw new System.Exception(string.Format("参数越界  数据源个数={0} ,beginIndex1={1},count1={2}", agr1.ParemeterCount, beginIndex1, count1));
            }

            #endregion


            List<object> parameter = new List<object>((int)(count0 + count1));
            for (int dex=(int) beginIndex0; dex< (int)beginIndex0 + count0;++dex)
            {
                parameter.Add(agr0.Parameter[dex]);
            }
            for (int dex = (int)beginIndex1; dex < (int)beginIndex1 + count1; ++dex)
            {
                parameter.Add(agr1.Parameter[dex]);
            }

            return UIParameterArgs.Create(parameter.ToArray());
        }
        #endregion

    }
}