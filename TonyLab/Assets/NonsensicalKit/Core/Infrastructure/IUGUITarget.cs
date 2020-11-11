using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IUGUITarget
{
    void Reveal(bool immediate = false, Action action = null);
    void Hide(bool immediate = false, Action action = null);
}
