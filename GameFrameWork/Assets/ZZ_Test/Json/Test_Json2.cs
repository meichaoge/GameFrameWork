using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Json2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var test01 = JsonMapper.ToJson(1);
        var test02 = JsonMapper.ToJson(test01);
        var test03= JsonMapper.ToJson(test02);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
