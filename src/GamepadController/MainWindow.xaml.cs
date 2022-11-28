using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GamepadController;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    Gamepad controller;

    DispatcherTimer dispatcherTimer;

    private string _selectPort = string.Empty;

    public SerialPort MyPort { get; set; } = new SerialPort();

    public MainWindow()
    {
        this.InitializeComponent();

        dispatcherTimer = new DispatcherTimer();

        dispatcherTimer.Interval = new TimeSpan(100);

        dispatcherTimer.Tick += dispatcherTimer_Tick;

        dispatcherTimer.Start();

        //public static event EventHandler<Gamepad> GamepadAdded
        Gamepad.GamepadAdded += Gamepad_GamepadAdded;
        //public static event EventHandler<Gamepad> GamepadRemoved
        Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
        //public event TypedEventHandler<IGameController, Headset> HeadsetConnected

        var list = SerialPort.GetPortNames();

        PortList.ItemsSource = list;
    }

    public string SelectPort
    {
        get
        {
            return _selectPort;
        }
    }

    private void Gamepad_GamepadAdded(object sender, Gamepad e)
    {
        e.HeadsetConnected += E_HeadsetConnected;
        e.HeadsetDisconnected += E_HeadsetDisconnected;
        e.UserChanged += E_UserChanged;
        Log("Gamepad Added");
    }

    private void Gamepad_GamepadRemoved(object sender, Gamepad e)
    {
        Log("Gamepad Removed");
    }
    private void E_UserChanged(IGameController sender, Windows.System.UserChangedEventArgs args)
    {
        Log("User changed");
    }

    private void E_HeadsetDisconnected(IGameController sender, Headset args)
    {
        Log("HeadsetDisconnected");
    }

    private void E_HeadsetConnected(IGameController sender, Headset args)
    {
        Log("HeadsetConnected");
    }

    private void dispatcherTimer_Tick(object sender, object e)
    {
        if (Gamepad.Gamepads.Count > 0)
        {
            controller = Gamepad.Gamepads.First();
            var reading = controller.GetCurrentReading();

            pbLeftThumbstickX.Value = reading.LeftThumbstickX;
            pbLeftThumbstickY.Value = reading.LeftThumbstickY;

            pbRightThumbstickX.Value = reading.RightThumbstickX;
            pbRightThumbstickY.Value = reading.RightThumbstickY;
            LeftX.Text = reading.LeftThumbstickX.ToString();
            LeftY.Text = reading.LeftThumbstickY.ToString();
            RightX.Text = reading.RightThumbstickX.ToString();
            RightY.Text = reading.RightThumbstickY.ToString();
            // pbRightThumbstickY.Value = reading.RightThumbstickY;

            pbLeftTrigger.Value = reading.LeftTrigger;
            pbRightTrigger.Value = reading.RightTrigger;
            pbLeft.Text = reading.LeftTrigger.ToString();
            pbRight.Text = reading.RightTrigger.ToString();
            //https://msdn.microsoft.com/en-us/library/windows/apps/windows.gaming.input.gamepadbuttons.aspx
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.A), lblA);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.B), lblB);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.X), lblX);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.Y), lblY);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.Menu), lblMenu);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadLeft), lblDPadLeft);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadRight), lblDPadRight);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadUp), lblDPadUp);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadDown), lblDPadDown);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.View), lblView);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.RightThumbstick), ellRightThumbstick);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.LeftThumbstick), ellLeftThumbstick);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.LeftShoulder), rectLeftShoulder);
            ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.RightShoulder), recRightShoulder);

            double duoji = 90;

            if (reading.LeftThumbstickX < 0 && reading.LeftThumbstickX >= -1)
            {
                duoji = 50 * System.Math.Abs(reading.LeftThumbstickX) + 90;
            }
            else if (reading.LeftThumbstickX > 0 && reading.LeftThumbstickX <= 1)
            {
                duoji = 90 - 30 * reading.LeftThumbstickX;
            }

            else
            {
                duoji = 90;
            }

            if (reading.Buttons.HasFlag(GamepadButtons.DPadLeft))
            {
                duoji = 90;
            }



            int a = (int)(((1600 - 1350) * reading.RightTrigger) + 1350);

            int b = (int)(1350 - ((1350 - 700) * reading.LeftTrigger));

            int duo = (int)duoji;

            //MyPort.WriteLine(String.Format("{0} {1}", duo, a));
            //if (a > 1350)
            //{

            //}
            //else
            //{
            //    //for (int i = 1350; i>800; i = i-2)
            //    //{
            //    //    MyPort.WriteLine(String.Format("{0} {1}", duo, i));
            //    //}

            //    //for (int i = 800; i < 1350; i = i +2)
            //    //{
            //    //    MyPort.WriteLine(String.Format("{0} {1}", duo, i));
            //    //}
            //    MyPort.WriteLine(String.Format("{0} {1}", duo, b));
            //}

        }

    }

    private void ChangeVisibility(bool flag, UIElement elem)
    {
        if (flag)
        {
            elem.Visibility = Visibility.Visible;
        }
        else
        {
            elem.Visibility = Visibility.Collapsed;
        }
    }

    private void Log(String txt)
    {
        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal,
        () =>
        {
            txtEvents.Text = DateTime.Now.ToString("hh:mm:ss.fff ") + txt + "\n" + txtEvents.Text;
        }
        );

    }

    private void PortList_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;

        var item = comboBox?.SelectedItem as string;

        _selectPort = item;
    }

    private void ConnectBtn_Checked(object sender, RoutedEventArgs e)
    {
        MyPort.BaudRate = 9600;

        MyPort.DataBits = 8;

        if (!string.IsNullOrEmpty(_selectPort))
        {
            MyPort.PortName = _selectPort;
            MyPort.NewLine = "\r\n";
            MyPort.Open();
            ConnectBtn.Content = "已连接";
        }
    }

    private void ConnectBtn_Unchecked(object sender, RoutedEventArgs e)
    {
        if (MyPort.IsOpen)
        {
            MyPort.Close();
            ConnectBtn.Content = "连接";
        }
    }
}
