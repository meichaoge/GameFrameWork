using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
   /// <summary>
   /// 数学库的扩展
   /// </summary>
    public class MathUtility : Singleton_Static<MathUtility>
    {
        #region  获取幂次方

        /// <summary>
        /// 获取制定参数的最高2次幂 2^x<=data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetPowerNumber(int data, int maxPower = 11)
        {
            int temple = 1;
            for (int dex = 1; dex < maxPower; ++dex)
            {
                temple *= 2;
                if (temple == data)
                    return dex;
                if (temple > data)
                    return dex - 1;
            }
            Debug.Log("超过最大的大小  返回默认最大值 " + maxPower);
            return maxPower;
        }

        /// <summary>
        /// 获取两个参数中最大的数据的2 次幂
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="maxPower"></param>
        /// <returns></returns>
        public int GetPowerNumber(int width, int height, int maxPower = 10)
        {
            return GetPowerNumber(Mathf.Max(width, height), maxPower);
        }

        #endregion

        #region 获取最接近的 2 次幂
        /// <summary>
        /// 获取最接近的2次幂的值 (最接近data 的2次幂)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetNearnestPowerNumber(int data, int maxPower = 11)
        {
            int temple1 = 1;
            int temple2 = 1;

            for (int dex = 1; dex < maxPower; ++dex)
            {
                temple2 = temple1 * 2;
                if (temple2 >= data && temple1 < data)
                {
                    if (temple2 - data <= data - temple1)
                        return temple2;
                    return temple1;
                }//说明结果在 temple2 与temple1 之间
                temple1 *= 2;
            }
            temple1 *= 2;
            Debug.Log(string.Format("超过最大的大小  返回默认最大值maxPower ={0} 2^ maxPower= {1}", maxPower, temple1));
            return temple1;
        }



        /// <summary>
        /// 获取最接近的2次幂的值小于等于
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetNearnestMinPowerNumber(int data, int maxPower = 11)
        {
            int temple1 = 1;
            int temple2 = 1;

            for (int dex = 1; dex < maxPower; ++dex)
            {
                temple2 = temple1 * 2;
                if (temple2 >= data && temple1 < data)
                {
                    return temple1;
                }//说明结果temple1 
                temple1 *= 2;
            }
            Debug.Log("超过最大的大小  返回默认最大值 2^maxPower= " + temple1);
            return temple1;
        }


        #endregion


    }
}