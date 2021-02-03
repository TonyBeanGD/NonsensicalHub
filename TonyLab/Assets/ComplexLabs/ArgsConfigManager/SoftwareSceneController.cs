//using HighlightingSystem;
//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using UnityEngine;
//using XProject.Models;


///// <summary>
///// 加载和保存场景以及模型的导入的控制器
///// </summary>
//public class SoftwareSceneController : MonoBehaviour
//{
//    private const int version = 3;

//    private SuperConfigData configData;
//    #region 加载场景用私有变量
//    private int loadVersion;
//    private SuperSaveData savedata;
//    /// <summary>
//    /// 当前正在加载的模型
//    /// </summary>
//    private Dictionary<string, Action> loadingModel;
//    /// <summary>
//    /// 当前加载完成的模型
//    /// </summary>
//    private Dictionary<string, Transform> loadedModel;
//    /// <summary>
//    /// 记录正在加载中的数量，加载之后为0代表所有需要加载的模型都加在完毕了
//    /// </summary>
//    private int LoadCount;
//    /// <summary>
//    /// 用于临时保存模型ab包所在文件夹
//    /// </summary>
//    private string modelDirPath;
//    #endregion

//    private void Awake()
//    {
//        NotificationCenter.Instance().AttachObsever(ActionType.LoadScene, this.LoadScene);
//        NotificationCenter.Instance().AttachObsever(ActionType.SaveScene, this.SaveScene);
//        NotificationCenter.Instance().AttachObsever(ActionType.ImportModel, this.ImportModel);

//        NotificationCenter.Instance().AttachObsever(ActionType.ResetScene, this.ResetScene);
//    }

//    private void Start()
//    {
//        loadingModel = new Dictionary<string, Action>();
//        loadedModel = new Dictionary<string, Transform>();

//        if (File.Exists(Path.Combine(Application.streamingAssetsPath, "Json", "SuperConfigs.json")) == false)
//        {
//            Debug.LogError("无法找到配置文件");
//        }
//        else
//        {
//            string text = System.IO.File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Json", "SuperConfigs.json"));
//            configData = JsonConvert.DeserializeObject<SuperConfigData>(text);
//        }

//    }

//    private void OnDestroy()
//    {
//        NotificationCenter.Instance().DetachObsever(ActionType.LoadScene, this.LoadScene);
//        NotificationCenter.Instance().DetachObsever(ActionType.SaveScene, this.SaveScene);
//        NotificationCenter.Instance().DetachObsever(ActionType.ImportModel, this.ImportModel);

//        NotificationCenter.Instance().DetachObsever(ActionType.ResetScene, this.ResetScene);
//    }

//    /// <summary>
//    /// 加载场景入口函数
//    /// </summary>
//    /// <param name="modelDirPath">模型ab包所在文件夹路径</param>
//    /// <param name="sceneJsonFilePath">场景文件json路径</param>
//    public void LoadScene(Notification noti)
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.ResetAB, new Notification(true, this));
//        Tuple<string, string> temp = noti.Arguments as Tuple<string, string>;

//        this.modelDirPath = temp.Item1;
//        string sceneJsonFilePath = temp.Item2;

//        if (File.Exists(sceneJsonFilePath))
//        {
//            StartCoroutine(LoadScene(sceneJsonFilePath));
//        }
//        else
//        {
//            Debug.LogWarning("无法加载场景，因为找不到场景Json文件");
//        }
//    }

//    private IEnumerator LoadScene(string sceneJsonFilePath)
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.CloseVirtualConnectionScene, new Notification(this));
//        NotificationCenter.Instance().PostDispatch(ActionType.ResetScene, new Notification(this));

//        yield return null;

//        StreamReader file = File.OpenText(sceneJsonFilePath);
//        string fileContent = file.ReadToEnd();
//        savedata = JsonConvert.DeserializeObject<SuperSaveData>(fileContent);
//        file.Close();

//        loadVersion = savedata.version;

//        LoadCount = savedata.topNodes.Count;
//        for (int i = 0; i < savedata.topNodes.Count; i++)
//        {
//            LoadAndUpdateModel(savedata.topNodes[i], null);
//        }

//        CutterManager.Instance().LoadCutterInfo(savedata.cutterInfos);
//        GDataManager.Instance.coordinatePoints = savedata.coordinatePoints;
//    }

//    /// <summary>
//    /// 保存场景入口函数
//    /// </summary>
//    private void SaveScene(Notification noti)
//    {
//        SuperSaveData saveData = new SuperSaveData();
//        saveData.version = version;

//        for (int i = 0; i < GameManager.instance.createRoot.childCount; i++)
//        {
//            if ((GameManager.instance.createRoot.GetChild(i).GetComponent<ModelInfo>() != null&& GameManager.instance.createRoot.GetChild(i).GetComponent<ModelInfo>().IsDelete==false))
//            {
//                UpdateNodeInfoArgs(GameManager.instance.createRoot.GetChild(i));
//                saveData.topNodes.Add(GameManager.instance.createRoot.GetChild(i).GetComponent<ModelInfo>().nodeInfo);
//            }
//        }
//        MotionVesselManager.SaveMotion(savedata);

//        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
//        foreach (var t in types)
//        {
//            var interfaces = t.GetInterfaces();

//            foreach (var itfc in interfaces)
//            {
//                if (itfc.IsGenericType && itfc.GetGenericTypeDefinition() == typeof(IUseArgsClass<>))
//                {
//                    saveData.SaveData.Add(t.ToString(), new Dictionary<string, List<string>>());

//                    MethodInfo useMethod = itfc.GetMethod("GetArgs");

//                    Component[] components = GameManager.instance.createRoot.GetComponentsInChildren(t);

//                    foreach (var item in components)
//                    {
//                        string id = item.GetComponent<ModelInfo>().nodeInfo.selfID;
//                        if (saveData.SaveData[t.ToString()].ContainsKey(id) == false)
//                        {
//                            saveData.SaveData[t.ToString()].Add(id, new List<string>());
//                        }

//                        object args = useMethod.Invoke(item, null);
//                        string str = JsonConvert.SerializeObject(args);

//                        saveData.SaveData[t.ToString()][id].Add(str);
//                    }
//                    break;
//                }
//            }
//        }


//        saveData.coordinatePoints = GDataManager.Instance.coordinatePoints;
//        saveData.cutterInfos = CutterManager.Instance().cutterInfos;

//        string randomSceneName = "Scene" + System.Guid.NewGuid().ToString();

//        CommonMethed.WriteJson(JsonConvert.SerializeObject(saveData, Formatting.Indented),
//               Application.streamingAssetsPath + "/SceneJson/",
//            randomSceneName);

//        string pathStr = Application.streamingAssetsPath + "/SceneJson/" + randomSceneName + ".json";

//        SendSignalHelper.SaveScene(Application.streamingAssetsPath + "/AssetBundle/Config/win",
//            pathStr,
//            GetNeedModel());
//    }

//    /// <summary>
//    /// 导入模型入口函数
//    /// </summary>
//    public void ImportModel(Notification noti)
//    {
//        Tuple<string, ImportMode, ImportInfo> tuple = noti.Arguments as Tuple<string, ImportMode, ImportInfo>;
//        string modelFilePath = tuple.Item1;
//        ImportMode importMode = tuple.Item2;
//        ImportInfo importInfo = tuple.Item3;
//        string temp = modelFilePath.Substring(modelFilePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
//        string modelName = temp.Substring(0, temp.LastIndexOf('.'));

//        NotificationCenter.Instance().PostDispatch(ActionType.LoadModelFormAB, new Notification(new Tuple<string, string, Action<GameObject>>(
//                   modelName,
//                   modelFilePath,
//                  (go) =>
//                  {
//                      LoadModel(go.transform, modelName, importMode, importInfo);
//                  }), this));
//    }


//    /// <summary>
//    /// 重置场景入口函数
//    /// </summary>
//    public void ResetScene(Notification noti)
//    {
//        for (int i = 0; i < GameManager.instance.createRoot.childCount; i++)
//        {
//            UnityEngine.Object.Destroy(GameManager.instance.createRoot.GetChild(i).gameObject);
//        }

//        NotificationCenter.Instance().PostDispatch(ActionType.SwitchAxis, new Notification(3, this));
//    }

//    #region 加载场景私有方法

//    /// <summary>
//    /// 加载场景完成
//    /// </summary>
//    private void LoadCompelete()
//    {
//        foreach (var item in loadedModel)
//        {
//            item.Value.gameObject.SetActive(false);
//            UnityEngine.Object.Destroy(item.Value.gameObject);
//        }

//        loadingModel.Clear();
//        loadedModel.Clear();

//        ModelNode nodeData = CommonMethed.GetModelNode();
//        SendSignalHelper.LoadScene(nodeData);

//        MotionVesselManager.Init(savedata.motions, savedata.resetInfos);

//        foreach (var data in savedata.SaveData)
//        {
//            var monoType = Type.GetType(data.Key);
//            MethodInfo initMethod = null;
//            MethodInfo deserializeMethod = typeof(JsonConvert).GetMethods().FirstOrDefault(p => p.IsStatic == true && p.IsPublic == true && p.Name == "DeserializeObject" && p.ContainsGenericParameters == true);


//            Type argsType = null;
//            var interfaces = monoType.GetInterfaces();
//            foreach (var itfc in interfaces)
//            {
//                if (itfc.IsGenericType && itfc.GetGenericTypeDefinition() == typeof(IUseArgsClass<>))
//                {
//                    argsType = itfc.GetGenericArguments()[0];
//                    initMethod = itfc.GetMethod("Init", new Type[] { argsType });

//                    deserializeMethod = deserializeMethod.MakeGenericMethod(new Type[] { argsType });
//                    break;
//                }
//            }
//            foreach (var target in data.Value)
//            {
//                GameObject temp = CommonMethed.GetGameObjectByName(target.Key);

//                foreach (var args in target.Value)
//                {
//                    Component component = temp.gameObject.AddComponent(monoType);

//                    object deserializeData = deserializeMethod.Invoke(null, new object[] { args });

//                    initMethod.Invoke(component, new object[] { deserializeData });
//                }
//            }
//        }

//        NotificationCenter.Instance().PostDispatch(ActionType.ResetAB, new Notification(false, this));
//    }

//    /// <summary>
//    /// 加载并更新模型
//    /// </summary>
//    /// <param name="nodeInfo"></param>
//    private void LoadAndUpdateModel(NodeInfoArgs nodeInfoArgs, Transform parentNode)
//    {
//        if (loadedModel.ContainsKey(nodeInfoArgs.rawModelName) == true)
//        {
//            GetNeedNode(loadedModel[nodeInfoArgs.rawModelName], nodeInfoArgs, parentNode);
//            LoadCount--;
//            if (LoadCount == 0)
//            {
//                LoadCompelete();
//            }
//        }
//        else
//        {
//            string modelName = nodeInfoArgs.rawModelName;

//            if (loadingModel.ContainsKey(modelName) == true)
//            {
//                loadingModel[modelName] += () => { LoadAndUpdateModel(nodeInfoArgs, parentNode); };
//            }
//            else
//            {
//                loadingModel.Add(modelName, new Action(() =>
//                {
//                    LoadAndUpdateModel(nodeInfoArgs, parentNode);
//                }));

//                string path = modelDirPath + "/" + modelName + ".assetBundle";

//                NotificationCenter.Instance().PostDispatch(ActionType.LoadModelFormAB, new Notification(new Tuple<string, string, Action<GameObject>>(
//                    modelName,
//                    path,
//                    (go) =>
//                    {
//                        LoadOldSaveModel(go.transform, modelName, nodeInfoArgs.rawModelName);
//                    }), this));

//            }
//        }
//    }

//    private void LoadOldSaveModel(Transform tsf, string modelName, string rawmodelname)
//    {
//        InitModelByConfigsFile(tsf);

//        loadedModel.Add(modelName, tsf);
//        loadingModel[modelName]();
//        loadingModel.Remove(rawmodelname);
//    }

//    private void UpdateNewPartModelInfo(Transform newPart, Transform rawPart)
//    {
//        newPart.GetComponent<ModelInfo>().nodeInfo.rawChild = new List<string>(rawPart.GetComponent<ModelInfo>().nodeInfo.rawChild.ToArray());

//        for (int i = 0; i < newPart.transform.childCount; i++)
//        {
//            UpdateNewPartModelInfo(newPart.GetChild(i), rawPart.GetChild(i));
//        }
//    }

//    /// <summary>
//    /// 根据节点路径从原始模型上实例化自己需要的节点
//    /// </summary>
//    /// <param name="rootTsf">原始模型</param>
//    /// <param name="rootNodeInfo">节点路径</param>
//    private void GetNeedNode(Transform rootTsf, NodeInfoArgs rootNodeInfoArgs, Transform parentNode)
//    {
//        Transform targetTsf = rootTsf;
//        if (parentNode == null)
//        {
//            parentNode = GameManager.instance.createRoot;
//        }

//        if (rootNodeInfoArgs.nodePath.Length != 0)
//        {
//            string[] paths = rootNodeInfoArgs.nodePath.Substring(1).Split(new char[1] { '|' });

//            for (int i = 0; i < paths.Length; i++)
//            {
//                targetTsf = targetTsf.GetChild(int.Parse(paths[i]));
//            }
//        }

//        GameObject newGameObject = UnityEngine.Object.Instantiate<GameObject>(targetTsf.gameObject, targetTsf.parent);
//        if (loadVersion <= 2)
//        {
//            UpdateNewPartModelInfo(newGameObject.transform, targetTsf);
//        }
//        newGameObject.transform.SetParent(parentNode);

//        string modelName = newGameObject.name.Substring(0, newGameObject.name.Length - 7);

//        UpdateNodeInfoBySaveData(newGameObject.transform, rootNodeInfoArgs);

//    }

//    /// <summary>
//    /// 根据节点信息类更新对应模型的信息
//    /// </summary>
//    /// <param name="rootTsf">模型Transform</param>
//    /// <param name="rootNodeInfo">根节点的信息类</param>
//    private void UpdateNodeInfoBySaveData(Transform rootTsf, NodeInfoArgs rootNodeInfoArgs)
//    {
//        Queue<Transform> nodes = new Queue<Transform>();
//        Queue<NodeInfoArgs> modelNodeInfos = new Queue<NodeInfoArgs>();

//        nodes.Enqueue(rootTsf);
//        modelNodeInfos.Enqueue(rootNodeInfoArgs);

//        while (nodes.Count > 0)
//        {
//            Transform crtNode = nodes.Dequeue();
//            NodeInfoArgs crtNodeInfo = modelNodeInfos.Dequeue();

//            if (loadVersion <= 2)
//            {
//                List<string> temp = new List<string>(crtNode.GetComponent<ModelInfo>().nodeInfo.rawChild.ToArray());
//                crtNode.GetComponent<ModelInfo>().nodeInfo = crtNodeInfo;
//                crtNode.GetComponent<ModelInfo>().nodeInfo.rawChild = temp;
//            }
//            else
//            {
//                crtNode.GetComponent<ModelInfo>().nodeInfo = crtNodeInfo;
//            }

//            crtNode.transform.localPosition = new Vector3(crtNodeInfo.nodePos.pX, crtNodeInfo.nodePos.pY, crtNodeInfo.nodePos.pZ);

//            crtNode.transform.eulerAngles = new Vector3(crtNodeInfo.nodeRot.rX, crtNodeInfo.nodeRot.rY, crtNodeInfo.nodeRot.rZ);

//            crtNode.name = crtNodeInfo.changeModelName;

//            if (crtNodeInfo.isHidden == true)
//            {
//                crtNode.GetComponent<ModelInfo>().Hide();
//            }

//            List<Transform> rawChilds = new List<Transform>();

//            foreach (Transform item in crtNode)
//            {
//                rawChilds.Add(item);
//            }

//            for (int i = 0; i < crtNodeInfo.childNodes.Count; i++)
//            {
//                if (crtNodeInfo.childNodes[i].needInstantiate == true)
//                {
//                    LoadCount++;
//                    LoadAndUpdateModel(crtNodeInfo.childNodes[i], crtNode);
//                    continue;
//                }

//                for (int j = 0; j < rawChilds.Count; j++)
//                {
//                    if (rawChilds[j].name == crtNodeInfo.childNodes[i].rawNodeName)
//                    {
//                        nodes.Enqueue(rawChilds[j]);
//                        modelNodeInfos.Enqueue(crtNodeInfo.childNodes[i]);
//                        rawChilds.RemoveAt(j);
//                        break;
//                    }
//                }
//            }

//            foreach (Transform item in rawChilds)
//            {
//                if (crtNodeInfo.rawChild.Contains(item.name))
//                {
//                    item.gameObject.SetActive(false);
//                    UnityEngine.Object.Destroy(item.gameObject);
//                }
//                else
//                {
//                    InitNewPart(item);
//                }
//            }
//        }
//    }

//    private void InitNewPart(Transform tran)
//    {
//        Queue<Transform> nodes = new Queue<Transform>();
//        nodes.Enqueue(tran);

//        while (nodes.Count > 0)
//        {
//            Transform crtNode = nodes.Dequeue();

//            ModelInfo crtModelInfo = tran.GetComponent<ModelInfo>();

//            crtModelInfo.nodeInfo.changeModelName = tran.name;
//            crtModelInfo.nodeInfo.topName = tran.parent.GetComponent<ModelInfo>().nodeInfo.topName;
//            crtModelInfo.nodeInfo.topID = tran.parent.GetComponent<ModelInfo>().nodeInfo.topID;
//            crtModelInfo.nodeInfo.selfID = System.Guid.NewGuid().ToString();
//            crtModelInfo.nodeInfo.mountNodeID = tran.parent.GetComponent<ModelInfo>().nodeInfo.selfID;

//            for (int i = 0; i < crtNode.childCount; i++)
//            {
//                if ( crtNode.GetChild(i).GetComponent<ModelInfo>() != null&& crtNode.GetChild(i).GetComponent<ModelInfo>().IsDelete==false)
//                {
//                    nodes.Enqueue(crtNode.GetChild(i));
//                }
//            }
//        }
//    }
//    #endregion

//    #region 保存场景私有方法
//    private string GetNeedModel()
//    {
//        Dictionary<string, string> names = new Dictionary<string, string>();
//        ModelInfo[] modelInfos = GameManager.instance.createRoot.GetComponentsInChildren<ModelInfo>();
//        foreach (var item in modelInfos)
//        {
//            if (names.ContainsKey(item.nodeInfo.rawModelName) == false)
//            {
//                names.Add(item.nodeInfo.rawModelName, item.nodeInfo.rawModelName);
//            }
//        }
//        string needString = string.Empty;
//        foreach (var item in names)
//        {
//            needString += item.Key + "|";
//        }
//        if (needString.Length > 0)
//        {
//            needString = needString.Substring(0, needString.Length - 1);
//        }
//        return needString;
//    }

//    private void UpdateNodeInfoArgs(Transform rootNode)
//    {
//        Queue<Transform> nodes = new Queue<Transform>();

//        nodes.Enqueue(rootNode);
//        while (nodes.Count > 0)
//        {
//            Transform crtNode = nodes.Dequeue();

//            ModelInfo modelInfo = crtNode.GetComponent<ModelInfo>();
//            if (!modelInfo)
//            {
//                continue;
//            }

//            NodeInfoArgs crtNodeInfoArgs = modelInfo.nodeInfo;
      
//            crtNodeInfoArgs.childNodes = new List<NodeInfoArgs>();
//            crtNodeInfoArgs.nodePos = new Positon3D() { pX = crtNode.localPosition.x, pY = crtNode.localPosition.y, pZ = crtNode.localPosition.z };
//            crtNodeInfoArgs.nodeRot = new Rotate3D() { rX = crtNode.eulerAngles.x, rY = crtNode.eulerAngles.y, rZ = crtNode.eulerAngles.z };

//            foreach (Transform item in crtNode)
//            {
//                if ( item.GetComponent<ModelInfo>() != null && item.GetComponent<ModelInfo>().IsDelete==false)
//                {
//                    crtNodeInfoArgs.childNodes.Add(item.GetComponent<ModelInfo>().nodeInfo);
//                    nodes.Enqueue(item);
//                }
//            }
//        }
//    }

//    #endregion

//    #region 加载模型私有方法

//    //加载模型初始化物品信息
//    private void Init(Transform rootTsf)
//    {
//        ModelInfo modeInfo = rootTsf.GetComponent<ModelInfo>();
//        InitNewModelInfo(rootTsf, rootTsf.name);

//        modeInfo.nodeInfo.modelName = rootTsf.name;
//        modeInfo.nodeInfo.topName = rootTsf.name;
//        rootTsf.name = modeInfo.nodeInfo.modelName;

//        CommonMethed.AutoChangeName(rootTsf);

//        CommonMethed.UpdateNewModelInfo(rootTsf);
//        SendModelImportSignal(rootTsf);
//    }

//    private void InitNewModelInfo(Transform tsf, string modelName)
//    {
//        tsf.GetComponent<ModelInfo>().nodeInfo.needInstantiate = true;

//        Stack<Transform> nodes = new Stack<Transform>();
//        Stack<string> nodePaths = new Stack<string>();

//        nodes.Push(tsf);
//        nodePaths.Push("");

//        while (nodes.Count > 0)
//        {
//            Transform node = nodes.Pop();
//            string nodePath = nodePaths.Pop();
//            if (node.GetComponent<ModelInfo>() != null)
//            {
//                node.GetComponent<ModelInfo>().nodeInfo.nodePath = nodePath;
//                node.GetComponent<ModelInfo>().nodeInfo.rawModelName = modelName;
//                node.GetComponent<ModelInfo>().nodeInfo.modelName = node.name;
//                node.GetComponent<ModelInfo>().nodeInfo.changeModelName = node.name;
//                node.GetComponent<ModelInfo>().nodeInfo.rawNodeName = node.name;
//                node.GetComponent<ModelInfo>().nodeInfo.rawChild = new List<string>();
//                for (int i = 0; i < node.childCount; i++)
//                {
//                    node.GetComponent<ModelInfo>().nodeInfo.rawChild.Add(node.GetChild(i).name);
//                    nodes.Push(node.GetChild(i));
//                    nodePaths.Push(nodePath + "|" + i);
//                }
//            }
//        }
//    }

//    //发送模型导入信号给WPF
//    private void SendModelImportSignal(Transform rootTsf)
//    {
//        if (rootTsf.GetComponent<ShowWPFNode>()==null)
//        {
//            return;
//        }
//        ModelInfo modeInfo = rootTsf.GetComponent<ModelInfo>();
//        Vector3 position = rootTsf.position;
//        Positon3D positon3D = new Positon3D { pX = position.x, pY = position.y, pZ = position.z };
//        Vector3 rotation = rootTsf.eulerAngles;
//        Rotate3D rotate3D = new Rotate3D { rX = rotation.x, rY = rotation.y, rZ = rotation.z };

//        ModelNode nodeData = CommonMethed.GetModelNode(rootTsf);

//        SendSignalHelper.ModelImport(modeInfo.nodeInfo.topName, modeInfo.nodeInfo.selfID, modeInfo.nodeInfo.topID, positon3D, rotate3D, nodeData);
//    }

//    private void LoadModel(Transform newTransform, string modelName, ImportMode importMode, ImportInfo importInfo)
//    {
//        InitModelByConfigsFile(newTransform);

//        if (configData.ConfigData.ContainsKey(newTransform.name))
//        {
//            foreach (var item in configData.ConfigData[newTransform.name])
//            {
//                Transform temp = CommonMethed.GetTransformByNodePath(newTransform.transform, item.Key);
//                foreach (var item2 in item.Value)
//                {
//                    var t = Type.GetType(item2.Key);
//                    Component component = temp.gameObject.AddComponent(t);

//                    var interfaces = t.GetInterfaces();

//                    foreach (var itfc in interfaces)
//                    {
//                        if (itfc.IsGenericType && itfc.GetGenericTypeDefinition() == typeof(IUseArgsClass<>))
//                        {
//                            MethodInfo unsubMethod = typeof(JsonConvert).GetMethods().FirstOrDefault(p => p.IsStatic == true && p.IsPublic == true && p.Name == "DeserializeObject" && p.ContainsGenericParameters == true);

//                            unsubMethod = unsubMethod.MakeGenericMethod(new Type[] { itfc.GetGenericArguments()[0] });
//                            object data = unsubMethod.Invoke(null, new object[] { item2.Value });
//                            MethodInfo unsubMethod2 = itfc.GetMethod("Init", new Type[] { itfc.GetGenericArguments()[0] });

//                            unsubMethod2.Invoke(component, new object[] { data });

//                            break;
//                        }
//                    }
//                }
//            }
//        }


//        Init(newTransform);

//        GameManager.instance.curDragObj = newTransform;
//        switch (importMode)
//        {
//            case ImportMode.NormalImport:
//                {
//                    GameManager.instance.crtMouseState = MouseLeftState.SetModel;
//                }
//                break;
//            case ImportMode.DoubleClickImport:
//                {
//                    Vector3? viewPoint;
//                    if ((viewPoint = CommonMethed.GetViewPoint(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward)) == null)
//                    {
//                        viewPoint = transform.position + transform.forward * 10;
//                    }

//                    GameManager.instance.curDragObj.position = CommonMethed.GetNearPoint((Vector3)viewPoint);
//                    NotificationCenter.Instance().PostDispatch(ActionType.RecordClickableThing, new Notification(new Tuple<List<ClickableThing>, bool, bool>(new List<ClickableThing> { GameManager.instance.curDragObj.GetComponent<ClickableThing>() }, false, false), this));
//                    NotificationCenter.Instance().PostDispatch(ActionType.LocationAxis, new Notification(this));
//                    GameManager.instance.curDragObj = null;
//                }
//                break;
//            case ImportMode.DragImport:
//                {
//                    Debug.LogWarning("暂不支持拖拽导入功能");

//                    if (importInfo != null)
//                    {
//                        NotificationCenter.Instance().PostDispatch(ActionType.Move, new Notification(new Tuple<Transform, Transform, Vector3?, Vector3?>(newTransform, importInfo.parent, importInfo.selfPos, importInfo.selfRot), this));
//                        newTransform.localPosition = importInfo.selfPos;
//                        newTransform.localEulerAngles = importInfo.selfRot;
//                        if (importInfo.bladeID >= 0)
//                        {
//                            newTransform.gameObject.GetComponent<BladeInfo>().bladeID = importInfo.bladeID;
//                            if (importInfo.cutterID >= 0)
//                                newTransform.gameObject.GetComponent<BladeInfo>().cutterID = importInfo.cutterID;
//                        }
//                    }
//                }
//                break;
//            case ImportMode.DragVirtualConnectionModel:
//                {
//                    if (importInfo != null)
//                    {
//                        newTransform.localPosition = importInfo.selfPos;
//                        newTransform.localEulerAngles = importInfo.selfRot;
//                    }
//                    NotificationCenter.Instance().PostDispatch(ActionType.GetVirtualConnectionModel, new Notification(newTransform, this));
//                }
//                break;
//            default:
//                {
//                    Debug.LogWarning("未进行判断的枚举类型" + importMode.ToString());

//                }
//                break;
//        }

//        //导入虚拟接线盒时不记入
//        if (importMode != ImportMode.DragVirtualConnectionModel)
//        {
//            NotificationCenter.Instance().PostDispatch(ActionType.RecordOperater, new Notification(new Tuple<List<Transform>, OperateType>(new List<Transform>() { newTransform }, OperateType.Import), this));
//        }

//    }


//    #endregion

//    #region 通用私有方法

//    private void InitModelByConfigsFile(Transform target)
//    {
//        target.name = target.name.Substring(0, target.name.Length - 7); //去掉尾部的"(Clone)"
//        Transform[] allChilds = target.GetComponentsInChildren<Transform>();

//        foreach (Transform item in allChilds)
//        {
//            item.gameObject.AddComponent<ModelInfo>();

//            if (loadVersion <= 2)
//            {
//                item.GetComponent<ModelInfo>().nodeInfo.rawChild = new List<string>();
//                foreach (Transform item2 in item)
//                {
//                    item.GetComponent<ModelInfo>().nodeInfo.rawChild.Add(item2.name);
//                }

//            }

//            item.gameObject.AddComponent<ClickableThing>();

//            item.gameObject.AddComponent<Highlighter>();

//            item.gameObject.AddComponent<SceneObjHighlightController>();
//        }
//    }
//    #endregion
//}

////倒入模型的父节点和自身的位置
//public class ImportInfo
//{
//    public Transform parent;
//    public int cutterID = -1;
//    public int bladeID = -1;
//    public Vector3 selfPos;
//    public Vector3 selfRot;
//}