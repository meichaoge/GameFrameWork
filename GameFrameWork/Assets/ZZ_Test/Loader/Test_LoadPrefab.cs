using GameFrameWork;
using GameFrameWork.ResourcesLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LoadPrefab : MonoBehaviour {
    public string m_Url;
    public LoadAssetModel m_LoadAssetModel = LoadAssetModel.Async;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
            PrefabLoader loader = PrefabLoader.LoadAsset(transform, m_Url, m_LoadAssetModel, OnComplete);
            //loader.ReduceReference(loader, false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {

            if(m_LoadAssetModel== LoadAssetModel.Async)
            {
                ResourcesMgr.Instance.InstantiateAsync(m_Url, transform, (obj) =>
                {
                    Debug.Log("生成成功");
                }, true);
            }
            else
            {
                GameObject go = ResourcesMgr.Instance.InstantiateSync(m_Url, transform);
            }
        }
    }


    void OnComplete(BaseAbstracResourceLoader loader)
    {
        Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
        //GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject);
        //Debug.LogInfor("" + loader.ResultObj.GetType());
        GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject,transform);
    }



}
