using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixerACv2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Text = "Test";
        }
    }
}
