using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace WebBrowser
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly string[] prefix;
        ObservableCollection<TabItem> TabItems = new ObservableCollection<TabItem>();
        int TabEditing;
        TabItem NewTab = new TabItem();

        public MainWindow()
		{
			this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
			prefix = new String[] {"http://", "file:///"};
            Tab.ItemsSource = TabItems;
            NewTab.Header = "+";
            TabItems.Add(NewTab);
            AddPage();
        }

        public System.Windows.Controls.WebBrowser WebPage
        {
            get {
                if (Tab == null) return null;
                else return GetWebBrowser(Tab.SelectedIndex);
            }
        }

        public string GetWindowTitle(int i)
        {
            string title = GetWebTitle(i);
            if (title.Length != 0)
                title += " - ";
            title += "WebBroser by K.F.Storm";
            return title;
        }

        public string GetUrl(int i)
        {
            var webbrowser = GetWebBrowser(i);
            return webbrowser == null || webbrowser.Document == null || ((mshtml.IHTMLDocument2)webbrowser.Document).url == null ? "" : ((mshtml.IHTMLDocument2)webbrowser.Document).url;
        }

        public string GetCurrentUrl()
        {
            var webbrowser = GetWebBrowser(Tab.SelectedIndex);
            return webbrowser == null || webbrowser.Document == null || ((mshtml.IHTMLDocument2)webbrowser.Document).url == null ? "" : ((mshtml.IHTMLDocument2)webbrowser.Document).url;
        }

        public void AddPage()
        {
            ++TabEditing;
            TabItems.RemoveAt(TabItems.Count - 1);
            TabItems.Add(new TabItem());
            var RecentItem = TabItems[TabItems.Count - 1];
            RecentItem.Content = new System.Windows.Controls.WebBrowser();
            RecentItem.Header = GetWebTitle(TabItems.Count - 1);
            var RecentWebPage = RecentItem.Content as System.Windows.Controls.WebBrowser;
            RecentWebPage.Navigated += new System.Windows.Navigation.NavigatedEventHandler(RecentWebPage_Navigated);
            //if (((System.Windows.Controls.WebBrowser)RecentItem.Content).Document != null)
            //    RecentItem.Header = ((mshtml.IHTMLDocument2)((System.Windows.Controls.WebBrowser)RecentItem.Content).Document).title + " - WebBrowser by K.F.Storm";
            TabItems.Add(NewTab);
            --TabEditing;
        }

        public System.Windows.Controls.WebBrowser GetWebBrowser(int i)
        {
            if (i >= 0 && i < TabItems.Count && TabItems[i] != null && TabItems[i].Content != null)
                return (System.Windows.Controls.WebBrowser)TabItems[i].Content;
            else return null;
        }

        public string GetWebTitle(int i)
        {
            var webbrowser = GetWebBrowser(i);
            string title = webbrowser == null || webbrowser.Document == null || ((mshtml.IHTMLDocument2)webbrowser.Document).title == null ? "" : ((mshtml.IHTMLDocument2)webbrowser.Document).title;
            if (title == null || title.Length == 0)
                title = "Blank Page";
            return title;
        }

        public void RecentWebPage_Navigated(Object sender, NavigationEventArgs e)
        {
            var RecentWebPage = sender as System.Windows.Controls.WebBrowser;
            for (int i = 0; i < TabItems.Count; ++i)
                if (TabItems[i].Content == sender)
                {
                    TabItems[i].Header = GetWebTitle(i);
                    if (i == Tab.SelectedIndex)
                    {
                        this.Title = GetWindowTitle(i);
                        this.LocationBar.Text = GetUrl(i);
                    }
                    break;
                }
        }

        private void GoToPage(object sender, ExecutedRoutedEventArgs e)
        {
            string uri = this.LocationBar.Text.ToLower();
            if (uri == "blank")
                uri = prefix[0] + System.IO.Path.GetFullPath("blank.htm");
			if (uri.Length >= 2 && uri[1] == ':')
				uri = prefix[1] + uri;
			else
			{
				bool flag = false;
				for (int i = 0; i < prefix.Length; ++i)
					if (uri.Substring(0, Math.Min(prefix[i].Length, uri.Length)) == prefix[i])
					{
						flag = true;
						break;
					}
				if (!flag)
					uri = prefix[0] + uri;
			}

            try
            {
                WebPage.Navigate(uri);
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

        private void BrowseBack(object sender, ExecutedRoutedEventArgs e)
        {
            this.WebPage.GoBack();
        }

        private void BrowseBackCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WebPage != null && this.WebPage.CanGoBack;
        }

        private void BrowseForward(object sender, ExecutedRoutedEventArgs e)
        {
            this.WebPage.GoForward();
        }

        private void BrowseForwardCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WebPage != null && this.WebPage.CanGoForward;
        }

        private void LocationBarKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	if (e.Key == Key.Enter)
				NavigationCommands.GoToPage.Execute(null, null);
        }

        private void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabEditing > 0) return;
            ++TabEditing;
            if (Tab.SelectedIndex == TabItems.Count - 1)
            {
                AddPage();
                Tab.SelectedIndex = Tab.Items.Count - 2;
            }

            this.Title = GetWindowTitle(Tab.SelectedIndex);
            this.LocationBar.Text = GetUrl(Tab.SelectedIndex);
            --TabEditing;
        }

	}


}

