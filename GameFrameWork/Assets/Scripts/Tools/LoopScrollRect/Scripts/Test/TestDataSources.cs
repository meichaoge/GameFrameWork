using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
public class TestDataSources : MonoBehaviour {
    public static TestDataSources Instance;

    public List<int> m_DataTest = new List<int>();


    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
		for (int dex=0;dex< InitOnStart.Instance.m_DataCount;++dex)
		{
            m_DataTest.Add(dex);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public int  GetDataById(int id)
    {
        return m_DataTest[id];
    }






}
#endif
