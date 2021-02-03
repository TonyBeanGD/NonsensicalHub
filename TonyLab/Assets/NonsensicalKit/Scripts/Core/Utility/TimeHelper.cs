using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit.Utility
{
    public static class TimeHelper
    {
        public static string ToHMS(int time)
        {
            int hour = time / 3600;
            int minute = (time - hour * 3600) / 60;
            int second = time % 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }

        public static string ToMS(int time)
        {
            int minute = time / 60;
            int second = time % 60;
            return string.Format("{0:D2}:{1:D2}", minute, second);
        }
    }
}