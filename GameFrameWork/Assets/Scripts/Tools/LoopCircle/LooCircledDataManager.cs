using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LooCircledDataManager
{
    private static LooCircledDataManager _instance;
    public static LooCircledDataManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new LooCircledDataManager();
            return _instance;
        }
    }

    private List<int> Datas = new List<int>();

    public void RefillDate(int count)
    {
        Datas.Clear();
        for (int dex = 0; dex < count; dex++)
        {
            Datas.Add(dex);
        }
    }

    public void GetDateByIndex()
    {

    }

}
