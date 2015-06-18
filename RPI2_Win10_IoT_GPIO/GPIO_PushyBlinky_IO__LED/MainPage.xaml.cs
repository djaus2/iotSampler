using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GPIO_PushyBlinky_IO_LED
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 5;
        private GpioPin pin;
        private GpioPinValue pinValue;
        private DispatcherTimer blinkTimer;


        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private bool bGpioStatus = false;

        public MainPage()
        {
            this.InitializeComponent();

            this.button.Fill = grayBrush;

            this.blinkTimer = new DispatcherTimer();
            this.blinkTimer.Interval = TimeSpan.FromMilliseconds(200);
            this.blinkTimer.Tick += BlimkTimer_Tick;
            this.blinkTimer.Stop();

            InitGPIO();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (this.image.Visibility == Visibility.Collapsed)
                this.image.Visibility = Visibility.Visible;
            else
                this.image.Visibility = Visibility.Collapsed;

            this.date_textblock.Text = DateTime.UtcNow.ToString();

            this.button.Fill = redBrush;
            if (bGpioStatus)
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }

            this.blinkTimer.Start();
        }

        private void BlimkTimer_Tick(object sender, object e)
        {
            this.button.Fill = grayBrush;
            this.blinkTimer.Stop();
            if (bGpioStatus)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
            }
        }



        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                bGpioStatus = false;
                return;
            }

            pin = gpio.OpenPin(LED_PIN);

            // Show an error if the pin wasn't initialized properly
            if (pin == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO pin.";
                bGpioStatus = false;
                return;
            }


            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);


            pinValue = GpioPinValue.Low;
            pin.Write(pinValue);


            GpioStatus.Text = "GPIO pin initialized correctly.";
            bGpioStatus = true;
        }
    }
}
