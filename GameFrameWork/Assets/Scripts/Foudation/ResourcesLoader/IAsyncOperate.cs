using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 异步加载资源操作接口
    /// </summary>
    public interface IAsyncOperate 
    {
        /// <summary>
        /// 标识当前加载任务是否完成
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 当前加载任务是否已经出错(当IsCompleted&&IsError==false 时候才表示加载资源正确且完成)
        /// </summary>
        bool IsError { get; }
        /// <summary>
        /// 当前加载的进度(0~1)
        /// </summary>
        float Process { get; }

        /// <summary>
        /// 加载过程描述(一般为显示当前的进度)
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 最后加载出来的对象
        /// </summary>
        object ResultObj { get; }
    }
}