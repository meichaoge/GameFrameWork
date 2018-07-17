using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using UnityEngine.UI;

public class Test_LoadTextAsset : MonoBehaviour
{
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
            TextAssetLoader loader = TextAssetLoader.LoadAsset(  m_Url, OnComplete);
        }
    }




    /// <summary>
    /// 测试时候会报错 字符串太长了
    /// </summary>
    /// <param name="loader"></param>
    void OnComplete(BaseAbstracResourceLoader loader)
    {
        //GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject);
        //Debug.LogInfor("" + loader.ResultObj.GetType());
        m_ShowText.text = loader.ResultObj.ToString();
    }




}
