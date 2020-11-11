using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.Core.Infrastructure;
using Assets.Sources.Infrastructure;
using Assets.Sources.ViewModels;
using NonsensicalKit;
using UnityEngine;
using UnityEngine.UI;
using NonsensicalKit;


namespace Assets.Sources.Views
{
    public class BadgeView:ViewBase<BadgeViewModel>
    {
        public Image iconImage;
        public Image elementColorImage;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Binder.Add<string>("Icon",OnIconPropertyValueChanged);
            Binder.Add<string>("ElementColor",OnElementColorPropertyValueChanged);
        }

        private void OnElementColorPropertyValueChanged(string oldvalue, string newvalue)
        {
            elementColorImage.color = HexConverter.HexToColor(newvalue);
        }

        private void OnIconPropertyValueChanged(string oldValue, string newValue)
        {
            var settings = GameObject.Find("UICamera").GetComponent<GlobalSettings>();
            var field = typeof(GlobalSettings).GetField(newValue);
            iconImage.sprite = field.GetValue(settings) as Sprite;
        }
    }
}
