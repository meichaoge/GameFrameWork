using GameFrameWork;
using GameFrameWork.ResourcesLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LoadMaterial : MonoBehaviour
{
    public LoadAssetModel m_LoadAssetModel = LoadAssetModel.Async;
    public GameObject m_Go;
    public string m_URL;
    // Use this for initialization
    void Start()
    {
        Debug.Log("   m_Go.GetComponent<Renderer>()=" + m_Go.GetComponent<Renderer>().material.name);
#if UNITY_EDITOR
        //***测试获取材质球属性字段
        GameObjectRenderPropertyInfor propertyInfor = ShaderUtility.Instance.GetMaterialShaderProperty(m_Go.GetComponent<Renderer>());
        Debug.Log(propertyInfor);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
    //        ResourcesMgr.Instance.LoadMaterial(m_URL, m_Go.transform, m_LoadAssetModel, OnComplete);
        }
    }

    void OnComplete(Material mat)
    {
        Debug.LogInfor("Time=" + Time.realtimeSinceStartup + "Frame=" + EventCenter.Instance.CurFrameCount);
        m_Go.GetComponent<Renderer>().material = mat;
    }



}
