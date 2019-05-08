//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Text;

//namespace GameFrameWork
//{
//    /// <summary>
//    /// 使用Unity PlayerPrefs  存储的数据
//    /// </summary>
//   [System.Serializable]
//    public class LocalStoreInfor
//    {
//        public string Key;
//        public string LastRecordTime { get; private set; }
//        public string StoreInformation { get; private set; } //存储的信息

//        public LocalStoreInfor() { }

//        public LocalStoreInfor(string key,string date)
//        {
//            Key = key;
//            LastRecordTime = System.DateTime.Now.ToString("F");
//            StoreInformation = string.Format(date);
//        }

//        /// <summary>
//        /// 更新本地记录
//        /// </summary>
//        /// <param name="date"></param>
//        public void UpdateRecord(string date)
//        {
//            LastRecordTime = System.DateTime.Now.ToString("F");
//            StoreInformation = date;
//        }

       

//    }



//}