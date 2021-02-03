using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit.UI
{
    public interface IData
    {
        List<IData> GetChilds();
    }

}