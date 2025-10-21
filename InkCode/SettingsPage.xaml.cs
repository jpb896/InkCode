using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Devices;

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
                SpatialSwitch.IsEnabled = false;
            }
        }

        private void SoundSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (SoundSwitch.IsOn) {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
            }
        }

        private void FocusSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (FocusSwitch.IsOn) { 
                App.Current.FocusVisualKind = FocusVisualKind.Reveal;
            } else
            {
                App.Current.FocusVisualKind = FocusVisualKind.HighVisibility;
            }
        }
    }
}
