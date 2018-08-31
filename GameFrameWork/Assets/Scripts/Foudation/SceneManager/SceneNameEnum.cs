using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 提供 SceneNameEnum 常用的扩展使用方法
/// </summary>
public static class SceneNameHelper
{
    /// <summary>
    /// 通过场景名称获取对应的枚举类型  
    /// </summary>
    /// <param name="sceneName">错误的场景名称返回 SceneNameEnum.None 并报错</param>
    /// <returns></returns>
    public static SceneNameEnum GetSceneName(string sceneName)
    {
        object result = System.Enum.Parse(typeof(SceneNameEnum), sceneName);
        if(result==null)
        {
            Debug.LogError("GetSceneName Fail,Not Avaliable SceneName " + sceneName);
            return SceneNameEnum.None;
        }
        return (SceneNameEnum)result;
    }



}







/// <summary>
/// 所有的场景名都需要在这里定义 
/// </summary>
public enum SceneNameEnum
{
    None=-1,  //无定义的
    ApplicationEntry,
    StartUp,
    Test_Scene,                //测试场景

}
