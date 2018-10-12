using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class TestClass {
    public int m_TestValue;

    public TestClass(int te)
    {
        m_TestValue = te;
    }

}



public class Test_Json : MonoBehaviour
{
    HashSet<string> testHashSet = new HashSet<string>();
    HashSet<int> testHashSet2 = new HashSet<int>();
    Dictionary<string, int> testDic = new Dictionary<string, int>();

    Dictionary<int, TestClass> m_TestDic = new Dictionary<int, TestClass>();
    Dictionary<int, int> m_Test2Dic = new Dictionary<int, int>();

    // Use this for initialization
    void Start()
    {
        //testHashSet.Add("1");
        //testHashSet.Add("2");
        //testHashSet2.Add(1);
        //testHashSet2.Add(2);
        //testDic.Add("1", 1);
        //testDic.Add("2", 1);
        //var value1 = JsonMapper.ToJson(testHashSet);
        //var value3 = JsonMapper.ToJson(testHashSet2);
        //var value2 = JsonMapper.ToJson(testDic);


        m_TestDic.Add(1, new TestClass(1));
        m_Test2Dic.Add(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TestClass test001 = m_TestDic[1];
            test001.m_TestValue = 2;
            Debug.Log(m_TestDic[1].m_TestValue);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            int test002 = m_Test2Dic[1];
            test002 = 2;
            Debug.Log(m_Test2Dic[1]);
        }
    }
}
