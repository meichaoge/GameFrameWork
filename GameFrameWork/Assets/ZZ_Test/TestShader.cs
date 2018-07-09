using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShader : MonoBehaviour
{
    public string m_Url;
    public GameObject go;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShaderLoader.LoadAsset(m_Url, CompleteLoadHandler);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
     //       OnApplicationQuit();
        }
    }

    private void CompleteLoadHandler(BaseAbstracResourceLoader loader)
    {
        ShaderLoader shaderLoader = loader as ShaderLoader;
        if (shaderLoader.IsError == false)
        {
            go.GetComponent<MeshRenderer>().material.shader = shaderLoader.ResultObj as Shader;
        }
    }



#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
       ShaderLoader.UnLoadAsset(m_Url);
    }
# else
    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
            ShaderLoader.UnLoadAsset(m_Url);
    }
#endif
}
