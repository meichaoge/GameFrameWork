using GameFrameWork;
using GameFrameWork.ResourcesLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LoadPrefab : MonoBehaviour {
    public string m_Url;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PrefabLoader loader = PrefabLoader.LoadAsset(transform, m_Url, OnComplete);
            loader.ReduceReference(loader, false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResourcesMgr.Instance.Instantiate(m_Url, transform, (obj) => {
                Debug.Log("生成成功");

            }, true);
        }
    }


    void OnComplete(BaseAbstracResourceLoader loader)
    {
        //GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject);
        //Debug.LogInfor("" + loader.ResultObj.GetType());
        GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject,transform);
    }



}
