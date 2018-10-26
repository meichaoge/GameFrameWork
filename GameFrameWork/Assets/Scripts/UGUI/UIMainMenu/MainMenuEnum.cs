using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UGUI
{

    /// <summary>
    /// 主菜单显示的
    /// </summary>
    [System.Serializable]
    public enum MainMenuEnum
    {
        None = -1,  //默认不可用的值
        //***底部显示
        God = 0,  //英雄
        Union,  //工会
        Knapsack,  //背包
        Task,  //任务

        Mall = 49,  //商城

        //***左侧显示
        Friend = 50, //好友
        Mail,  //邮件
        Rank,  //排行榜


        //右上显示
        TimeLimitActivity = 100,//限时
        Welfare,//福利
        Activity,  //活动
        FirstRecharge,  //首冲
        NewServerActivity,//开服活动
        Notice,//公告

        //**右上角第二排显示的菜单
        WorldBoss = 200, //世界Boss 
        AreanAward,  //苍穹竞技场



    }
}