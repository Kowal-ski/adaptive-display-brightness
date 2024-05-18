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
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Auto_Brightness
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(450, 300));

            brightnessSlider_Setup();
        }

        private void brightnessSlider_Setup()
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            if (hwnd == IntPtr.Zero) { throw new InvalidOperationException("Failed to obtain the window handle."); }

            uint? currentBrightness = MonitorControl.getBrightness(hwnd);
            if (currentBrightness.HasValue)
            {
                this.brightnessSlider.Value = currentBrightness.Value;
                this.brightnessSlider.IsEnabled = true;
            }
            else
            {
                this.brightnessSlider.IsEnabled = false;
            }
        }

        private void brightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            if (hwnd == IntPtr.Zero) { throw new InvalidOperationException("Failed to obtain the window handle."); }

            uint newBrightness = (uint)e.NewValue;
            MonitorControl.setBrightness(hwnd, newBrightness);
        }
    }
}
