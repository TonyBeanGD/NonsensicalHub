
using System;
using uMVVM.Sources.Models;
using Debug = UnityEngine.Debug;
using NonsensicalKit;

namespace uMVVM.Sources.ViewModels
{
    public class SetupViewModel: UGUIViewModelBase
    {
        public readonly BindableProperty<string> Name = new BindableProperty<string>();
        public readonly BindableProperty<string> Job=new BindableProperty<string>(); 
        public readonly BindableProperty<int> ATK = new BindableProperty<int>();
        public readonly BindableProperty<float> SuccessRate=new BindableProperty<float>(); 
        public readonly BindableProperty<State> State=new BindableProperty<State>();

        public void JoininCurrentTeam()
        {
           //MessageAggregator<object>.Instance.Publish("Toggle", this, new MessageArgs<object>("Red"));

            Debug.Log(Name.Value + "加入当前Team，职业：" + Job.Value + ",攻击力：" + ATK.Value + "成功率：" + SuccessRate.Value);
        }

        public void JoininClan()
        {
            //MessageAggregator<object>.Instance.Publish("Toggle", this, new MessageArgs<object>("Yellow"));
            Debug.Log(Name.Value + "加入当前Clan，职业：" + Job.Value + ",攻击力：" + ATK.Value + "成功率：" + SuccessRate.Value);
        }
    }
}
