using GameFrameWork;
using GameFrameWork.ResourcesLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoadAssetBundle : MonoBehaviour {
    public LoadAssetModel m_LoadAssetModel = LoadAssetModel.Async;
    public string URL;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            AssetBundleLoader.LoadAssetBundleAsset(URL, System.IO.Path.GetFileNameWithoutExtension(URL), m_LoadAssetModel, CompleteLoad,true);
        }
	}




    private void CompleteLoad(BaseAbstracResourceLoader bunleLoader)
    {
        //GameObject go = bundle.LoadAsset<GameObject>("TestObj");
        GameObject.Instantiate<GameObject>(bunleLoader.ResultObj as GameObject);
        Debug.Log("CompleteLoad");

    }



}
