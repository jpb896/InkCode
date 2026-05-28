using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using WinRT.Interop;

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
        string author = Environment.UserName;
        TextBox authorBox = new TextBox();

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
            string rtfContent;
            editor.Document.GetText(TextGetOptions.FormatRtf, out rtfContent);
            // Replace or insert the \generator tag
            // Pattern matches: {\*\generator ...;}
            string newGenerator = @"{\*\generator InkCode Dev 2.1}";
            if (Regex.IsMatch(rtfContent, @"\{\\\*\\generator.*?\}"))
            {
                rtfContent = Regex.Replace(rtfContent, @"\{\\\*\\generator.*?\}", newGenerator);
            }
            else
            {
                // Insert after the RTF header (\rtf1...)
                rtfContent = Regex.Replace(rtfContent, @"(\\rtf\d+)", "$1 " + newGenerator);
            }
            // Check if an author already exists, and remove it
            int authorIndex = rtfContent.IndexOf(@"{\author ");
            if (authorIndex >= 0)
            {
                int endIndex = rtfContent.IndexOf("}", authorIndex);
                rtfContent = rtfContent.Remove(authorIndex, endIndex - authorIndex + 1);
            }

            // Insert the author metadata after the initial {\rtf1
            int insertIndex = rtfContent.IndexOf(@"{\rtf1");
            int insertPos = rtfContent.IndexOf(" ", insertIndex) + 1; // after {\rtf1 
            string authorEntry = @"{\author " + author + "}";
            rtfContent = rtfContent.Insert(insertPos, authorEntry);
            Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            var hwnd = WindowNative.GetWindowHandle(App.window);
            InitializeWithWindow.Initialize(savePicker, hwnd);

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we
                // finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);

                // Write to file
                File.WriteAllText(file.Path, rtfContent);

                if (!diag_open)
                {
                    (VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.TabItems[VisualTreeHelperExtensions.FindParent<MainPage>(this).Tabs.SelectedIndex] as TabViewItem).Header = file.Name;
                }
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
            SuperscriptButton.IsChecked = editor.Document.Selection.CharacterFormat.Superscript == FormatEffect.On;
            SubscriptButton.IsChecked = editor.Document.Selection.CharacterFormat.Subscript == FormatEffect.On;

            LeftAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Left;
            CenterAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Center;
            RightAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Right;
            JustifyAlignButton.IsChecked = editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Justify;
        }

        private async void IncludeAuthorInformation(object sender, RoutedEventArgs e)
        {
            authorBox.PlaceholderText = author;
            ContentDialog authorDialog = new ContentDialog();
            authorDialog.Title = "Set document author";
            authorDialog.Content = authorBox;
            authorDialog.PrimaryButtonText = "Set author";
            authorDialog.PrimaryButtonClick += AuthorDialog_PrimaryButtonClick;
            authorDialog.PrimaryButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"];
            authorDialog.CloseButtonText = "Cancel";
            authorDialog.XamlRoot = this.XamlRoot;
            await authorDialog.ShowAsync();
        }

        private void AuthorDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (authorBox.Text != "")
            {
                author = authorBox.Text;
            }
        }

        private void SuperScriptButton_Click(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Superscript = FormatEffect.Toggle;
        }

        private void SubscriptButton_Click(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Subscript = FormatEffect.Toggle;
        }

        private async void InsertImage(object sender, RoutedEventArgs e)
        {
            // Open an image file.
            FileOpenPicker open = new(this.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".jpeg");

            PickFileResult picker = await open.PickSingleFileAsync();
            StorageFile file = await StorageFile.GetFileFromPathAsync(picker.Path);

            if (file != null)
            {
                using IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.Read);
                var properties = await file.Properties.GetImagePropertiesAsync();
                int width = (int)properties.Width;
                int height = (int)properties.Height;

                ImageOptionsDialog dialog = new()
                {
                    DefaultWidth = width,
                    DefaultHeight = height,
                    XamlRoot = this.XamlRoot
                };

                ContentDialogResult result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    editor.Document.Selection.InsertImage((int)dialog.DefaultWidth, (int)dialog.DefaultHeight, 0, VerticalCharacterAlignment.Baseline, string.IsNullOrWhiteSpace(dialog.Tag) ? "Image" : dialog.Tag, randAccStream);
                    return;
                }

                // Insert an image
                editor.Document.Selection.InsertImage(width, height, 0, VerticalCharacterAlignment.Baseline, "Image", randAccStream);

            }
        }
    }
}
