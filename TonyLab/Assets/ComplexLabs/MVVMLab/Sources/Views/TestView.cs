using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.ViewModels;
using uMVVM.Sources.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using NonsensicalKit;

namespace Assets.Sources.Views
{
    public class TestView:UGUIViewBase<TestViewModel>
    {
        public Image buttonImage;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Binder.Add<string>("Color",OnColorPropertyValueChanged);
        }

        private void OnColorPropertyValueChanged(string oldValue, string newValue)
        {
            switch (newValue)
            {
                case "Red":
                    buttonImage.color = Color.red;
                    break;
                case "Yellow":
                    buttonImage.color = Color.yellow;
                    break;
                default:
                    break;
            }
        }
    }
}
