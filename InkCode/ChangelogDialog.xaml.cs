using Microsoft.UI.Xaml.Controls;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangelogDialog : Page
    {
        public ChangelogDialog()
        {
            InitializeComponent();
            changelogBox.Text = File.ReadAllText("Assets\\changelog.md");
        }
    }
}
