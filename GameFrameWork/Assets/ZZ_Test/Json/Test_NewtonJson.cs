//using Newtonsoft.Json;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Test_NewtonJson : MonoBehaviour {

//    HashSet<string> testHashSet = new HashSet<string>();
//    HashSet<int> testHashSet2 = new HashSet<int>();
//    Dictionary<string, int> testDic = new Dictionary<string, int>();

//    Dictionary<int, TestClass> m_TestDic = new Dictionary<int, TestClass>();
//    Dictionary<int, int> m_Test2Dic = new Dictionary<int, int>();
//    // Use this for initialization



//    void Start () {

//        testHashSet.Add("1");
//        testHashSet.Add("2");
//        testHashSet2.Add(1);
//        testHashSet2.Add(2);
//        testDic.Add("1", 1);
//        testDic.Add("2", 1);
//        var value1 = JsonConvert.SerializeObject(testHashSet);
//        var value2 = JsonConvert.SerializeObject(testDic);  //序列化
//        //var value3 = JsonConvert.SerializeObject(testDic);

//        Debug.Log("value1=  " + value1);
//      //  Debug.Log("value2=  " + value2);
//        //Debug.Log("value3=  " + value3);
//        var test001 = JsonConvert.DeserializeObject<HashSet<string>>(value1);
//        var test002 = JsonConvert.DeserializeObject<Dictionary<string, int>>(value2);  //反序列化


//        m_TestDic.Add(1, new TestClass(1));
//        m_Test2Dic.Add(1, 1);




//    }
	
//	// Update is called once per frame
//	void Update () {
//	}
//}
