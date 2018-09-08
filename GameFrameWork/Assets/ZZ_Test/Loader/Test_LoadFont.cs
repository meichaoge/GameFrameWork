using GameFrameWork;
using GameFrameWork.ResourcesLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_LoadFont : MonoBehaviour
{
    public Text m_ShowText;
    public string m_Url;
    public LoadAssetModel m_LoadAssetModel = LoadAssetModel.Async;
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
            ResourcesMgr.Instance.LoadFont(m_Url, m_LoadAssetModel, OnComplete);
        }
    }

    void OnComplete(Font font)
    {
        Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
        m_ShowText.font = font;
    }
}
