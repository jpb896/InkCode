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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComputeHashDialogContent : Page
    {
        public ComputeHashDialogContent()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (App.window.Content is MainPage tabPage)
            {
                if (tabPage.Tabs.SelectedItem is TabViewItem selectedTab)
                {
                    if (selectedTab.Content is Frame tabFrame)
                    {
                        if (tabFrame.Content is RichTextPage mainPage)
                        {
                            string docText = mainPage.docText;
                            docText = EncryptDecryptHashingHelpers.Base64Encode(docText);
                            base64_result.Text = docText;
                        }
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (App.window.Content is MainPage tabPage)
            {
                if (tabPage.Tabs.SelectedItem is TabViewItem selectedTab)
                {
                    if (selectedTab.Content is Frame tabFrame)
                    {
                        if (tabFrame.Content is RichTextPage mainPage)
                        {
                            string docText = mainPage.docText;
                            docText = EncryptDecryptHashingHelpers.Base64Decode(docText);
                            base64_result.Text = docText;
                        }
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (App.window.Content is MainPage tabPage)
            {
                if (tabPage.Tabs.SelectedItem is TabViewItem selectedTab)
                {
                    if (selectedTab.Content is Frame tabFrame)
                    {
                        if (tabFrame.Content is RichTextPage mainPage)
                        {
                            string docText = mainPage.docText;
                            docText = EncryptDecryptHashingHelpers.SHA1Encrypt(docText);
                            sha1_result.Text = docText;
                        }
                    }
                }
            }
        }
    }
}
