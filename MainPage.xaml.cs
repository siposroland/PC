// Copyright (c) Microsoft. All rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 



    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public static MainPage Instance
        {
            // This will return null when your current page is not a MainPage instance!
            get { return (Window.Current.Content as Frame).Content as MainPage; }
        }

        private void HyperlinkButton_Daily(object sender, RoutedEventArgs e)
        {
            TempDocument.Instance.PageNumber++;
            this.Frame.Navigate(typeof(DailyTemplate));
        }

        private void HyperlinkButton_Stat(object sender, RoutedEventArgs e)
        {
            TempDocument.Instance.PageNumber++;
            this.Frame.Navigate(typeof(Statistic));
        }

        public void UpdateMCP9809(double temp)
        {
            //TempDocument.Instance.SensorTemp.GetTemperatureValue();
            tbAverage.Text = temp.ToString() + "°C";
        }

    }
}
