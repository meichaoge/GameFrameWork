using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 用于编辑器下创建循环列表
/// </summary>
public class LoopCricleViewCreater
{

    #region Menu

    [MenuItem("GameObject/LoopCircle/HorizontialLoopCircle", false)]
    private static void CreateHorizontialLoopCircle()
    {
        CreateLoopCicleView("HorizontialLoopCircle");
    }

    [MenuItem("GameObject/LoopCircle/VerticalLoopCircle", false)]
    private static void CreateVerticalLoopCircle()
    {
        CreateLoopCicleView("VerticalLoopCircle");

    }

    [MenuItem("GameObject/LoopCircle/HorizontialLoopCircleTween", false)]
    private static void CreateHorizontialLoopCircleTween()
    {
        CreateLoopCicleView("HorizontialLoopCircleTween");

    }
    [MenuItem("GameObject/LoopCircle/VerticalLoopCircleTween", false)]
    private static void CreateVerticalLoopCircleTween()
    {
        CreateLoopCicleView("VerticalLoopCircleTween");

    }

    [MenuItem("GameObject/LoopCircle/HorizontialLoopCircleFixDistance", false)]
    private static void CreateHorizontialLoopCircleFixDistance()
    {
        CreateLoopCicleView("HorizontialLoopCircleFixDistance");

    }

    [MenuItem("GameObject/LoopCircle/VerticalHalfLoopCircleFixdistance", false)]
    private static void CreateVerticalHalfLoopCircleFixdistance()
    {
        CreateLoopCicleView("VerticalHalfLoopCircleFixdistance");

    }


    #endregion


    private static void CreateLoopCicleView(string prefabName)
    {
        Transform parent = GetViewParent();
        string prefabsPath = string.Format("Assets/Editor/LoopCircleView/{0}.prefab", prefabName);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath);
        if (prefab == null)
        {
            Debug.LogError("Load Editor Asset Fail  " + prefabsPath);
            return;
        }
        GameObject go = GameObject.Instantiate<GameObject>(prefab, parent);
        go.name = prefab.name;
    }


    private static Transform GetViewParent()
    {
        //Debug.Log(Selection.activeGameObject);
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null || selectedObj.GetComponentInParent<Canvas>() == null)
        {
            Canvas canvas = Transform.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject go = new GameObject("LoopCicleCanvas");
                canvas = go.GetAddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                go.GetAddComponent<GraphicRaycaster>();
            }
            return canvas.transform;
        }
        return selectedObj.transform;
    }


}
