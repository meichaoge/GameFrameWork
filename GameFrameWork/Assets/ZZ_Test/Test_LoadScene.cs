using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LoadScene : MonoBehaviour
{
    public SceneNameEnum m_WillLoad = SceneNameEnum.None;
    public LoadSceneModeEnum m_LoadSceneEnum = LoadSceneModeEnum.ReleaseSelf;
    public KeyCode m_Key = KeyCode.F;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(m_Key) && AppSceneManager.Instance.IsUnLoadingScene == false)
        {
            AppSceneManager.Instance.LoadScene(m_WillLoad, m_LoadSceneEnum, (isComplete) =>
            {
                if (isComplete)
                {
                    Debug.LogInfor("加载其他的场景..Go!!!");
                }
            },
           () =>
           {
               Debug.LogInfor("卸载其他场景完成");
           }
           );
        }
    }
}
