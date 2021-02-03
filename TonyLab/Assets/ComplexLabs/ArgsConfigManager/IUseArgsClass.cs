using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseArgsClass<T> where T : ArgsBase
{
    T GetArgs();
    void Init(T args);

    //T Args { get; set; }
}
