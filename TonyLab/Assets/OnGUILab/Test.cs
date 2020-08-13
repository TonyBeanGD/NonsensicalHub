using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    public Texture2D img;
    string user_text = string.Empty;
    string pwd_text = string.Empty;
    float cur_value1;
    float cur_value2;
    public GameObject cube;
    bool rotate = false;
    float last_num = 0, current_num;

    private void OnGUI()
    {
        //text
        GUIStyle gui_style = new GUIStyle();
        gui_style.normal.textColor = Color.black;
        gui_style.alignment = TextAnchor.MiddleCenter;
        gui_style.fontStyle = FontStyle.Bold;
        gui_style.fontSize = 20;
        GUI.Label(new Rect(Screen.width - 120, 20, 100, 20), "宽度：" + Screen.width.ToString() + "\n高度：" + Screen.height.ToString(), gui_style);
        //button
        if (GUI.Button(new Rect(10, 10, 100, 20), "旋转"))
        {
            rotate = !rotate;
        }
        if (GUI.Button(new Rect(10, 30, 100, 100), img))
        {
            cube.transform.rotation = Quaternion.Euler(new Vector3(0, 180 + cur_value1, 0));
        }
        GUI.color = Color.yellow;
        if (GUI.RepeatButton(new Rect(10, 130, 100, 20), "持续"))
        {
            cube.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime * 5);
        }
        GUI.color = Color.white;
        //input
        GUI.Label(new Rect(Screen.width * 0.5f - 100, 20, 100, 20), "用户名");
        user_text = GUI.TextField(new Rect(Screen.width * 0.5f - 50, 20, 100, 20), user_text);
        GUI.Label(new Rect(Screen.width * 0.5f - 100, 40, 100, 20), "密码");
        pwd_text = GUI.PasswordField(new Rect(Screen.width * 0.5f - 50, 40, 100, 20), pwd_text, '*');
        if (GUI.Button(new Rect(Screen.width * 0.5f + 75, 20, 40, 40), "登录"))
        {
            Debug.Log("用户名为：" + user_text);
            Debug.Log("密码为：" + pwd_text);
        }
        //滚动条
        GUI.Label(new Rect(10, Screen.height - 40, 100, 20), cur_value1.ToString());
        cur_value1 = GUI.HorizontalSlider(new Rect(10, Screen.height - 20, 100, 20), cur_value1, 0, 360);
        GUI.Label(new Rect(Screen.width - 120, Screen.height - 20, 100, 20), cur_value2.ToString());
        cur_value2 = GUI.VerticalSlider(new Rect(Screen.width - 20, Screen.height - 120, 20, 100), cur_value2, 10, 0);
    }
    private void Update()
    {
        current_num = cur_value1 - last_num;
        cube.transform.Rotate(new Vector3(0, current_num, 0));
        if (rotate)
            cube.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime * 5);
        cube.transform.position = new Vector3(0, cur_value2 - 5, 0);
        last_num = cur_value1;
    }
}
