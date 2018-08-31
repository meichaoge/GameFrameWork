using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_IO : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log(System.IO.Path.GetDirectoryName(Application.dataPath));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
