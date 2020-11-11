using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.Models;
using NonsensicalKit;

namespace Assets.Sources.ViewModels
{
    public class BadgeViewModel: UGUIViewModelBase
    {   
        public readonly BindableProperty<string> Icon=new BindableProperty<string>();
        public readonly BindableProperty<string> ElementColor = new BindableProperty<string>();
        public void Initialization(Badge badge)
        {
            Icon.Value = badge.Icon;
            ElementColor.Value = badge.ElementColor;
        }
    }
}
