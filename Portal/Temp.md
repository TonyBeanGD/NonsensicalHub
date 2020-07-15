
litjson获取集合键名
JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            IDictionary idic = (IDictionary)jsonData;
            foreach (var item in idic.Keys)
            {
                Debug.Log(item.ToString());
            }

环境：c#控制台程序
委托和事件的创建：
public delegate void delegate_name(object arg_name);
public event delegate_name event_Name;

字典:
            //字典
            //键和值是一一对应的
            //键是不能重复的，值可以重复
            Dictionary<long, string> dic = new Dictionary<long, string>();
            //添加一个项
            dic.Add(2330160206, "董礼斌");
            dic.Add(2330160207, "sha0");
            dic.Add(2330160208, "sha1");
            dic.Add(2330160209, "sha2");
            dic.Add(2330160210, "sha3");
            dic.Add(2330160211, "sha4");
            dic.Add(2330160212, "sha5");
            dic.Add(2330160213, "sha6");
            dic.Add(2330160214, "sha7");
            dic.Add(2330160215, "sha8");
            dic.Add(2330160216, "sha9");
            //键值对
            KeyValuePair<long, string> kvp;
            string st;
            //尝试搜寻键对应的值
            bool b = dic.TryGetValue(2330160206, out st);
            //第0个键值对
            dic.ElementAt(0);
            //根据键修改值
            dic[2330160206] = "donglibin";
            //linq查询
            var newDic =
                from temp in dic
                where temp.Key % 2 == 1
                select temp;
            Console.WriteLine(st);
            foreach (var item in newDic)
            {
                Console.WriteLine(item);
            }
            Console.Read();
            //string.Format

随机值：
Random r = new Random();
int i= r.Next(0, 101);

链表选择创建：
 List<T> List_2 = (from temp in List_1
                   where temp.arg == true
                   select temp).ToList();
				   
环境：C#控制台程序

清屏函数：Console.Clear();

使用相对路径：@+相对路径，例：@"test.txt"
	如果时为发布的程序，相对路径根为：项目名称\Bin\Debug\

查看文件是否存在：File.Exists("path")

读取文档，逐行输出：
StreamReader sr = new StreamReader(@"Debug.txt", Encoding.UTF8);
string line;
while ((line = sr.ReadLine()) != null)
{
	Console.WriteLine(line.ToString());
}

 if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }


//transform.RotateAround(player.position, Vector3.up, Input.GetAxis("Mouse X"));//系统围绕旋转方法



    public void  SetMouseCursor()//设置鼠标样式
    {
        Cursor.SetCursor(cursor,Vector2.zero, CursorMode.Auto);
    }

  Quaternion a = Quaternion.LookRotation(new Vector3(0, 0, 0));


var content = new StreamReader("path",Encoding.UTF8).ReadToEnd();

var content = File.ReadAllText("path");


        //文件读取流
        StreamReader sr = new StreamReader(xmlpath, System.Text.Encoding.UTF8);
        string content = sr.ReadToEnd();
        sr.Close();

        //文件写入流
        FileStream fs = new FileStream(xmlpath1, FileMode.Create);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
        fs.Write(data, 0, data.Length);
        fs.Close();


环境：unity3D
InputField:
	让光标置于选定输入栏：inputfieldname.Select()
	判断光标是否处于选定输入栏上：inputfieldname.isFocused
	
	字符串字符替换和切割：
	  _s = _s.Replace("(", "").Replace(")", "");
	        string[] s = _s.Split(',');
			
			环境：unity3d
			ongui按钮:
			void OnGUI()
			    {
			        if (GUI.Button(new Rect(0, 0, 100, 50), "第一个Button"))
			        {
			            Debug.Log("第一个Button被点击了！");
			        }
			
			        if (GUI.Button(new Rect(20, 20, 100, 50), "第二个Button"))
			        {
			            Debug.Log("第二个Button被点击了！");
			        }
			    }
				
				
				环境：unity3D
				获取变量名：
				string a;
				string s = GetVarName(p => a);
				//获取变量名
				public static string GetVarName(System.Linq.Expressions.Expression<Func<string, string>> exp)
				{
				    return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
				}
				获取时间：
				 TimeVal.text = string.Format("{0:D2}:{1:D2} {2}", h, m, ap);
				        //  Debug.Log("W now  " + System.DateTime.Now);        //当前时间（年月日时分秒）  
				        //  Debug.Log("W utc  " + System.DateTime.UtcNow);     // 当前时间（年月日时分秒）  
				        //  Debug.Log("W year  " + System.DateTime.Now.Year);  //当前时间（年）  
				        //  Debug.Log("W month   " + System.DateTime.Now.Month); //当前时间（月）  
				        //  Debug.Log("W day   " + System.DateTime.Now.Day);    // 当前时间(日)  
				        //  Debug.Log("W h    " + System.DateTime.Now.Hour);  // 当前时间(时)  
				        //  Debug.Log("W min   " + System.DateTime.Now.Minute);  // 当前时间(分)  
				        //   Debug.Log("W second   " + System.DateTime.Now.Second); // 当前时间(秒) 
				字符串格式：
				 TimeVal.text = string.Format("<color=#FFE300FF>当前运动时间:</color>{0:D2}:{1:D2}:{2:D2}", h, m, s);
				 
				 环境：C#
				 static int i = 0;
				         static void Main(string[] args)
				         {
				             
				             i=test()+i;
				         }
				 
				         private static int test()
				         {
				             i++;
				             return 2;
				         }
				 运行结束时i=3
				  static int i = 0;
				         static void Main(string[] args)
				         {
				             
				             i+=test();
				         }
				 
				         private static int test()
				         {
				             i++;
				             return 2;
				         }
				 运行结束时i=2
				 
				 环境：unity3d
				 创建文件夹和检测文件夹：
				  if(Directory.Exists(Path))
				         {
				             Debug.Log("存在");
				         }
				         else
				         {
				             Directory.CreateDirectory(Path);
				             Debug.Log("不存在");
				         }
						 
						 
						 写入TXT文本时的换行：
						 	要使用\r\n
				
 /// <summary>
        /// 获取当前工作目录的完全限定路径
        /// </summary>
        /// <returns>当前工作目录的完全限定路径</returns>
        public static string Get_Current_Path()
        {
            string path = null;
            if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows应用程序则相等  
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory + "Bin\\";
            }
            return path;
        }

//”我的文档“路径
Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

/// <summary>
        /// 获取当前日期的字符串，以下划线隔开
        /// </summary>
        /// <returns>当前日期的字符串</returns>
        internal static string Get_Date_String()
        {
            return DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day;
        }

/// <summary>
        /// 获取目标路径xml文件的根节点
        /// </summary>
        /// <param name="_path">目标文件路径</param>
        /// <returns>当目标文件存在并且获取成功时，返回目标xml文件根节点，否则返回null</returns>
        public static XmlNode GetRoot(string _path)
        {
            if (!File.Exists(_path))
            {
                return null;
            }

            XmlDocument xmlfile = new XmlDocument();

            try
            {
                xmlfile.Load(_path);
                XmlNode nodes = xmlfile.SelectSingleNode("Root");
                return nodes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建一个新的XML文件(覆盖)
        /// </summary>
        /// <param name="_path">创建文件的存放路径</param>
        /// <returns></returns>
        public static bool Create_New_Xml_File(string _path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);
            try
            {
                xmlDoc.Save(_path);
                return true;
            }
            catch (Exception e)
            {
                Nonsensical_Manager.Debug_Log(e.Message);
                return false;
            }
        }


       /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <returns>当数据转移成功时时，返回真，否则返回假</returns>
        public static bool Transfer_Data(string _filepath1, string _filepath2)
        {
            if (!File.Exists(_filepath1) || !File.Exists(_filepath1))
            {
                return false;
            }

            StreamReader sr = null;
            FileStream fs = null;

            try
            {
                sr = new StreamReader(_filepath1, Encoding.UTF8);
                string content = sr.ReadToEnd();

                fs = new FileStream(_filepath2, FileMode.Create);
                byte[] data = Encoding.UTF8.GetBytes(content);
                fs.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <param name="_encoding">编码格式</param>
        /// <returns>当转移成功时时，返回真，否则返回假</returns>
        public static bool Transfer_Data(string _filepath1, string _filepath2, Encoding _encoding)
        {
            StreamReader sr = null;
            FileStream fs = null;

            if (!File.Exists(_filepath1) || !File.Exists(_filepath1))
            {
                return false;
            }

            try
            {
                sr = new StreamReader(_filepath1, Encoding.UTF8);
                string content = sr.ReadToEnd();

                fs = new FileStream(_filepath2, FileMode.Create);
                byte[] data = _encoding.GetBytes(content);
                fs.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        
        /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <param name="_origin">源文件编码格式</param>
        /// <param name="_target">目标文件编码格式</param>
        /// <returns>当转移成功时时，返回真，否则返回假</returns>
        public static bool Transfer_Data(string _filepath1, string _filepath2, Encoding _origin, Encoding _target)
        {
            StreamReader sr = null;
            FileStream fs = null;

            if (!File.Exists(_filepath1) || !File.Exists(_filepath1))
            {
                return false;
            }

            try
            {
                sr = new StreamReader(_filepath1, _origin);
                string content = sr.ReadToEnd();

                fs = new FileStream(_filepath2, FileMode.Create);
                byte[] data = _target.GetBytes(content);
                fs.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 创建一个新的txt文件（覆盖）并往其中写入消息
        /// </summary>
        /// <param name="_path">创建文件路径</param>
        /// <param name="_message">写入的消息</param>
        /// <returns>创建并写入成功则返回true，否则返回false</returns>
        public static bool Create_And_Write(string _path, string _message)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = new FileStream(_path, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(_message);
                return true;
            }
            catch (Exception e)
            {
                Nonsensical_Manager.Debug_Log(e.Message);
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
		
		环境：C#
		//委托的定义
		public delegate void 【委托名】(【参数表】);
		//事件的定义
		public static event 【委托名】【事件名】
		//事件添加方法
		【事件名】 += 【要添加的方法名】;
		//定义线程
		Thread 【线程名】;
		//给线程赋值并开启线程
		Run = new Thread(new ThreadStart(【方法名】));
		//定义随机数生成器
		Random 【name】;
		//线程休眠
		Thread.Sleep(1000);
		//定义枚举
		 public enum 【枚举名】
		{
		    【枚举值表】
		}
		例：
		public enum Level
		{
		    one=0,
		    two=1,
		    three=2,
		}
		//枚举的使用
		Level level=Level.one;或者Level level=(Level)0;
		
		//字节数组和字符串的相互转换
		Byte[] buffer = Encoding.UTF8.GetBytes(Console.ReadLine());
		for (int i = 0; i < buffer.Length; i++)
		{
		    Console.WriteLine(buffer[i]);
		}
		Console.WriteLine(new UTF8Encoding().GetString(buffer, 0, buffer.Length));
		
		如何获取指定目录包含的文件和子目录
		1. DirectoryInfo.GetFiles()：获取目录中（不包含子目录）的文件，返回类型为FileInfo[]，支持通配符查找；
		2. DirectoryInfo.GetDirectories()：获取目录（不包含子目录）的子目录，返回类型为DirectoryInfo[]，支持通配符查找；
		3. DirectoryInfo. GetFileSystemInfos()：获取指定目录下（不包含子目录）的文件和子目录，返回类型为FileSystemInfo[]，支持通配符查找；
		如何获取指定文件的基本信息；
		FileInfo.Exists：获取指定文件是否存在；
		FileInfo.Name，FileInfo.Extensioin：获取文件的名称和扩展名；
		FileInfo.FullName：获取文件的全限定名称（完整路径）；
		FileInfo.Directory：获取文件所在目录，返回类型为DirectoryInfo；
		FileInfo.DirectoryName：获取文件所在目录的路径（完整路径）；
		FileInfo.Length：获取文件的大小（字节数）；
		FileInfo.IsReadOnly：获取文件是否只读；
		FileInfo.Attributes：获取或设置指定文件的属性，返回类型为FileAttributes枚举，可以是多个值的组合
		FileInfo.CreationTime、FileInfo.LastAccessTime、FileInfo.LastWriteTime：分别用于获取文件的创建时间、访问时间、修改时间；
		
		
		
		    unity3d提供了一个用于本地持久化保存与读取的类——PlayerPrefs。工作原理非常简单，以键值对的形式将数据保存在文件中，然后程序可以根据这个名称取出上次保存的数值。
		    PlayerPrefs类支持3中数据类型的保存和读取，浮点型，整形，和字符串型。
		    分别对应的函数为：
		    SetInt();保存整型数据；
		GetInt();读取整形数据；
		SetFloat();保存浮点型数据；
		    GetFlost();读取浮点型数据；
		    SetString();保存字符串型数据；
		    GetString();读取字符串型数据；
		这些函数的用法基本一致使用Set进行保存，使用Get进行读取。
		
		PlayerPrefs.SetString("_NAME", set_NAME); 这个方法中第一个参数表示存储数据的名称，第二的参数表示具体存储的数值。
		
		get_NAME=PlayerPrefs.GetString("_NAME"); 这个方法中第一个数据表示读取数据的名称，本来还有第二的参数，表示默认值，如果通过数据名称没有找到对应的值，那么就返回默认值，这个值也可以写，则返回空值。
		
		在PlayerPrefs 类中还提供了
		PlayerPrefs.DeleteKey (key : string)删除指定数据；
		PlayerPrefs.DeleteAll() 删除全部键 ;
		PlayerPrefs.HasKey (key : string)判断数据是否存在的方法；
		
		环境：C#
		线程简单定义： 
		Thread threadNiming = new Thread(new ThreadStart(delegate ()  
		            {  
		                string firstParam = "Hello ";  
		                string secondParam = "World";  
		                Console.WriteLine(firstParam + secondParam);  
		            }));  
					
					/// <summary>
					   /// 添加盒子碰撞器并且自适应其大小
					   /// </summary>
					   /// <param name="go"></param>
					   private static void FitCollider(GameObject go)
					   {
					       Renderer Renderer = go.transform.GetComponent<Renderer>();
					       if (Renderer == null)
					       {
					           return;
					       }
					       BoxCollider bc;
					       if (go.GetComponent<BoxCollider>() != null)
					       {
					           bc = go.GetComponent<BoxCollider>();
					       }
					       else
					       {
					           bc = go.AddComponent<BoxCollider>();
					       }
					       bc.isTrigger = true;
					
					       Quaternion qn = go.transform.rotation;
					       go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
					       Bounds bounds = Renderer.bounds;
					       go.transform.rotation = qn;
					
					       bc.size = bounds.size;
					   }
					
					   /// <summary>
					   /// 创建一个刚好包住所有子物体的盒子碰撞器
					   /// </summary>
					   /// <param name="go"></param>
					   private static void FitToChildren(GameObject go)
					   {
					       Quaternion qn = go.transform.rotation;
					       go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
					
					       bool hasBounds = false;
					       Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
					
					       Renderer[] childRenderers = go.transform.GetComponentsInChildren<Renderer>();
					
					       foreach (var item in childRenderers)
					       {
					           if (hasBounds)
					           {
					               bounds.Encapsulate(item.bounds);
					           }
					           else
					           {
					               bounds = item.bounds;
					               hasBounds = true;
					           }
					
					       }
					
					       BoxCollider collider;
					       if (go.GetComponent<BoxCollider>() != null)
					       {
					           collider = go.GetComponent<BoxCollider>();
					       }
					       else
					       {
					           collider = go.AddComponent<BoxCollider>();
					       }
					       collider.isTrigger = true;
					       collider.center = bounds.center - go.transform.position;
					       collider.size = bounds.size;
					
					       go.transform.rotation = qn;
					   }