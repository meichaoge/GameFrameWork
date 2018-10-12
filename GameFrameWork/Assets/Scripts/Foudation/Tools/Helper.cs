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
        public object[] MegerParameter(object agr0, params object[] parameter)
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
        public object[] MegerParameter(object[] agr0, params object[] parameter)
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
        #endregion


    }
}