using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseArgsClass
{
    ArgsBase GetArgs();
    void Init(ArgsBase args);
}
