using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameFrameWork;


[System.Flags]
public enum  FlagTestEnum{
    One=0,
    Two=1,
    Three=2,

}



public class TestEnumFlag : MonoBehaviour {

    [Enum_FlagsAttribute("Test Menu")]
    public FlagTestEnum tt;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
