using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using WinUI_Desktop_MVVMBinding.Helpers;

namespace WinUI_Desktop_MVVMBinding.ViewModels
{
    public class MainViewModel : Observable
    {
        private string _name;

        private string _gender;

        private string _avatar;

        private string _input;

        private List<string> _descList;

        private ICommand _switchCommand;

        private ICommand _itemInvokedCommand;

        public MainViewModel()
        {
            _name = "GreenShade Zhang";

            _gender = "MAN";

            _avatar = "ms-appx:///Assets/Avatar.jpg";

            _input = "test";

            _descList = new List<string>
            {
                "一个好人",
                "小白码农",
                "努力变优秀"
            };
        }

        public ICommand SwitchCommand => _switchCommand ?? (_switchCommand = new RelayCommand<object>((param) =>
        {
            Name = "绿荫";

            Avatar = "ms-appx:///Assets/Avatar1.jpg";

            Gender = "男";
        }));

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<ItemClickEventArgs>(OnItemInvoked));

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(ref _name, value);
            }
        }

        public string Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                Set(ref _gender, value);
            }
        }
        public string Avatar
        {
            get
            {
                return _avatar;
            }
            set
            {
                Set(ref _avatar, value);
            }
        }
        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                Set(ref _input, value);
            }
        }

        public List<string> DescList
        {
            get
            {
                return _descList;
            }
            set
            {
                Set(ref _descList, value);
            }
        }

        private void OnItemInvoked(ItemClickEventArgs e)
        {
            string desc = e.ClickedItem as string;

            Name = desc;
        }
    }
}
