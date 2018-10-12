using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Test_Json3 : MonoBehaviour {
    public class Test {
        public long m_Test;
    }

	// Use this for initialization
	void Start () {
        Test test001 = new Test();
        test001.m_Test = 1539055198;

      var data=  JsonMapper.ToJson(test001);
        var data2 = JsonMapper.ToObject<Test>(data);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
