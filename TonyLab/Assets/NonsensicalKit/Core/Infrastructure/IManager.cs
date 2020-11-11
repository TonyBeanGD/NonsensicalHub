using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager
{
    void OnInit();
    void OnLateInit();

    bool InitComplete { get; }
    bool LateInitComplete { get; }
}
