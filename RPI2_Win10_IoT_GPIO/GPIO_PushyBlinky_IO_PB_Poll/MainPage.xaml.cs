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

namespace GPIO_PushyBlinky_IO__PB_Poll_RTM
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

        private const int PB_PIN = 6;
        private GpioPin pbPin;     
        private GpioPinValue pbPinValue = GpioPinValue.Low;       
        private DispatcherTimer pbPolltimer;

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

            if (bGpioStatus)
            {
                //Poll the PB
                this.pbPolltimer = new DispatcherTimer();
                this.pbPolltimer.Interval = TimeSpan.FromMilliseconds(500);
                this.pbPolltimer.Tick += PBTimer_Tick;
                this.pbPolltimer.Start();
            }
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

        private void PBTimer_Tick(object sender, object e)
        {
            GpioPinValue pbPinValueTemp = pbPin.Read();
            if (pbPinValue != pbPinValueTemp)
            {
                //Pulse LED etc if new state is high.
                if (pbPinValueTemp == GpioPinValue.High)
                    button_Click(null, null);
                pbPinValue = pbPinValueTemp;
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

            pbPin = gpio.OpenPin(PB_PIN);

            // Show an error if the pin wasn't initialized properly
            if (pbPin == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO pbPin.";
                bGpioStatus = false;
                return;
            }

            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pbPin.SetDriveMode(GpioPinDriveMode.Input);
                   

            pinValue = GpioPinValue.Low;
            pin.Write(pinValue);


            GpioStatus.Text = "GPIO pin initialized correctly.";
            bGpioStatus = true;
        }
    }
}
