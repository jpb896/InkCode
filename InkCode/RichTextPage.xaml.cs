using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Provider;
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

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Create the picker using the AppWindowId from the element
                var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId)
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
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Create the picker with AppWindowId
                var savePicker = new FileSavePicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId)
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    SuggestedFileName = "Untitled"
                };

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

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

                    editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);

                    // Finalize file updates
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                    if (status != FileUpdateStatus.Complete)
                    {
                        var errorBox = new ContentDialog();
                        errorBox.Title = "Error";
                        errorBox.Content = $"File {file.Name} couldn't be saved.";
                        errorBox.PrimaryButtonText = "OK";
                        await errorBox.ShowAsync();
                    }
                }
            }
        }
    }
}
