using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigArgs
{
    public Dictionary<string, Dictionary<string,RobotArmArgs>> RobotArmArgsInfo = new Dictionary<string, Dictionary<string, RobotArmArgs>>();
}
