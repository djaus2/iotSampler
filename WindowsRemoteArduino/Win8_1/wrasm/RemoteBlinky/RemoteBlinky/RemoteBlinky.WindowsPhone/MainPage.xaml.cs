/*using System;
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

namespace RemoteBlinky
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Usb is not supported on Win8.1. To see the USB connection steps, refer to the win10 solution instead.
        BluetoothSerial bluetooth;
        RemoteDevice arduino;

        private const int PB_PIN = 6;
        private const int LED_PIN = 5;
        private DispatcherTimer pbPolltimer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);




        public MainPage()
        {
            this.InitializeComponent();

           // 
           ////  * I've written my bluetooth device name as a parameter to the BluetoothSerial constructor. You should change this to your previously-paired
           //  * device name if using Bluetooth. You can also use the BluetoothSerial.listAvailableDevicesAsync() function to list
           //  available devices, but that is not covered in this sample.
           //  
            bluetooth = new BluetoothSerial("FireFly-748B");

            arduino = new RemoteDevice(bluetooth);
            bluetooth.ConnectionEstablished += OnConnectionEstablished;
            this.pbPolltimer = new DispatcherTimer();
            this.pbPolltimer.Interval = TimeSpan.FromMilliseconds(250);
            this.pbPolltimer.Tick += PBTimer_Tick;
            this.pbPolltimer.Stop();

            this.button.Fill = grayBrush;


        }

        

        private void OnConnectionEstablished()
        {
            //enable the buttons on the UI thread!
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() => {
                OnButton.IsEnabled = true;
                OffButton.IsEnabled = true;
                ConnectButton.IsEnabled = false;
                DisconnectButton.IsEnabled = true;

                arduino.pinMode(LED_PIN, PinMode.OUTPUT);
                arduino.pinMode(PB_PIN, PinMode.INPUT);
                //arduino.DigitalPinUpdatedEvent += Arduino_DigitalPinUpdated;
                this.pbPolltimer.Start();

            }));
        }

        PinState pbPinValue = PinState.LOW;

        private void PBTimer_Tick(object sender, object e)
        {
            PinState pbPinValueTemp = arduino.digitalRead(6);
            Pushbutton_Pressed(pbPinValueTemp);
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
                        this.button.Fill = redBrush;
                        break;
                    case PinState.LOW:
                        this.button.Fill = grayBrush;
                        break;
                }

            }
        }


        private async void Arduino_DigitalPinUpdated(byte pin, PinState pbPinValue)
        {
            if (pin == LED_PIN)
                return;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (pin == PB_PIN)
                {
                    Pushbutton_Pressed(pbPinValue);
                }
        
            });
        }




 
        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            //turn the LED connected to pin 5 ON
            arduino.digitalWrite(5, PinState.HIGH);

        }

        private void OffButton_Click(object sender, RoutedEventArgs e)
        {;
            //turn the LED connected to pin 5 OFF
            arduino.digitalWrite(5, PinState.LOW);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            //these parameters don't matter for bluetooth, except  SerialConfig.SERIAL_8N1
            bluetooth.begin(115200, SerialConfig.SERIAL_8N1);
            ConnectButton.IsEnabled = false;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            bluetooth.end();
            OnButton.IsEnabled = false;
            OffButton.IsEnabled = false;
            ConnectButton.IsEnabled = true;
            DisconnectButton.IsEnabled = false;

            this.pbPolltimer.Stop();

        }

    }
}
*/
