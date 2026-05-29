using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public static MainPage current;
        public bool diagOpen;
        public bool notCancelClicked;

        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Loaded += MainPage_Loaded;
            current = this;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var currentWindow = App.window;
            currentWindow.ExtendsContentIntoTitleBar = true;
            currentWindow.SetTitleBar(CustomDragRegion);
            currentWindow.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            currentWindow.AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            CustomDragRegion.MinWidth = 188;
        }

        private void AddTabButtonClick(TabView sender, object args)
        {
            ILocalizer localizer = Localizer.Get();
            MenuFlyoutItem newrtf = new();
            MenuFlyoutItem newcode = new();
            MenuFlyoutItem newcode_m = new();
            newrtf.Text = localizer.GetLocalizedString("NewRTF");
            newrtf.Click += Newrtf_Click;
            newcode.Text = localizer.GetLocalizedString("NewCode");
            newcode.Click += Newcode_Click;
            MenuFlyout menuFlyout = new();
            menuFlyout.Items.Add(newcode);
            menuFlyout.Items.Add(newrtf);
            menuFlyout.ShowAt(sender);
        }

        private void Newrtf_Click(object sender, RoutedEventArgs e)
        {
            ILocalizer localizer = Localizer.Get();
            var iconSource = new FontIconSource
            {
                Glyph = "\uE8A5"
            };
            var tab = new TabViewItem
            {
                Header = localizer.GetLocalizedString("Untitled"),
                IconSource = iconSource,
                Content = new RichTextPage()
            };
            Tabs.TabItems.Add(tab);
            if (Tabs.TabItems.Count == 1) { 
                Tabs.SelectedItem = Tabs.TabItems[0];
            }
        }

        private void Newcode_Click(object sender, RoutedEventArgs e)
        {
            ILocalizer localizer = Localizer.Get();
            var iconSource = new FontIconSource
            {
                Glyph = "\uE943"
            };
            var tab = new TabViewItem
            {
                Header = localizer.GetLocalizedString("Untitled"),
                IconSource = iconSource,
                Content = new CodePage()
            };
            Tabs.TabItems.Add(tab);
            if (Tabs.TabItems.Count == 1)
            {
                Tabs.SelectedItem = Tabs.TabItems[0];
            }
        }

        private async void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if ((Tabs.SelectedItem as TabViewItem).Content as RichTextPage != null)
            {
                await ((Tabs.SelectedItem as TabViewItem).Content as RichTextPage).ShowUnsavedDialog(args.Tab);
            }
            else
            {
                await ((Tabs.SelectedItem as TabViewItem).Content as CodePage).ShowUnsavedDialog(args.Tab);
            }
            if (notCancelClicked) {
                sender.TabItems.Remove(args.Tab);
            }
            notCancelClicked = false;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Frame? frame = this.Parent as Frame;
            frame.Navigate(typeof(SettingsPage));
        }
        private void SettingsButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(this.SettingsAnimatedIcon, "PointerOver");
        }

        private void SettingsButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(this.SettingsAnimatedIcon, "Normal");
        }
    }
}
