using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork;
using GameFrameWork.ResourcesLoader;

public class Test_LoadSprite : MonoBehaviour
{
    public LoadAssetModel m_LoadAssetModel = LoadAssetModel.Async;
    public Image m_Image;
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
            Debug.LogInfor("Time=" + Time.realtimeSinceStartup+"Frame="+EventCenter.Instance.CurFrameCount);
            SpriteLoader loader = SpriteLoader.LoadAsset(m_Image.transform, m_Url, m_LoadAssetModel, OnComplete);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResourcesMgr.Instance.LoadSprite(m_Url, m_Image, m_LoadAssetModel, null);
        }
    }


    void OnComplete(BaseAbstracResourceLoader loader)
    {
        Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
        //GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject);
        //Debug.LogInfor("" + loader.ResultObj.GetType());
        m_Image.sprite = loader.ResultObj as Sprite;
    }




}
