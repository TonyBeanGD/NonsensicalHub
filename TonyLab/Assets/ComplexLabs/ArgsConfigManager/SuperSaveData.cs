using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSaveData
{

    public int version;

    public List<NodeInfoArgs> topNodes = new List<NodeInfoArgs>();

    public Dictionary<string, Dictionary<string, List<string>>> SaveData = new Dictionary<string, Dictionary<string, List<string>>>();

}
