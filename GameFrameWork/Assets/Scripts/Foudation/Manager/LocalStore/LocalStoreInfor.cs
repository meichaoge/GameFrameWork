using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 使用Unity PlayerPrefs  存储的数据
    /// </summary>
   [System.Serializable]
    public class LocalStoreInfor
    {
        public string Key;
        public string LastRecordTime;
        public object DataValue;  //最新的值
        public List<StoreInforRecord> AllRecordInfors = new List<StoreInforRecord>();
        private const int S_MaxRecordInforCount = 5;  //最大的更新记录个数

        public LocalStoreInfor(string key,string date, StoreInforStateEnum state)
        {
            Key = key;
            LastRecordTime = System.DateTime.Now.ToString("F");
            DataValue = date;

            StoreInforRecord record = new StoreInforRecord();
            record.StoreState = state;
            record.StoreTime = LastRecordTime;
            record.StoreData = date;
            AllRecordInfors.Add(record);
          
        }

        /// <summary>
        /// 更新本地记录
        /// </summary>
        /// <param name="date"></param>
        public void UpdateRecord(string date)
        {
            LastRecordTime = System.DateTime.Now.ToString("F");
            DataValue = date;

            StoreInforRecord record = new StoreInforRecord();
            record.StoreState = StoreInforStateEnum.Update;
            record.StoreTime = LastRecordTime;
            record.StoreData = date;
            AllRecordInfors.Add(record);

            if (AllRecordInfors.Count > S_MaxRecordInforCount)
                AllRecordInfors.RemoveAt(0);
        }

    }

    /// <summary>
    /// 记录每一次操作改变的值
    /// </summary>
    [System.Serializable]
    public class StoreInforRecord
    {
        public StoreInforStateEnum StoreState = StoreInforStateEnum.Add;
        public string StoreTime;
        public string StoreData;
    }

    /// <summary>
    /// 本地化持续数据的状态
    /// </summary>
    public enum StoreInforStateEnum
    {
        Add,
       // Delete,
        Update
    }

}