using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_LoadFont : MonoBehaviour {
    public Text m_ShowText;
    public string m_Url;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResourcesMgr.Instance.LoadFont(m_Url, (result) => { m_ShowText.font= result; });
        }
    }
}
