using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 所有配置层接口
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// 标识当前是否可用(有可能读取出错)
        /// </summary>
        bool IsEnable { get; }

        void InitialSetting();

    }
}