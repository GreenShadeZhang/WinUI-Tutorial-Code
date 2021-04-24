using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI_Desktop
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _desc = ".Net Developer";

        private string _input = "test";
        public MainWindow()
        {
            this.InitializeComponent();

            Name = "GreenShade Zhang";

            Avatar = "ms-appx:///Assets/Avatar.jpg";

            Gender = "Man";
        }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public string Gender { get; set; }

        public string Desc
        {
            get
            {
                return _desc;
            }
            set
            {
                if (Desc != value)
                {
                    Set(ref _desc, value);
                }
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
                if (Input != value)
                {
                    Set(ref _input, value);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";

            Name = "绿荫";

            Avatar = "ms-appx:///Assets/Avatar.jpg";

            Gender = "男人";

            Desc = ".Net 开发人员";
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
