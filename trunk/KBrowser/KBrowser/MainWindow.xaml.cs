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
        static string[] prefix = new string[] { "http://", "file:///" };
        readonly TabItem NewTab;
        ObservableCollection<TabItem> MyTabItems = new ObservableCollection<TabItem>();
        int TabEditing;

        public MainWindow()
        {
            InitializeComponent();
            MyTab.ItemsSource = MyTabItems;
            NewTab = new TabItem();
            NewTab.MinWidth = 0;
			NewTab.Header = new TabHeader();
            var header = NewTab.Header as TabHeader;
            header.Favicon.Visibility = Visibility.Collapsed;
            header.PageTitle.Text = "+";
            header.Close.Visibility = Visibility.Collapsed;
            //header.Width = header.Height;
            MyTabItems.Add(CreateNewPage());
            MyTabItems.Add(NewTab);
        }

        private void MyTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabEditing == 0)
                foreach (TabItem tab in e.AddedItems)
                {
                    if (tab == NewTab)
                    {
                        ++TabEditing;
                        MyTab.IsEnabled = false;
                        MyTabItems.RemoveAt(MyTabItems.Count - 1);
                        MyTabItems.Add(CreateNewPage());
                        MyTabItems.Add(NewTab);
                        MyTab.SelectedIndex = MyTabItems.Count - 2;
                        MyTab.IsEnabled = true;
                        --TabEditing;
                    }
                }
            if (MyTab.SelectedIndex != -1)
            {
                var tab2 = MyTabItems[MyTab.SelectedIndex];
                LocationBar.Text = GetPageUrl(tab2);
                this.Title = GetWindowTitle(GetPageTitle(tab2));
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
            string st = "";
            try
            {
                st = GetPageDocument(tab).title;
            }
            catch (NullReferenceException)
            {
            }
            return st == "" ? "Blank Page" : st;
        }

        string GetPageUrl(TabItem tab)
        {
            try
            {
                return GetPageDocument(tab).url;
            }
            catch (NullReferenceException)
            {
            }
            return "";
        }

        string GetWindowTitle(string pagetitle)
        {
            string title = pagetitle;
            if (title != "")
                title += " - ";
            title += "USTC Explorer";
            return title;
        }

        TabItem CreateNewPage()
        {
            TabItem tab = new TabItem();
            tab.Content = new WebBrowser();
            (tab.Content as WebBrowser).LoadCompleted += new LoadCompletedEventHandler(Tab_LoadCompleted);
            tab.Header = new TabHeader(GetPageTitle(tab));
            (tab.Header as TabHeader).Close.Click += new RoutedEventHandler(Close_Click);
            return tab;
        }

        void Tab_LoadCompleted(object sender, NavigationEventArgs e)
        {
            /*
             * TabItem targettab = null;
            foreach (TabItem tab in MyTabItems)
            {
                try
                {
                    if ((tab.Content as WebBrowser) == sender)
                    {
                        targettab = tab;
                        break;
                    }
                }
                catch (NullReferenceException) { }
            }
            */

            TabItem targettab = (sender as WebBrowser).Parent as TabItem;
            try
            {

                if (targettab == MyTabItems[MyTab.SelectedIndex])
                {
                    LocationBar.Text = GetPageUrl(targettab);
                    this.Title = GetWindowTitle(GetPageTitle(targettab));
                }
                (targettab.Header as TabHeader).PageTitle.Text = GetPageTitle(targettab);
            }
            catch (NullReferenceException)
            { }
        }

        int GetTabOrderFromCloseButton(Button close)
        {
            return MyTabItems.IndexOf(((close.Parent as Grid).Parent as TabHeader).Parent as TabItem);
            /*
            for (int i = 0; i < MyTabItems.Count - 1; ++i)
            {
                try
                {
                    if (((MyTabItems[i] as TabItem).Header as TabHeader).Close == close)
                        return i;
                }
                catch (NullReferenceException)
                {
                }
            }
            return -1;
            */
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            ++TabEditing;
            int j = MyTab.SelectedIndex;
            int i = GetTabOrderFromCloseButton(sender as Button);
            MyTab.SelectedIndex = 0;
            MyTabItems.RemoveAt(i);
            if (MyTabItems.Count == 1)
                MyTabItems.Insert(0, CreateNewPage());
            if (i != j)
                MyTab.SelectedIndex = i > j ? j : j - 1;
            else
                MyTab.SelectedIndex = i - 1 > 0 ? i - 1 : 0;
            --TabEditing;
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (MyTabItems[MyTab.SelectedIndex].Content as WebBrowser).GoBack();
        }

        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = (MyTabItems[MyTab.SelectedIndex].Content as WebBrowser).CanGoBack;
            }
            catch (NullReferenceException)
            {
                e.CanExecute = false;
            }
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (MyTabItems[MyTab.SelectedIndex].Content as WebBrowser).GoForward();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = (MyTabItems[MyTab.SelectedIndex].Content as WebBrowser).CanGoForward;
            }
            catch (NullReferenceException)
            {
                e.CanExecute = false;
            }
        }

        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
            string url = LocationBar.Text.ToLower();
            if (url.Length >= 2 && url[1] == ':')
                url = prefix[1] + url;
            else
            {
                bool flag = false;
                for (int i = 0; i < prefix.Length; ++i)
                    if (url.Substring(0, Math.Min(prefix[i].Length, url.Length)) == prefix[i])
                    {
                        flag = true;
                        break;
                    }
                if (!flag)
                    url = prefix[0] + url;
            }

            try
            {
                (MyTabItems[MyTab.SelectedIndex].Content as WebBrowser).Navigate(url);
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Address format error!");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Address format error!");
            }
        }

        private void LocationBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                GoToPageButton.Command.Execute(null);
        }
    }
}
