using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.Infrastructure;
using UnityEngine;
using NonsensicalKit;

namespace Assets.Sources.ViewModels
{
    public class TestViewModel: UGUIViewModelBase
    {
        public readonly BindableProperty<string> Color=new BindableProperty<string>();

        public TestViewModel()
        {
            MessageAggregator<object>.Instance.Subscribe("Toggle", ToggleHandler);
        }

        public void ToggleHandler(object o)
        {
            Debug.Log(o);
        }
    }
}
