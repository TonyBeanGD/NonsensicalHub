//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using UnityEngine;

///* 版本更新：
// * 2：保存节点信息的位置值从本地变成世界
// * 3: 新增保存节点信息的初始子物体名称链表用于在模型更改后控制节点的删除
// */

///// <summary>
///// 加载和保存场景以及模型的导入的控制器
///// </summary>
//public class SoftwareSceneController : MonoBehaviour
//{
//    private const int version = 3;

//    #region 加载场景用私有变量
//    private int loadVersion;
//    private ConfigArgs configArgs;
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

//    #region 保存场景用私有变量
//    private List<MotionInfo> motions = new List<MotionInfo>();
//    private List<ResetInfo> resetInfos = new List<ResetInfo>();
//    private List<string> robotArmID = new List<string>();
//    private List<RobotArmArgs> robotArms = new List<RobotArmArgs>();
//    #endregion


//    private void Start()
//    {
//        loadingModel = new Dictionary<string, Action>();
//        loadedModel = new Dictionary<string, Transform>();

//        if (File.Exists(Path.Combine(Application.streamingAssetsPath, "Json", "ModelConfigs.json")) == false)
//        {
//            Debug.LogError("无法找到配置文件");
//        }
//        else
//        {
//            string text = System.IO.File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Json", "ModelConfigs.json"));
//            configArgs = JsonConvert.DeserializeObject<ConfigArgs>(text);
//        }
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
//        NotificationCenter.Instance().PostDispatch(ActionType.ResetScene, new Notification(this));

//        yield return null;

//        StreamReader file = File.OpenText(sceneJsonFilePath);
//        string fileContent = file.ReadToEnd();
//        SaveDataArgs saveDataArgs = JsonConvert.DeserializeObject<SaveDataArgs>(fileContent);
//        file.Close();

//        loadVersion = saveDataArgs.version;

//        LoadCount = saveDataArgs.topNodes.Count;
//        for (int i = 0; i < saveDataArgs.topNodes.Count; i++)
//        {
//            LoadAndUpdateModel(saveDataArgs.topNodes[i], null);
//        }

//        motions = saveDataArgs.motions;
//        resetInfos = saveDataArgs.resetInfos;
//        robotArmID = saveDataArgs.robotArmID;
//        robotArms = saveDataArgs.robotArms;
//    }

//    /// <summary>
//    /// 保存场景入口函数
//    /// </summary>
//    private void SaveScene(Notification noti)
//    {
//        SaveDataArgs saveDataArgs = new SaveDataArgs();
//        saveDataArgs.version = version;

//        for (int i = 0; i < GameManager.instance.createRoot.childCount; i++)
//        {
//            if (GameManager.instance.createRoot.GetChild(i).gameObject.activeSelf == true)
//            {
//                UpdateNodeInfoArgs(GameManager.instance.createRoot.GetChild(i));
//                saveDataArgs.topNodes.Add(GameManager.instance.createRoot.GetChild(i).GetComponent<ModelInfo>().nodeInfo);
//            }
//        }

//        SaveMotion(saveDataArgs);
//        SaveRobotArm(saveDataArgs);

//        string randomSceneName = "Scene" + System.Guid.NewGuid().ToString();

//        CommonMethed.WriteJson(JsonConvert.SerializeObject(saveDataArgs, Formatting.Indented),
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
//        Tuple<string, ImportMode> tuple = noti.Arguments as Tuple<string, ImportMode>;
//        string modelFilePath = tuple.Item1;
//        ImportMode importMode = tuple.Item2;
//        string temp = modelFilePath.Substring(modelFilePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
//        string modelName = temp.Substring(0, temp.LastIndexOf('.'));

//        NotificationCenter.Instance().PostDispatch(ActionType.LoadModelFormAB, new Notification(new Tuple<string, string, Action<GameObject>>(
//                   modelName,
//                   modelFilePath,
//                  (go) =>
//                  {
//                      GameManager.instance.StartCoroutine(LoadModel(go.transform, modelName, importMode));
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

//        ModelNode nodeData = GetModelNodeJson();

//        SendSignalHelper.LoadScene(nodeData);

//        for (int i = 0; i < motions.Count; i++)
//        {
//            GameObject temp = CommonMethed.GetModelInfoByID(motions[i].ID).gameObject;

//            MotionVesselBase tempbase = null;
//            switch (motions[i].motionType)
//            {
//                case MotionType.Dismantle:
//                    tempbase = temp.AddComponent<MotionVesselDismantle>();
//                    break;
//                case MotionType.Generate:
//                    tempbase = temp.AddComponent<MotionVesselGenerate>();
//                    break;
//                case MotionType.Install:
//                    tempbase = temp.AddComponent<MotionVesselInstall>();
//                    break;
//                case MotionType.OpenClose:
//                    tempbase = temp.AddComponent<MotionVesselOpenClose>();
//                    break;
//                case MotionType.Rotate:
//                    tempbase = temp.AddComponent<MotionVesselRotate>();
//                    break;
//                case MotionType.Translation:
//                    tempbase = temp.AddComponent<MotionVesselTranslation>();
//                    break;
//                case MotionType.Transmit:
//                    tempbase = temp.AddComponent<MotionVesselTransmit>();
//                    break;
//            }
//            tempbase.resetInfo = resetInfos[i];
//        }

//        for (int i = 0; i < robotArmID.Count; i++)
//        {
//            RobotRunController temp = CommonMethed.GetModelInfoByID(robotArmID[i]).GetComponent<RobotRunController>();
//            temp.Init(robotArms[i]);
//            temp.ResetOrigin();
//        }


//        NotificationCenter.Instance().PostDispatch(ActionType.ResetAB, new Notification(false, this));
//    }

//    /// <summary>
//    /// 获取世界节点(modelCreateParent)的ModelNode
//    /// </summary>
//    /// <returns></returns>
//    private ModelNode GetModelNodeJson()
//    {
//        ModelNode rootModelNode = new ModelNode();

//        Queue<Transform> nodes = new Queue<Transform>();
//        Queue<ModelNode> modelNodes = new Queue<ModelNode>();

//        for (int i = 0; i < GameManager.instance.createRoot.childCount; i++)
//        {
//            if (GameManager.instance.createRoot.GetChild(i).GetComponent<ModelInfo>().nodeInfo.selfID != null &&
//                GameManager.instance.createRoot.GetChild(i).GetComponent<ModelInfo>().nodeInfo.selfID != "" &&
//                GameManager.instance.createRoot.GetChild(i).GetComponent<ModelInfo>().nodeInfo.selfID != string.Empty)
//            {
//                nodes.Enqueue(GameManager.instance.createRoot.GetChild(i));
//                modelNodes.Enqueue(rootModelNode);
//            }
//        }

//        while (nodes.Count > 0)
//        {
//            Transform crtNode = nodes.Dequeue();
//            ModelNode crtParentModelNode = modelNodes.Dequeue();

//            ModelNode crtModelNode = new ModelNode();
//            ModelInfo crtmodelInfo = crtNode.GetComponent<ModelInfo>();
//            crtModelNode.modelName = crtmodelInfo.nodeInfo.changeModelName;
//            crtModelNode.id = crtmodelInfo.nodeInfo.selfID;
//            crtModelNode.parentName = crtmodelInfo.nodeInfo.topID;
//            crtModelNode.isHidden = crtmodelInfo.nodeInfo.isHidden;
//            crtParentModelNode.childList.Add(crtModelNode);

//            foreach (Transform item in crtNode)
//            {
//                if (item.gameObject.activeSelf == true)
//                {
//                    nodes.Enqueue(item);
//                    modelNodes.Enqueue(crtModelNode);
//                }
//            }
//        }

//        return rootModelNode;
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
//                        GameManager.instance.StartCoroutine(LoadOldSaveModel(go.transform, modelName, nodeInfoArgs.rawModelName));
//                    }), this));

//            }
//        }
//    }

//    private IEnumerator LoadOldSaveModel(Transform tsf, string modelName, string rawmodelname)
//    {
//        InitModelByConfigsFile(tsf);

//        yield return null;


//        InitModelByConfigsFile2(tsf);


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
//        if (configArgs.AlignmentArgsInfo.ContainsKey(modelName))
//        {
//            foreach (var item in configArgs.AlignmentArgsInfo[modelName])
//            {
//                Transform temp = CommonMethed.GetTransformByNodePath(newGameObject.transform, item.Key);
//                temp.gameObject.AddComponent<CatchThing>().Init(item.Value);
//            }
//        }

//        if (configArgs.RobotArmArgsInfo.ContainsKey(modelName))
//        {
//            foreach (var item in configArgs.RobotArmArgsInfo[modelName])
//            {
//                Transform temp = CommonMethed.GetTransformByNodePath(newGameObject.transform, item.Key);
//                temp.gameObject.AddComponent<RobotRunController>().Init(item.Value);
//                temp.gameObject.GetComponent<RobotRunController>().InitRotate();
//            }
//        }

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

//            if (loadVersion == 1)
//            {
//                crtNode.transform.localPosition = new Vector3(crtNodeInfo.nodePos.pX, crtNodeInfo.nodePos.pY, crtNodeInfo.nodePos.pZ);
//            }
//            else
//            {
//                crtNode.transform.position = new Vector3(crtNodeInfo.nodePos.pX, crtNodeInfo.nodePos.pY, crtNodeInfo.nodePos.pZ);
//            }

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
//                if (crtNode.GetChild(i).gameObject.activeSelf == true && crtNode.GetChild(i).GetComponent<ModelInfo>() != null)
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

//            NodeInfoArgs crtNodeInfoArgs = crtNode.GetComponent<ModelInfo>().nodeInfo;
//            crtNodeInfoArgs.childNodes = new List<NodeInfoArgs>();
//            crtNodeInfoArgs.nodePos = new Positon3D() { pX = crtNode.position.x, pY = crtNode.position.y, pZ = crtNode.position.z };
//            crtNodeInfoArgs.nodeRot = new Rotate3D() { rX = crtNode.eulerAngles.x, rY = crtNode.eulerAngles.y, rZ = crtNode.eulerAngles.z };

//            foreach (Transform item in crtNode)
//            {
//                if (item.gameObject.activeSelf == true && item.GetComponent<ModelInfo>() != null)
//                {
//                    crtNodeInfoArgs.childNodes.Add(item.GetComponent<ModelInfo>().nodeInfo);
//                    nodes.Enqueue(item);
//                }
//            }
//        }
//    }

//    private void SaveMotion(SaveDataArgs saveDataArgs)
//    {
//        MotionVesselBase[] motionVessels = GameManager.instance.createRoot.GetComponentsInChildren<MotionVesselBase>();

//        foreach (var item in motionVessels)
//        {
//            MotionInfo mi = new MotionInfo();
//            mi.ID = item.GetComponent<ModelInfo>().nodeInfo.selfID;

//            if (item.GetType() != typeof(MotionVesselInstall))
//            {
//                mi.motionType = MotionType.Install;
//            }
//            else if (item.GetType() != typeof(MotionVesselDismantle))
//            {
//                mi.motionType = MotionType.Dismantle;
//            }
//            else if (item.GetType() != typeof(MotionVesselGenerate))
//            {
//                mi.motionType = MotionType.Generate;
//            }
//            else if (item.GetType() != typeof(MotionVesselRotate))
//            {
//                mi.motionType = MotionType.Rotate;
//            }
//            else if (item.GetType() != typeof(MotionVesselTransmit))
//            {
//                mi.motionType = MotionType.Transmit;
//            }
//            else if (item.GetType() != typeof(MotionVesselTranslation))
//            {
//                mi.motionType = MotionType.Translation;
//            }
//            else if (item.GetType() != typeof(MotionVesselOpenClose))
//            {
//                mi.motionType = MotionType.OpenClose;
//            }

//            saveDataArgs.motions.Add(mi);
//            saveDataArgs.resetInfos.Add(item.resetInfo);
//        }
//    }

//    private void SaveRobotArm(SaveDataArgs saveDataArgs)
//    {
//        RobotRunController[] robotRunController = GameManager.instance.createRoot.GetComponentsInChildren<RobotRunController>();

//        foreach (var item in robotRunController)
//        {
//            saveDataArgs.robotArmID.Add(item.GetComponent<ModelInfo>().nodeInfo.selfID);
//            saveDataArgs.robotArms.Add(item.robotArmArgs);
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

//            node.GetComponent<ModelInfo>().nodeInfo.nodePath = nodePath;
//            node.GetComponent<ModelInfo>().nodeInfo.rawModelName = modelName;
//            node.GetComponent<ModelInfo>().nodeInfo.modelName = node.name;
//            node.GetComponent<ModelInfo>().nodeInfo.changeModelName = node.name;
//            node.GetComponent<ModelInfo>().nodeInfo.rawNodeName = node.name;
//            node.GetComponent<ModelInfo>().nodeInfo.rawChild = new List<string>();
//            for (int i = 0; i < node.childCount; i++)
//            {
//                node.GetComponent<ModelInfo>().nodeInfo.rawChild.Add(node.GetChild(i).name);
//                nodes.Push(node.GetChild(i));
//                nodePaths.Push(nodePath + "|" + i);
//            }
//        }
//    }

//    //发送模型导入信号给WPF
//    private void SendModelImportSignal(Transform rootTsf)
//    {
//        ModelInfo modeInfo = rootTsf.GetComponent<ModelInfo>();
//        Vector3 position = rootTsf.position;
//        Positon3D positon3D = new Positon3D { pX = position.x, pY = position.y, pZ = position.z };
//        Vector3 rotation = rootTsf.eulerAngles;
//        Rotate3D rotate3D = new Rotate3D { rX = rotation.x, rY = rotation.y, rZ = rotation.z };

//        CommonMethed.UpdateNewModelInfo(rootTsf);
//        ModelNode nodeData = CommonMethed.GetModelNode(rootTsf);

//        SendSignalHelper.ModelImport(modeInfo.nodeInfo.topName, modeInfo.nodeInfo.selfID, modeInfo.nodeInfo.topID, positon3D, rotate3D, nodeData);
//    }

//    private IEnumerator LoadModel(Transform newTransform, string modelName, ImportMode importMode)
//    {
//        InitModelByConfigsFile(newTransform);
//        yield return null;

//        InitModelByConfigsFile2(newTransform);



//        if (configArgs.AlignmentArgsInfo.ContainsKey(newTransform.name))
//        {
//            foreach (var item in configArgs.AlignmentArgsInfo[newTransform.name])
//            {
//                Transform temp = CommonMethed.GetTransformByNodePath(newTransform.transform, item.Key);
//                temp.gameObject.AddComponent<CatchThing>().Init(item.Value);
//            }
//        }

//        if (configArgs.RobotArmArgsInfo.ContainsKey(newTransform.name))
//        {
//            foreach (var item in configArgs.RobotArmArgsInfo[newTransform.name])
//            {
//                Transform temp = CommonMethed.GetTransformByNodePath(newTransform.transform, item.Key);
//                temp.gameObject.AddComponent<RobotRunController>().Init(item.Value);
//                temp.gameObject.GetComponent<RobotRunController>().InitRotate();
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
//                }
//                break;
//            default:
//                {
//                    Debug.LogWarning("未进行判断的枚举类型" + importMode.ToString());
//                }
//                break;
//        }

//        NotificationCenter.Instance().PostDispatch(ActionType.RecordOperater, new Notification(new Tuple<List<Transform>, OperateType>(new List<Transform>() { newTransform }, OperateType.Import), this));

//    }
//    #endregion

//    #region 通用私有方法
//    private void InitModelByConfigsFile(Transform target)
//    {
//        target.name = target.name.Substring(0, target.name.Length - 7); //去掉尾部的"(Clone)"

//        Transform[] allChilds = target.GetComponentsInChildren<Transform>();

//        foreach (var item in allChilds)
//        {
//            if (item.gameObject.GetComponent<ModelInfo>() != null)
//            {
//                UnityEngine.Object.Destroy(item.gameObject.GetComponent<ModelInfo>());
//            }
//            if (item.gameObject.GetComponent<ClickableThing>() != null)
//            {
//                UnityEngine.Object.Destroy(item.gameObject.GetComponent<ClickableThing>());
//            }
//            if (item.gameObject.GetComponent<Highlighter>() != null)
//            {
//                UnityEngine.Object.Destroy(item.gameObject.GetComponent<Highlighter>());
//            }
//            if (item.gameObject.GetComponent<SceneObjHighlightController>() != null)
//            {
//                UnityEngine.Object.Destroy(item.gameObject.GetComponent<SceneObjHighlightController>());
//            }
//            if (item.gameObject.GetComponent<CatchThing>() != null)
//            {
//                UnityEngine.Object.Destroy(item.gameObject.GetComponent<CatchThing>());
//            }
//            if (item.gameObject.GetComponent<CatchThing>() != null)
//            {
//                UnityEngine.Object.Destroy(item.gameObject.GetComponent<CatchThing>());
//            }
//            if (item.gameObject.GetComponent<RobotRunController>() != null)
//            {
//                UnityEngine.Object.Destroy(item.gameObject.GetComponent<RobotRunController>());
//            }
//        }
//    }

//    private void InitModelByConfigsFile2(Transform target)
//    {
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
