using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDataArgs
{
    public int version;
    public List<NodeInfoArgs> topNodes = new List<NodeInfoArgs>();
    
    public List<string> robotArmID = new List<string>();
    public List<RobotArmArgs> robotArms = new List<RobotArmArgs>();
}
