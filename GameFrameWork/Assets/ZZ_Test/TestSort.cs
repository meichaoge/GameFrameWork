using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSort : MonoBehaviour {
    public List<int> m_Data = new List<int>();
	// Use this for initialization
	void Start () {
        m_Data.Sort((a,b)=> {
            if (a > b) return 1;
            else if (a==b)
            {
                return 0;
            }
            else
            {

                return -1;
            }


        });

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
