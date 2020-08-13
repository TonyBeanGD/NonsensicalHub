using System.IO;
using UnityEditor;
using UnityEngine;

namespace TonyBeanTools
{
    public class PlayerPrefsModifier
    {
        [MenuItem("TBTools/PlayerPrefs修改器/清空PlayerPrefs存档")]
        static void CleanSavedate()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
