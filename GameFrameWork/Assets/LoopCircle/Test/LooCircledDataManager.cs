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

    public List<int> Datas = new List<int>();

    public void RefillDate(int count)
    {
        Datas.Clear();
        for (int dex = 0; dex < count; dex++)
        {
            Datas.Add(dex);
        }
    }

    public int GetDateByIndex(int dataIndex)
    {
        int realIndex = GetRealIndex(dataIndex);
        if (realIndex < 0)
            return 0;
        return Datas[realIndex];
    }

    private int GetRealIndex(int dataIndex)
    {
        if (Datas.Count == 0) return -1;
        int realIndex = dataIndex % Datas.Count;
        if (realIndex < 0)
            realIndex += Datas.Count;
        return realIndex;
    }

    public void DeleteDataByIndex(int dataIndex)
    {
        int realIndex = GetRealIndex(dataIndex);
        if (realIndex < 0)
            return ;

        Datas.RemoveAt(realIndex);
    }

}
