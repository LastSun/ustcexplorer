using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using mshtml;

namespace KBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly TabItem NewTab;
        ObservableCollection<TabItem> MyTabItems = new ObservableCollection<TabItem>();

        public MainWindow()
        {
            InitializeComponent();
            MyTab.ItemsSource = MyTabItems;
            NewTab = new TabItem();
            NewTab.MinWidth = 0;
			NewTab.Header = new TabHeader();
            var header = NewTab.Header as TabHeader;
            header.Favicon.Visibility = Visibility.Hidden;
            header.PageTitle.Content = "+++++++++++";
            header.Close.Visibility = Visibility.Hidden;
            header.Width = header.Height;
            var firsttab = new TabItem();
            firsttab.Header = new TabHeader();
            MyTabItems.Add(firsttab);
            MyTabItems.Add(NewTab);
        }

        private void MyTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TabItem tab in e.AddedItems)
            {
                if (tab == NewTab)
                {
                    MyTab.IsEnabled = false;
                    MyTabItems.RemoveAt(MyTabItems.Count - 1);
                    MyTabItems.Add(CreateNewPage());
                    MyTabItems.Add(NewTab);
                    MyTab.SelectedIndex = MyTabItems.Count - 2;
                    MyTab.IsEnabled = true;
                }
            }
        }

        IHTMLDocument2 GetPageDocument(TabItem tab)
        {
            try
            {
                return (tab.Content as WebBrowser).Document as IHTMLDocument2;
            }
            catch (NullReferenceException)
            {
            }
            return null;
        }

        string GetPageTitle(TabItem tab)
        {
            try
            {
                return GetPageDocument(tab).title;
            }
            catch (NullReferenceException)
            {
            }
            return "";
        }

        TabItem CreateNewPage()
        {
            TabItem tab = new TabItem();
            tab.Content = new WebBrowser();
            tab.Header = new TabHeader(GetPageTitle(tab));
            return tab;
        }
    }
}
