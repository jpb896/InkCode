using CommunityToolkit.Mvvm.Input;
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
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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

        public List<int> FontSizes = new List<int> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 36, 48, 72, 96 };

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

        private void Bold(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Bold = Microsoft.UI.Text.FormatEffect.Toggle;
        }
        private void Italic(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Italic = Microsoft.UI.Text.FormatEffect.Toggle;
        }
        private void Underline(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Underline = Microsoft.UI.Text.UnderlineType.Single;
        }
        private void Strikethrough(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Strikethrough = Microsoft.UI.Text.FormatEffect.Toggle;
        }

        private void OnKeyboardAcceleratorInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            switch (sender.Key)
            {
                case Windows.System.VirtualKey.B:
                    editor.Document.Selection.CharacterFormat.Bold = Microsoft.UI.Text.FormatEffect.Toggle;
                    BoldButton.IsChecked = editor.Document.Selection.CharacterFormat.Bold == FormatEffect.On;
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.I:
                    editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
                    ItalicButton.IsChecked = editor.Document.Selection.CharacterFormat.Italic == FormatEffect.On;
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.U:
                    editor.Document.Selection.CharacterFormat.Underline = UnderlineType.Single;
                    UnderlineButton.IsChecked = editor.Document.Selection.CharacterFormat.Underline == UnderlineType.Single;
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.T:
                    editor.Document.Selection.CharacterFormat.Strikethrough = FormatEffect.Toggle;
                    StrikethroughButton.IsChecked = editor.Document.Selection.CharacterFormat.Strikethrough == FormatEffect.On;
                    args.Handled = true;
                    break;
            }
        }

        private void fontBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Name = fontBox.SelectedValue.ToString();
        }

        private void fontSizeBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (editor != null)
            {
                editor.Document.Selection.CharacterFormat.Size = int.Parse(fontSizeBox.Text);
            }
        }
    }
}
