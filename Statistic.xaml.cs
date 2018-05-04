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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld
{
    
    public sealed partial class Statistic : Page
    {
        public Statistic()
        {
            this.InitializeComponent();
            GetChartData();
        }

        private void HyperlinkButton_Back_Stat(Object sender, RoutedEventArgs e)
        {
            TempDocument.Instance.PageNumber = 0;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void GetChartData()
        {
            (Line.Series[0] as LineSeries).ItemsSource = TempDocument.Instance.TempValues;
        }


    }
}
