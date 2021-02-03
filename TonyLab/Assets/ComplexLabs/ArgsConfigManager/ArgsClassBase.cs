using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract  class ArgsBase
{
    [HideInInspector]
    public string NodeID;
    public virtual ArgsBase Clone()
    {
        return null;
    }
}
