using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonsensicalKit.Editor
{
    [InitializeOnLoad]
    class SceneJump
    {
        static SceneJump()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChange;
        }

        static void PlayModeStateChange(PlayModeStateChange playModeStateChange)
        {
            bool needJump = PlayerPrefs.GetInt("tb_nonsensicalConfigurator_jumpFirstOnPlay", 0)==0?false:true;
            if (needJump&& playModeStateChange == UnityEditor.PlayModeStateChange.EnteredPlayMode)
            {
                if (SceneManager.GetActiveScene().buildIndex != 0)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }
}