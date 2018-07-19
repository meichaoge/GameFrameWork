using GameFrameWork;
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
            PrefabLoader loader = PrefabLoader.LoadAsset(gameObject, m_Url, OnComplete);
        }
    }


    void OnComplete(BaseAbstracResourceLoader loader)
    {
        //GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject);
        //Debug.LogInfor("" + loader.ResultObj.GetType());
        GameObject go = GameObject.Instantiate<GameObject>(loader.ResultObj as GameObject,transform);
    }



}
