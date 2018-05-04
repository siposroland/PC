using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DailyTemplate : Page
    {
        public DailyTemplate()
        {
            this.InitializeComponent();
        }

        private void HyperlinkButton_Back_Daily(Object sender, RoutedEventArgs e)
        {
            TempDocument.Instance.PageNumber = 0;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            string msg = "";
            if (e != null)
            {
                msg = String.Format("Hőmérsékleti érték: {0}°C", e.NewValue);
                this.actTemp.Text = msg;
            }   
        }

        private ObservableCollection<string> _items = new ObservableCollection<string>();

        public ObservableCollection<string> Items
        {
            get { return this._items; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Instead of hard coded items, the data could be pulled 
            // asynchronously from a database or the internet.
            for (int i = 0; i < 6; i=i+2)
            {
                Items.Add(i.ToString());
            }
            
        }

        private void Delete_Element(object sender, SelectionChangedEventArgs e)
        {
            if (itemListView.SelectedItem != null)
            {
                System.Diagnostics.Debug.WriteLine((itemListView.SelectedItem.ToString()));
                //itemListView.Items[itemListView.SelectedIndex];
            }
        }


    }
}
