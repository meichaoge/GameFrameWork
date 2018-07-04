using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResourceLoadShader : MonoBehaviour
{
    public GameObject go;
    public string m_Shadeurl;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
          Shader sha=  Resources.Load(m_Shadeurl) as Shader ;
            go.GetComponent<MeshRenderer>().material.shader = sha;
        }
    }
}
