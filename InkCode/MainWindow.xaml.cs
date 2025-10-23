using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            contentFrame.Content = new MainPage();
            MicaBackdrop micaAlt = new MicaBackdrop();
            micaAlt.Kind = MicaKind.BaseAlt;
            this.SystemBackdrop = micaAlt;
            this.AppWindow.SetIcon("inkcode.ico");
        }
    }
}
