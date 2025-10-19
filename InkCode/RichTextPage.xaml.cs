using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Text;
using WinUIEditor;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RichTextPage : Page
    {
        public RichTextPage()
        {
            InitializeComponent();
        }

        private void Paste(SplitButton sender, SplitButtonClickEventArgs args)
        {
            editor.Document.Selection.Paste(0);
        }

        // This method is currently WIP. It does not work properly yet.
        private async void PasteNoFormat(object sender, RoutedEventArgs e)
        {
            string text;
            DataPackageView dataPackageView = Clipboard.GetContent();
            editor.Document.GetText(Microsoft.UI.Text.TextGetOptions.FormatRtf, out text);
            if (dataPackageView.Contains(StandardDataFormats.Text)) 
            {
                editor.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, text + dataPackageView.GetTextAsync());
            } 
            else
            {
                Debug.WriteLine("ERROR: Not pasting text");
            }
        }

        private void Paste2(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.Paste(0);
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.Copy();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.Cut();
        }
    }
}
