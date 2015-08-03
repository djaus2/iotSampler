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


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GPIO_Pushy_RTM
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer blinkTimer;


        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            this.InitializeComponent();

            this.button.Fill = grayBrush;

            this.blinkTimer = new DispatcherTimer();
            this.blinkTimer.Interval = TimeSpan.FromMilliseconds(200);
            this.blinkTimer.Tick += BlinkTimer_Tick;
            this.blinkTimer.Stop();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (this.image.Visibility == Visibility.Collapsed)
                this.image.Visibility = Visibility.Visible;
            else
                this.image.Visibility = Visibility.Collapsed;

            this.date_textblock.Text = DateTime.UtcNow.ToString();

            this.button.Fill = redBrush;
 

            this.blinkTimer.Start();
        }

        private void BlinkTimer_Tick(object sender, object e)
        {
            this.button.Fill = grayBrush;
            this.blinkTimer.Stop();
        }
    }
}
