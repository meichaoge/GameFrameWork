using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShader : MonoBehaviour
{
    public string m_Url;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShaderLoader.LoadAsset(m_Url, CompleteLoadHandler, LoadAssetPathEnum.ResourcesPath);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnApplicationQuit();
        }
    }

    private void CompleteLoadHandler(bool isError, Shader data)
    {
        if (isError == false)
        {
            Debug.Log("Success");
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
            ShaderLoader.UnLoadAsset(url);
    }
#endif
}
