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
using Microsoft.Maker.Serial;
using Microsoft.Maker.RemoteWiring;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace wrauwp
{

    public sealed partial class MainPage : Page
    {
        //Usb is not supported on Win8.1. To see the USB connection steps, refer to the win10 solution instead.
        BluetoothSerial bluetooth;
        RemoteDevice arduino;

        // The pins used. Note: Actual pinn numbers.
        private const int PB_PIN = 6;
        private const int LED_PIN = 5;
        private const int ANALOG_PIN = 14;

        //Dimmer:
        private const int PWM_PIN = 10;

        // In Poll mode timer ticks sample the inputs
        //private DispatcherTimer pbPolltimer;

        // Colors for ellipse when hardware pushbutton is pressed/not pressed
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);



        public MainPage()
        {
            this.InitializeComponent();

            /*
             * I've written my bluetooth device name as a parameter to the BluetoothSerial constructor. You should change this to your previously-paired
             * device name if using Bluetooth. You can also use the BluetoothSerial.listAvailableDevicesAsync() function to list
             * available devices, but that is not covered in this sample.
             */
            bluetooth = new BluetoothSerial("FireFly-748B");

            arduino = new RemoteDevice(bluetooth);
            bluetooth.ConnectionEstablished += OnConnectionEstablished;

            //this.pbPolltimer = new DispatcherTimer();
            //this.pbPolltimer.Interval = TimeSpan.FromMilliseconds(250);
            //this.pbPolltimer.Tick += PBTimer_Tick;
            //this.pbPolltimer.Stop();

            //Start with off color for ellipse
            this.PBStatusLED.Fill = grayBrush;


        }



        private void OnConnectionEstablished()
        {
            //enable the buttons on the UI thread!
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() => {
                this.OnButton.IsEnabled = true;
                this.OffButton.IsEnabled = true;
                this.ConnectButton.IsEnabled = false;
                this.DisconnectButton.IsEnabled = true;

                arduino.pinMode(LED_PIN, PinMode.OUTPUT);
                arduino.pinMode(PB_PIN, PinMode.INPUT);

                // Note: Need actual pin number, not analog ibndex:
                arduino.pinMode(ANALOG_PIN, PinMode.ANALOG);


                arduino.AnalogPinUpdatedEvent += Arduino_AnalogPinUpdated;
                arduino.DigitalPinUpdatedEvent += Arduino_DigitalPinUpdated;

                //this.pbPolltimer.Start();

                //BT is connected so turn off progress ring
                this.progress1.IsActive = false;
                this.progress1.Visibility = Visibility.Collapsed;

                //Dimmer
                arduino.pinMode(PWM_PIN, PinMode.PWM);

            }));
        }



        PinState pbPinValue = PinState.LOW;

        private void PBTimer_Tick(object sender, object e)
        {
            PinState pbPinValueTemp = arduino.digitalRead(6);
            Pushbutton_Pressed(pbPinValueTemp);

            //Note: Analog Read Pin number is the analog index
            int PinValue = arduino.analogRead(ANALOG_PIN - 14);
            this.analogBar.Value = PinValue;
        }

        private void Pushbutton_Pressed(PinState pbPinValueTemp)
        {
            if (pbPinValue != pbPinValueTemp)
            {
                //Write value if changed
                TxtPin6.Text = "Pushbutton: " + pbPinValueTemp.ToString();
                pbPinValue = pbPinValueTemp;

                switch (pbPinValue)
                {
                    case PinState.HIGH:
                        this.PBStatusLED.Fill = redBrush;
                        break;
                    case PinState.LOW:
                        this.PBStatusLED.Fill = grayBrush;
                        break;
                }

            }
        }


        private async void Arduino_DigitalPinUpdated(byte pin, PinState pinValue)
        {
            if (pin == LED_PIN)
            {
                switch (pinValue)
                {
                    case PinState.HIGH:
                        this.OffButton.IsEnabled = true;
                        this.OnButton.IsEnabled = false;
                        break;
                    case PinState.LOW:
                        this.OffButton.IsEnabled = false;
                        this.OnButton.IsEnabled = true;
                        break;
                }
                return;
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (pin == PB_PIN)
                {
                    Pushbutton_Pressed(pinValue);
                }

            });
        }

        private async void Arduino_AnalogPinUpdated(byte pin, ushort PinValue)
        {
            //Note: Pin number is the analog index
            if (pin == ANALOG_PIN - 14)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.analogBar.Value = PinValue;
                });
            }
        }





        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            //turn the LED connected to pin 5 ON
            arduino.digitalWrite(5, PinState.HIGH);
            this.OffButton.IsEnabled = true;
            this.OnButton.IsEnabled = false;
        }

        private void OffButton_Click(object sender, RoutedEventArgs e)
        {
            //turn the LED connected to pin 5 OFF
            arduino.digitalWrite(5, PinState.LOW);
            this.OffButton.IsEnabled = false;
            this.OnButton.IsEnabled = true;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            //these parameters don't matter for bluetooth- Arduino Firmata, except  SerialConfig.SERIAL_8N1
            bluetooth.begin(115200, SerialConfig.SERIAL_8N1);
            this.ConnectButton.IsEnabled = false;


            //Connecting BT so show progress ring
            this.progress1.IsActive = true;
            this.progress1.Visibility = Visibility.Visible;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            bluetooth.end();
            this.OnButton.IsEnabled = false;
            this.OffButton.IsEnabled = false;
            this.ConnectButton.IsEnabled = true;
            this.DisconnectButton.IsEnabled = false;


            //this.pbPolltimer.Stop();
        }

        /// <summary>
        /// Dimmer:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            byte val = (byte) slider.Value;
            arduino.analogWrite(PWM_PIN, val);
        }
    }
}

