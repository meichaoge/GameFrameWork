using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using UnityEngine.UI;
using GameFrameWork.ResourcesLoader;

public class Test_LoadTextAsset : MonoBehaviour
{
    public LoadAssetModel m_LoadAssetModel = LoadAssetModel.Async;
    public Text m_ShowText;
    public string m_Url;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
            TextAssetLoader loader = TextAssetLoader.LoadAsset(  m_Url, m_LoadAssetModel, OnComplete);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResourcesMgr.Instance.LoadFile(m_Url, m_LoadAssetModel,(result) => { m_ShowText.text = result; });
        }
    }




    /// <summary>
    /// 测试时候会报错 字符串太长了
    /// </summary>
    /// <param name="loader"></param>
    void OnComplete(BaseAbstracResourceLoader loader)
    {
        Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
        //GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject);
        //Debug.LogInfor("" + loader.ResultObj.GetType());
        m_ShowText.text = loader.ResultObj.ToString();
    }




}
