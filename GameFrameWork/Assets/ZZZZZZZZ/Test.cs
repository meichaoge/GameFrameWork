using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test : MonoBehaviour {
    public string url;
	// Use this for initialization
	void Start () {
        ResourcesLoaderMgr.CreateLoader<ByteLoader>( OnCompleteLoad);
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCompleteLoad()
    {
        Debug.Log("OnCompleteLoad ..");
        ByteLoader.LoadAsset<ByteLoader>(url, CompleteLoadHandler, LoadAssetModel.Async, LoadAssetPathEnum.ResourcesPath);
    }

    private void  CompleteLoadHandler(bool isError, byte[] data)
    {
        if(isError==false)
        {
            Debug.Log(Encoding.UTF8.GetString(data));
        }
    }

}
