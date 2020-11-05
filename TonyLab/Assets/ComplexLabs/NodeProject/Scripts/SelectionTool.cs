using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NonsensicalKit
{
    /// <summary>
    /// 框选工具
    /// </summary>
    public class SelectionTool : MonoBehaviour
    {
        public List<Transform> crtSelectTargets;
        public Transform[] needCheckTargets;
        public bool IsOpenSelect {  get;  set; } = false;//是否开启框选
        public Action OnSelectOver;

        [SerializeField]
        private Color rectColor = Color.green;//框选使用的颜色
        [SerializeField]
        private Material lineMaterial = null;//这里使用Sprite下的defaultshader的材质即可
        [SerializeField]
        private float frameWidth = 2.5f;//边框的宽度
        [SerializeField,Range(0, 1)]
        private float frameAlpha = 0.8f;//边框的透明度

        private Color mainColor;//主要颜色
        private Color frameColor;//边框的颜色
        private Vector3 start = Vector3.zero;//记下鼠标按下位置
        private bool drawRectangle = false;//是否画线

        private List<Transform> lastTargets;//上一次点击时的对象链表
        private List<Transform> crtTargets;//鼠标当前位置的对象链表
        private int index = 0;//当前选择的对象的索引

        void Start()
        {
            frameColor = new Color(rectColor.r, rectColor.g, rectColor.b, frameAlpha);
            mainColor = new Color(rectColor.r, rectColor.g, rectColor.b, 0.1f);
        }

        void Update()
        {
#if UNITY_EDITOR
            frameColor = new Color(rectColor.r, rectColor.g, rectColor.b, frameAlpha);
            mainColor = new Color(rectColor.r, rectColor.g, rectColor.b, 0.1f);
#endif

            if (IsOpenSelect)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    drawRectangle = true;//如果鼠标左键按下 设置开始画线标志
                    start = Input.mousePosition;//记录按下位置
                }

                if (Input.GetMouseButtonUp(0) && drawRectangle)
                {
                    if (Vector3.Distance( start, Input.mousePosition) >0)
                    {
                        drawRectangle = false;//如果鼠标左键放开 结束画线
                        CheckSelection(start, Input.mousePosition);
                    }
                    else
                    {
                        UpdateCrtTargets();

                        crtSelectTargets.Clear();

                        if (crtTargets.Count > 0)
                        {
                            if (lastTargets == null || CompareList(lastTargets, crtTargets) == false)
                            {
                                index = 0;
                                lastTargets = new List<Transform>(crtTargets.ToArray());
                            }
                            else
                            {
                                index++;
                                if (index >= crtTargets.Count)
                                {
                                    index = 0;
                                }
                            }

                            crtSelectTargets.Add(crtTargets[index]);
                        }
                    }
                }
                OnSelectOver?.Invoke();
            }
        }

        void OnPostRender()
        {
            //画线这种操作推荐在OnPostRender（）里进行 而不是直接放在Update，所以需要标志来开启
            if (drawRectangle)
            {
                CreateLineMaterial();
                Vector3 end = Input.mousePosition;//鼠标当前位置
                if (Vector3.Distance(start,end)==0)
                {
                    return;
                }
                GL.PushMatrix();//保存摄像机变换矩阵,把投影视图矩阵和模型视图矩阵压入堆栈保存
                if (!lineMaterial)
                    return;
                lineMaterial.SetPass(0);//为渲染激活给定的pass。
                GL.LoadPixelMatrix();//设置用屏幕坐标绘图
                GL.Begin(GL.QUADS);//开始绘制矩形
                GL.Color(mainColor);//设置颜色和透明度，方框内部透明
                                    //绘制顶点
                GL.Vertex3(start.x, start.y, 0);
                GL.Vertex3(end.x, start.y, 0);
                GL.Vertex3(end.x, end.y, 0);
                GL.Vertex3(start.x, end.y, 0);
                GL.End();

                #region 绘制边框
                Vector3 p1 = CorrectionPosition(start, end).Item1;
                Vector3 p2 = CorrectionPosition(start, end).Item2;

                //绘制边框的左侧
                GL.Begin(GL.QUADS);
                GL.Color(frameColor);
                GL.Vertex3(p1.x - frameWidth, p1.y + frameWidth, 0);
                GL.Vertex3(p1.x, p1.y + frameWidth, 0);
                GL.Vertex3(p1.x, p2.y - frameWidth, 0);
                GL.Vertex3(p1.x - frameWidth, p2.y - frameWidth, 0);
                GL.End();

                //绘制边框的上方
                GL.Begin(GL.QUADS);
                GL.Color(frameColor);
                GL.Vertex3(p1.x, p1.y + frameWidth, 0);
                GL.Vertex3(p2.x, p1.y + frameWidth, 0);
                GL.Vertex3(p2.x, p1.y, 0);
                GL.Vertex3(p1.x, p1.y, 0);
                GL.End();

                //绘制边框的右侧
                GL.Begin(GL.QUADS);
                GL.Color(frameColor);
                GL.Vertex3(p2.x, p1.y + frameWidth, 0);
                GL.Vertex3(p2.x + frameWidth, p1.y + frameWidth, 0);
                GL.Vertex3(p2.x + frameWidth, p2.y - frameWidth, 0);
                GL.Vertex3(p2.x, p2.y - frameWidth, 0);
                GL.End();

                //绘制边框的下方
                GL.Begin(GL.QUADS);
                GL.Color(frameColor);
                GL.Vertex3(p1.x, p2.y, 0);
                GL.Vertex3(p2.x, p2.y, 0);
                GL.Vertex3(p2.x, p2.y - frameWidth, 0);
                GL.Vertex3(p1.x, p2.y - frameWidth, 0);
                GL.End();
                #endregion

                GL.PopMatrix();//恢复摄像机投影矩阵
            }
        }

        /// <summary>
        /// 更新当前选择对象链表
        /// </summary>
        private void UpdateCrtTargets()
        {
            crtTargets.Clear();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitInfos = Physics.RaycastAll(ray, 100);
            
            foreach (var item in hitInfos)
            {
                crtTargets.Add(item.transform);
            }
        }

        /// <summary>
        /// 比较两个链表内容是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns>为true时代表相等</returns>
        private bool CompareList<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].Equals(list2[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 矫正坐标，得出用户框选区域左上角和右下角的坐标（开始点不一定是左上角，结束点同理）
        /// </summary>
        /// <param name="startPos">鼠标拖拽的开始点</param>
        /// <param name="endPos">鼠标拖拽的结束点</param>
        /// <returns>坐标元组，第一个值是左上角的坐标，第二个值是右下角的坐标</returns>
        private Tuple<Vector3, Vector3> CorrectionPosition(Vector3 startPos, Vector3 endPos)
        {
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;
            if (startPos.x > endPos.x)
            {
                p1.x = endPos.x;
                p2.x = startPos.x;
            }
            else
            {
                p1.x = startPos.x;
                p2.x = endPos.x;
            }
            if (startPos.y > endPos.y)
            {
                p1.y = endPos.y;
                p2.y = startPos.y;
            }
            else
            {
                p1.y = startPos.y;
                p2.y = endPos.y;
            }

            return new Tuple<Vector3, Vector3>(p1, p2);
        }

        /// <summary>
        /// 检测被选择的物体
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void CheckSelection(Vector3 start, Vector3 end)
        {
            crtSelectTargets.Clear();
            Vector3 p1 = CorrectionPosition(start, end).Item1;
            Vector3 p2 = CorrectionPosition(start, end).Item2;

            foreach (Transform obj in needCheckTargets)
            {
                    Vector3 location = Camera.main.WorldToScreenPoint(obj.transform.position );//把对象的position+boxcenter转换成屏幕坐标
                    if (location.x > p1.x && location.x < p2.x && location.y > p1.y && location.y < p2.y
                    && location.z > Camera.main.nearClipPlane && location.z < Camera.main.farClipPlane)//z方向就用摄像机的设定值，看不见的也不需要选择了
                    {
                        crtSelectTargets.Add(obj);
                    }
            }
        }


        private void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

    }
}
