using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.Helper
{
    public static class UIHelper
    {
        public static void DelayTopping(Scrollbar scrollbar)
        {
            NonsensicalUnityInstance.Instance.DelayDoIt(0,()=> { scrollbar.value = 1; });
        }
    }
}