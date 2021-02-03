using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Assets.Sources.Core.Factory
{
    public interface IObjectFactory
    {
        object AcquireObject(string className);
        object AcquireObject(Type type);
        object AcquireObject<TInstance>() where TInstance : class, new();
        void ReleaseObject(object obj);
    }
}
