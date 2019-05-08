﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestHorizontialLoopCicle : MonoBehaviour {

    public int DataCount = 10;
    public HorizontialLoopCircle mHorizontialLoopCircle;
    void Start()
    {
        mHorizontialLoopCircle.OnItemCreateAct += OnCreateItem;
        mHorizontialLoopCircle.OnItemShowAct += OnShowItem;
        mHorizontialLoopCircle.RefillData(DataCount);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mHorizontialLoopCircle.RefillData(DataCount);
        }
    }


    private void OnCreateItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetAddComponent<TestLoopCircleItem>();
    }

    private void OnShowItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetComponent<TestLoopCircleItem>().Show(null);
    }
}
