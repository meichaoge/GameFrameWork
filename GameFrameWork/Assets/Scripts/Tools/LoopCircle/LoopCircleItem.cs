using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoopCircleItem : MonoBehaviour
{
    public int Dex;
    private Text ItemTxt;
    public bool IsVisible;
    void Awake()
    {
        ItemTxt = transform.GetChild(0).GetComponent<Text>();

    }

    public void InitialedView(int dex,bool isVisible)
    {
        Dex = dex;
        IsVisible = isVisible;
        ItemTxt.text = dex.ToString();
    }
}
