using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 处理常见的时间/时间格式转换
    /// </summary>
    public class TimeHelper : Singleton_Static<TimeHelper>
    {
        private const string S_TimeFormationConfigName = "TimeHelper";//日期格式配置文件

        /// <summary>  
        /// 时间戳转换为本地时间对象  
        /// </summary>  
        /// <returns></returns>        
        public DateTime GetDateTimeBySecend(long unix)
        {
            //long unix = 1500863191;  
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime newTime = dtStart.AddSeconds(unix);
            return newTime;
        }

        /// <summary>
        /// DataTime 转换成时间戳Timestamp(秒)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public long DateTime2Second(DateTime time)
        {
            DateTime dateStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0)); //根据时区计算当前时间
            int timeStamp = Convert.ToInt32((time - dateStart).TotalSeconds);
            return timeStamp;
        }

        /// <summary>
        /// 当前系统时间转成秒数
        /// </summary>
        /// <returns></returns>
        public long DateTime2Second_Now()
        {
            DateTime dt = System.DateTime.Now;  //当前系统时间
            return DateTime2Second(dt);
        }


        #region  时间格式转换


        /// <summary>
        /// 根据秒数获取显示的时间  (秒 )
        /// </summary>
        /// <param name="time"></param
        /// <param name="antoComplement">当不足10秒时候左边自动补齐0</param
        /// <returns></returns>
        public string GetShowTimeFromSecond(int time, bool antoComplement)
        {
            if (time < 0)
            {
                if (antoComplement)
                    return "00";
                return "0";
            }

            int second = time % 60;
            if (second < 10 && antoComplement)
                return "0" + second;
            return second.ToString();
        }

        /// <summary>
        /// 根据秒数获取显示的时间(分：秒) 04：23  如果超过一个小时 则显示01:22:09  
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetShowTimeFromSecond_Second(int time)
        {
            if (time >= 3600)
                return GetShowTimeFromSecond_Hour(time);  //超过一个小时

            int mins = time / 60;
            int seconds = time % 60;
            string timer_string = "";
            if (mins < 10)
                timer_string += "0" + mins.ToString() + ":";
            else
                timer_string += mins.ToString() + ":";


            if (seconds < 10)
                timer_string += "0" + seconds.ToString();
            else
                timer_string += seconds.ToString();
            return timer_string;
        }

        /// <summary>
        /// 根据秒数获取显示的时间(时：分：秒) 01:04：23
        /// </summary>
        /// <param name="time">秒</param>
        /// <returns></returns>
        public string GetShowTimeFromSecond_Hour(int time)
        {
            int hours = time / 3600;  //小时
            int mins = (time - hours * 3600) / 60;//分
            int seconds = time - hours * 3600 - mins * 60;  //秒

            string timer_string = "";
            if (hours < 10)
                timer_string += "0" + hours.ToString() + ":";
            else
                timer_string += hours.ToString() + ":";


            if (mins < 10)
                timer_string += "0" + mins.ToString() + ":";
            else
                timer_string += mins.ToString() + ":";


            if (seconds < 10)
                timer_string += "0" + seconds.ToString();
            else
                timer_string += seconds.ToString();
            return timer_string;
        }

        /// <summary>
        /// 对给定的时间参数(秒)格式化输出成｛0｝天/小时/分钟、等
        /// </summary>
        /// <param name="time">秒数时间</param>
        /// <param name="isBefore">=true 标识显示xxx前   =false显示XXX后</param>
        /// <param name="secondFomat">对秒数的格式化 =0标识小于1分钟显示1分钟，=1标识显示真实的秒数，其他显示刚刚</param>
        /// <param name="isOnlyNum">标识是否只显示为{0}天，{0}分钟，{0}秒</param>
        /// <returns></returns>
        public string ShowTimeFormat(int time, bool isBefore, int secondFomat, bool isOnlyNum = false)
        {
            if (time <= 0) return "";

            int showTimeParameter = time / (60 * 60 * 24);
            if (showTimeParameter > 0)
            {
                if (isBefore)
                    return string.Format(GetTimeFormation("TimeDay01"), showTimeParameter);  //{0}天前
                else if (isOnlyNum)
                    return string.Format(GetTimeFormation("TimeDay"), showTimeParameter);  //{0}天
                else
                    return string.Format(GetTimeFormation("TimeDay02"), showTimeParameter);  //{0}天后
            } //天

            showTimeParameter = time / (60 * 60);
            if (showTimeParameter > 0)
            {
                if (isBefore)
                    return string.Format(GetTimeFormation("TimeHour01"), showTimeParameter);  //{0}小时前
                else if (isOnlyNum)
                    string.Format(GetTimeFormation("TimeHour"), showTimeParameter);  //{0}小时
                else
                    return string.Format(GetTimeFormation("TimeHour02"), showTimeParameter);  //{0}小时后
            }//小时

            showTimeParameter = time / 60;
            if (showTimeParameter > 0)
            {
                if (isBefore)
                    return string.Format(GetTimeFormation("TimeMinute01"), showTimeParameter);  //{0}分钟前
                else if (isOnlyNum)
                    return string.Format(GetTimeFormation("TimeMinute"), showTimeParameter);  //{0}分钟
                else
                    return string.Format(GetTimeFormation("TimeMinute02"), showTimeParameter);  //{0}分钟后
            }

            if (secondFomat == 0)
            {
                if (isBefore)
                    return string.Format(GetTimeFormation("TimeMinute01"), 1);  //{0}分钟前
                else if (isOnlyNum)
                    return string.Format(GetTimeFormation("TimeMinute"), showTimeParameter);  //{0}分钟
                else
                    return string.Format(GetTimeFormation("TimeMinute02"), 1);  //{0}分钟后
            }  //不足一分钟显示1分钟
            else if (secondFomat == 1)
            {
                if (isBefore)
                    return string.Format(GetTimeFormation("TimeSecend01"), secondFomat);  //{0}秒前
                else if (isOnlyNum)
                    return string.Format(GetTimeFormation("TimeSecend"), showTimeParameter);  //{0}秒
                else
                    return string.Format(GetTimeFormation("TimeSecend02"), secondFomat);  //{0}秒后
            } //显示多少秒
            else
            {
                return GetTimeFormation("TimeJustNow");  //刚刚
            }//显示刚刚

        }

        /// <summary>
        /// 获得配置文件中的格式
        /// </summary>
        private string GetTimeFormation(string key)
        {
            return UILanguageMgr.Instance.GetUIDynamicStrConfig(S_TimeFormationConfigName, key);
        }

        #endregion










    }
}