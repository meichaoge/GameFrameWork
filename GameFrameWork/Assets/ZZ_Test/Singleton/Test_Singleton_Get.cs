using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Singleton_Get : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log(Test_Singleton.Instance);
       Debug.Log(Test_Singleton1.Instance);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
