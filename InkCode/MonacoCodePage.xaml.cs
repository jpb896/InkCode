using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

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
            } 
            else 
            {
                monaco.EditorTheme = Monaco.EditorThemes.VisualStudioDark;
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
                picker.FileTypeFilter.Add("*");
                picker.FileTypeFilter.Add(".txt");
                picker.FileTypeFilter.Add(".cs");
                picker.FileTypeFilter.Add(".xaml");
                picker.FileTypeFilter.Add(".xml");
                picker.FileTypeFilter.Add(".cpp");
                picker.FileTypeFilter.Add(".cxx");
                picker.FileTypeFilter.Add(".html");
                picker.FileTypeFilter.Add(".js");
                picker.FileTypeFilter.Add(".yml");
                picker.FileTypeFilter.Add(".json");

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
                        if (monaco.EditorContent.Length > 0)
                        {
                            monaco.EditorContent = "";
                        }
                        monaco.EditorContent = streamReader.ReadToEnd();
                        switch (file.FileType)
                        {
                            case ".cs":
                                monaco.EditorLanguage = "csharp";
                                break;
                            case ".xml":
                                monaco.EditorLanguage = "xml";
                                break;
                            case ".xaml":
                                monaco.EditorLanguage = "xml";
                                break;
                            case ".cpp":
                                monaco.EditorLanguage = "cpp";
                                break;
                            case ".cxx":
                                monaco.EditorLanguage = "cpp";
                                break;
                            case ".html":
                                monaco.EditorLanguage = "html";
                                break;
                            case ".json":
                                monaco.EditorLanguage = "json";
                                break;
                            case ".yml":
                                monaco.EditorLanguage = "yaml";
                                break;
                            case ".js":
                                monaco.EditorLanguage = "javascript";
                                break;
                            case ".txt":
                                monaco.EditorLanguage = "plaintext";
                                break;
                            case ".rtf":
                                ContentDialog rtf_dialog = new ContentDialog();
                                rtf_dialog.Title = "Looking to edit an RTF file?";
                                rtf_dialog.Content = "If you want to edit an RTF file quickly and seamlessly using WYSIWYG tools, use the RTF editing mode of the app. To use it, open a new tab selecting 'New rich text document' from the new tab menu and opening the file from there.";
                                rtf_dialog.XamlRoot = this.XamlRoot;
                                rtf_dialog.IsPrimaryButtonEnabled = true;
                                rtf_dialog.PrimaryButtonText = "Understood!";
                                await rtf_dialog.ShowAsync();
                                monaco.EditorLanguage = "plaintext";
                                break;
                            default:
                                monaco.EditorLanguage = "plaintext";
                                break;
                        }

                        (VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.TabItems[VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.SelectedIndex] as TabViewItem).Header = file.Name;
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
                savePicker.FileTypeChoices.Add("All files", new List<string>() { "." });
                savePicker.FileTypeChoices.Add("Plain text", new List<string>() { ".txt" });
                savePicker.FileTypeChoices.Add("C#", new List<string>() { ".cs" });
                savePicker.FileTypeChoices.Add("Extensible (Application) Markup Language", new List<string>() { ".xml", ".xaml" });
                savePicker.FileTypeChoices.Add("C++", new List<string>() { ".cpp", ".cxx" });
                savePicker.FileTypeChoices.Add("HyperText Markup Language", new List<string>() { ".html" });
                savePicker.FileTypeChoices.Add("JavaScript", new List<string>() { ".js" });
                savePicker.FileTypeChoices.Add("YAML", new List<string>() { ".yml" });
                savePicker.FileTypeChoices.Add("JSON", new List<string>() { ".json" });

                // Show picker
                PickFileResult result = await savePicker.PickSaveFileAsync();

                if (result != null)
                {
                    // Convert PickSaveFileResult to StorageFile
                    StorageFile file = await StorageFile.GetFileFromPathAsync(result.Path);

                    (VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.TabItems[VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.SelectedIndex] as TabViewItem).Header = file.Name;

                    // Prevent updates to the remote version of the file until complete
                    CachedFileManager.DeferUpdates(file);

                    // Write content into the file
                    using IRandomAccessStream randAccStream =
                        await file.OpenAsync(FileAccessMode.ReadWrite);

                    var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(
    monaco.EditorContent, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
                    await randAccStream.WriteAsync(buffer);
                }
            }
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
