using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork;
using GameFrameWork.ResourcesLoader;

public class Test_LoadSprite : MonoBehaviour
{
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
            SpriteLoader loader = SpriteLoader.LoadAsset(m_Image.transform, m_Url, OnComplete);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResourcesMgr.Instance.LoadSprite(m_Image, m_Url, null);
        }
    }


    void OnComplete(BaseAbstracResourceLoader loader)
    {
        //GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject);
        //Debug.LogInfor("" + loader.ResultObj.GetType());
        m_Image.sprite = loader.ResultObj as Sprite;
    }




}
