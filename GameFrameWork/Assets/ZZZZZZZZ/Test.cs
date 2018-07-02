﻿using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string url;
    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ByteLoader.LoadAsset(url, CompleteLoadHandler, LoadAssetModel.Async);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnApplicationQuit();
        }
    }


    private void CompleteLoadHandler(bool isError, byte[] data)
    {
        if (isError == false)
        {
            Debug.Log(Encoding.UTF8.GetString(data));
        }
    }



#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        ByteLoader.UnLoadAsset(url);
    }
# else
    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
            ByteLoader.UnLoadAsset(url);
    }
#endif

}
