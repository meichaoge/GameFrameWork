using GameFrameWork.UGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_As_Operater : MonoBehaviour {

    public UIBaseWidgetView m_TestView;
    // Use this for ;
    void Start () {
        Debug.Log("Test_As_Operater " + (m_TestView as UIBaseWidgetView));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
