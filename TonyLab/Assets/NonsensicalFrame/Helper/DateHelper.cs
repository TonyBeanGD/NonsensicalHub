using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalFrame
{
    public class DateHelper
    {
        private void Test()
        {
            //获取日期+时间
            DateTime.Now.ToString();                    // 2008-9-4 20:02:10
            DateTime.Now.ToLocalTime().ToString();      // 2008-9-4 20:12:12

            //获取日期
            DateTime.Now.ToLongDateString().ToString();     // 2008年9月4日
            DateTime.Now.ToShortDateString().ToString();    // 2008-9-4
            DateTime.Now.ToString("yyyy-MM-dd");            // 2008-09-04
            DateTime.Now.Date.ToString();                   // 2008-9-4 0:00:00

            //获取时间
            DateTime.Now.ToLongTimeString().ToString();     // 20:16:16
            DateTime.Now.ToShortTimeString().ToString();    // 20:16
            DateTime.Now.ToString("hh:mm:ss");              // 08:05:57
            DateTime.Now.TimeOfDay.ToString();              // 20:33:50.7187500

            //其他
            DateTime.Now.ToFileTime().ToString();       // 128650040212500000
            DateTime.Now.ToFileTimeUtc().ToString();    // 128650040772968750
            DateTime.Now.ToOADate().ToString();         // 39695.8461709606
            DateTime.Now.ToUniversalTime().ToString();  // 2008-9-4 12:19:14

            DateTime.Now.Year.ToString();           // 获取年份   2008
            DateTime.Now.Month.ToString();          // 获取月份   9
            DateTime.Now.DayOfWeek.ToString();      // 获取星期几   Thursday
            DateTime.Now.DayOfYear.ToString();      // 获取一年中的第几天   248
            DateTime.Now.Hour.ToString();           // 获取小时   20
            DateTime.Now.Minute.ToString();         // 获取分钟   31
            DateTime.Now.Second.ToString();         // 获取秒数   45


            DateTime dt = DateTime.Now;
            int n = 3;
            //n为一个数,可以数整数,也可以事小数
            dt.AddYears(n).ToString();      //时间加n年
            dt.AddDays(n).ToString();       //加n天
            dt.AddHours(n).ToString();      //加n小时
            dt.AddMonths(n).ToString();     //加n个月
            dt.AddSeconds(n).ToString();    //加n秒
            dt.AddMinutes(n).ToString();    //加n分

        }

        /// <summary>
        /// 获取当前日期的字符串，以下划线隔开
        /// </summary>
        /// <returns>当前日期的字符串</returns>
        internal static string GetDateString()
        {
            return DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day;
        }

    }

}

