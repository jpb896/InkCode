using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InkCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
            {
                SoundSwitch.IsOn = true;
            } 
            else 
            { 
                SoundSwitch.IsOn = false; 
                SpatialSwitch.IsEnabled = false; 
                SpatialSwitch.IsOn = false; 
            }
            if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On) 
            {
                SpatialSwitch.IsOn = true;
            } 
            else 
            { 
                SpatialSwitch.IsOn = false;
            }
            if (App.Current.FocusVisualKind == FocusVisualKind.Reveal)
            { 
                FocusSwitch.IsOn = true;
            } 
            else 
            {  
                FocusSwitch.IsOn = false;
            }
            BuildDateTextBlock.Text = "Built " + GetBuildDate(Assembly.GetExecutingAssembly()).ToString();
        }

        private void TitleBar_BackRequested(TitleBar sender, object args)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void SpatialSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (SoundSwitch.IsOn)
            {
                if (SpatialSwitch.IsOn)
                {
                    ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
                }
                else
                {
                    ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
                }
            }
            else
            {
                SpatialSwitch.IsOn = false;
                SpatialSwitch.IsEnabled = false;
            }
        }

        private void SoundSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (SoundSwitch.IsOn) 
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else 
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
            }
        }

        private void FocusSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (FocusSwitch.IsOn) 
            { 
                App.Current.FocusVisualKind = FocusVisualKind.Reveal;
            } 
            else
            {
                App.Current.FocusVisualKind = FocusVisualKind.HighVisibility;
            }
        }

        private static DateTime GetBuildDate(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute != null ? attribute.DateTime : default(DateTime);
        }
    }
}
