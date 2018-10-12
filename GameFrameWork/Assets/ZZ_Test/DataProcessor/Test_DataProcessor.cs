using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_DataProcessor : MonoBehaviour
{
    public string m_SourcesData;
    public string m_EncrypData;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_EncrypData = DataProcessor.Instance.EncryptData(m_SourcesData);
        }

        if (Input.GetKeyDown(KeyCode.B ))
        {
            Debug.Log(DataProcessor.Instance.DecryptData(m_EncrypData));
        }
    }
}
