using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var currentWindow = App.window;
            currentWindow.ExtendsContentIntoTitleBar = true;
            currentWindow.SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;
        }

        private void AddTabButtonClick(TabView sender, object args)
        {
            MenuFlyoutItem newrtf = new MenuFlyoutItem();
            MenuFlyoutItem newcode = new MenuFlyoutItem();
            MenuFlyoutItem newcode_m = new MenuFlyoutItem();
            newrtf.Text = "New rich text document";
            newrtf.Click += Newrtf_Click;
            newcode.Text = "New code file (Scintilla/WinUIEdit)";
            newcode.Click += Newcode_Click;
            newcode_m.Text = "New code file (Monaco)";
            newcode_m.Click += NewcodeM_Click;
            MenuFlyout menuFlyout = new MenuFlyout();
            menuFlyout.Items.Add(newcode);
            menuFlyout.Items.Add(newcode_m);
            menuFlyout.Items.Add(new MenuFlyoutSeparator());
            menuFlyout.Items.Add(newrtf);
            menuFlyout.ShowAt(sender);
        }

        private void Newrtf_Click(object sender, RoutedEventArgs e)
        {
            var iconSource = new FontIconSource();
            iconSource.Glyph = "\uE8A5";
            var tab = new TabViewItem();
            tab.Header = "Untitled";
            tab.IconSource = iconSource;
            tab.Content = new RichTextPage();
            Tabs.TabItems.Add(tab);
        }

        private void Newcode_Click(object sender, RoutedEventArgs e)
        {
            var iconSource = new FontIconSource();
            iconSource.Glyph = "\uE943";
            var tab = new TabViewItem();
            tab.Header = "Untitled";
            tab.IconSource = iconSource;
            tab.Content = new ScintillaCodePage();
            Tabs.TabItems.Add(tab);
        }

        private void NewcodeM_Click(object sender, RoutedEventArgs e)
        {
            var iconSource = new FontIconSource();
            iconSource.Glyph = "\uE943";
            var tab = new TabViewItem();
            tab.Header = "Untitled";
            tab.IconSource = iconSource;
            tab.Content = new MonacoCodePage();
            Tabs.TabItems.Add(tab);
        }

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }
    }
}
