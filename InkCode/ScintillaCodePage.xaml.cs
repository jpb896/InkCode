using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ScintillaCodePage : Page
    {
        public ScintillaCodePage()
        {
            InitializeComponent();
        }

        private void Paste(SplitButton sender, SplitButtonClickEventArgs args)
        {
            editor.Editor.Paste();
        }

        private void Paste2(object sender, RoutedEventArgs e)
        {
            editor.Editor.Paste();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            editor.Editor.Copy();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            editor.Editor.Cut();
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
                picker.FileTypeFilter.Add("*");
                picker.FileTypeFilter.Add(".txt");

                // Show picker
                PickFileResult result = await picker.PickSingleFileAsync();

                if (result != null)
                {
                    // Open with StorageFile (needed for RichEditBox)
                    StorageFile file = await StorageFile.GetFileFromPathAsync(result.Path);

                    using IRandomAccessStream randAccStream =
                        await file.OpenAsync(FileAccessMode.Read);

                    // Load file into the RichEditBox
                    using (StreamReader streamReader = new StreamReader(result.Path))
                    {
                        if (editor.Editor.Length > 0) { 
                            editor.Editor.ClearAll();
                        }
                        editor.Editor.AddText((long)randAccStream.Size, streamReader.ReadToEnd());
                        switch (file.FileType)
                        {
                            case ".cs":
                                editor.HighlightingLanguage = "csharp";
                                break;
                            case ".rtf":
                                ContentDialog rtf_dialog = new ContentDialog();
                                rtf_dialog.Title = "Looking to edit an RTF file?";
                                rtf_dialog.Content = "If you want to edit an RTF file quickly and seamlessly using WYSIWYG tools, use the RTF editing mode of the app, by opening a new tab having selected 'New rich text document' and opening the file from there.";
                                rtf_dialog.XamlRoot = this.XamlRoot;
                                rtf_dialog.IsPrimaryButtonEnabled = true;
                                rtf_dialog.PrimaryButtonText = "Understood!";
                                await rtf_dialog.ShowAsync();
                                break;
                            default:
                                break;
                        }
                    }
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
                savePicker.FileTypeChoices.Add("All files", new List<string>() { "" });
                savePicker.FileTypeChoices.Add("Plain text", new List<string>() { ".txt" });

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

                    Windows.Storage.Streams.Buffer buffer = new Windows.Storage.Streams.Buffer((uint)editor.Editor.Length);
                    editor.Editor.GetTextWriteBuffer(editor.Editor.Length, buffer);
                    await randAccStream.WriteAsync(buffer);

                    // Finalize file updates
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                    if (status != FileUpdateStatus.Complete)
                    {
                        var errorBox = new Windows.UI.Popups.MessageDialog(
                            $"File {file.Name} couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                }
            }
        }
    }
}
