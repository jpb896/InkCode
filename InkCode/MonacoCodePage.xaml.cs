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
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonacoCodePage : Page
    {
        public MonacoCodePage()
        {
            InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            if (ActualTheme == ElementTheme.Light)
            {
                monaco.EditorTheme = Monaco.EditorThemes.VisualStudioLight;
            } else {
                monaco.EditorTheme = Monaco.EditorThemes.VisualStudioDark;
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Paste(SplitButton sender, SplitButtonClickEventArgs args)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                monaco.PasteTextFromClipBoard(dataPackageView.GetTextAsync().ToString());
            }
        }

        private async void Paste2(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                monaco.PasteTextFromClipBoard(dataPackageView.GetTextAsync().ToString());
            }
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            monaco.CopyTextToClipBoard();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            monaco.CutTextToClipBoard();
        }
    }
}
