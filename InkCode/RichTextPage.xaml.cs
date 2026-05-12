using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class RichTextPage : Page
    {

        bool diag_open = false;

        public RichTextPage()
        {
            InitializeComponent();
        }

        private void Paste(SplitButton sender, SplitButtonClickEventArgs args)
        {
            editor.Document.Selection.Paste(0);
        }

        private void Paste2(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.Paste(0);
        }

        private void PasteNoFormat(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.Paste(13);
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
            editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }
        private void Italic(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        }
        private void Underline(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Underline = UnderlineType.Single;
        }
        private void Strikethrough(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Strikethrough = FormatEffect.Toggle;
        }

        private void LeftAlign(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        }
        private void CenterAlign(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
        }
        private void RightAlign(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
        }
        private void JustifyAlign(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
        }

        private void OnKeyboardAcceleratorInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            switch (sender.Key)
            {
                case Windows.System.VirtualKey.B:
                    editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
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
                case Windows.System.VirtualKey.O:
                    Open();
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.S:
                    Save();
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.L:
                    editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.E:
                    editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.R:
                    editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    args.Handled = true;
                    break;
                case Windows.System.VirtualKey.J:
                    editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
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
            editor?.Document.Selection.CharacterFormat.Size = int.Parse(fontSizeBox.Text);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Open();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Save();
            }
        }

        private async void Open()
        {
            // Create the picker using the AppWindowId from the element
            var picker = new FileOpenPicker(this.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            // Add file type filters
            picker.FileTypeFilter.Add(".rtf");

            // Show picker
            PickFileResult result = await picker.PickSingleFileAsync();

            if (result != null)
            {
                // Open with StorageFile (needed for RichEditBox)
                StorageFile file = await StorageFile.GetFileFromPathAsync(result.Path);
                (VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.TabItems[VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.SelectedIndex] as TabViewItem).Header = file.Name;

                using IRandomAccessStream randAccStream =
                    await file.OpenAsync(FileAccessMode.Read);

                // Load file into the RichEditBox
                editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream);
            }
        }

        private async void Save()
        {
            // Create the picker with AppWindowId
            var savePicker = new FileSavePicker(this.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "Untitled"
            };

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Rich Text", [".rtf"]);

            // Show picker
            PickFileResult result = await savePicker.PickSaveFileAsync();

            if (result != null)
            {
                // Convert PickSaveFileResult to StorageFile
                StorageFile file = await StorageFile.GetFileFromPathAsync(result.Path);

                // Prevent updates to the remote version of the file until complete
                CachedFileManager.DeferUpdates(file);

                // Write content into the file
                using IRandomAccessStream randAccStream =
                    await file.OpenAsync(FileAccessMode.ReadWrite);
                if (!diag_open)
                {
                    (VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.TabItems[VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.SelectedIndex] as TabViewItem).Header = file.Name;
                }

                editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
            }
        }

        private void ClearFormatting(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Off;
            editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Off;
            editor.Document.Selection.CharacterFormat.Underline = UnderlineType.None;
            editor.Document.Selection.CharacterFormat.Strikethrough = FormatEffect.Off;
        }

        public async Task ShowUnsavedDialog(TabViewItem tab)
        {
            string filename = (string)(tab as TabViewItem).Header;
            ContentDialog diag = new()
            {
                Title = filename + " has not been saved",
                Content = "Do you want to save your changes?",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save changes",
                SecondaryButtonText = "No",
                PrimaryButtonStyle = Resources["AccentButtonStyle"] as Style,
                XamlRoot = this.XamlRoot
            };
            diag.SecondaryButtonClick += Diag_CloseButtonClick;
            diag_open = true;

            ContentDialogResult result = await diag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                MainPage.current.notCancelClicked = true;
                Save();
            }
        }

        private void Diag_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MainPage.current.notCancelClicked = true;
            MainPage.current.diagOpen = false;
        }

        private void editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            BoldButton.IsChecked = editor.Document.Selection.CharacterFormat.Bold == FormatEffect.On;
            ItalicButton.IsChecked = editor.Document.Selection.CharacterFormat.Italic == FormatEffect.On;
            UnderlineButton.IsChecked = editor.Document.Selection.CharacterFormat.Underline != UnderlineType.None && editor.Document.Selection.CharacterFormat.Underline != UnderlineType.Undefined;
            StrikethroughButton.IsChecked = editor.Document.Selection.CharacterFormat.Strikethrough == FormatEffect.On;

            LeftAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Left;
            CenterAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Center;
            RightAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Right;
            JustifyAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Justify;
        }
    }
}
