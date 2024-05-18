using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Brightness
{
    internal class MonitorControl
    {
        private const uint MONITOR_DEFAULTTONULL = 0x00000000;
        private const uint MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        // PInvoke signature for MonitorFromWindow
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        // PInvoke signature for GetNumberOfPhysicalMonitorsFromHMONITOR
        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
            IntPtr hMonitor,
            out uint pdwNumberOfPhysicalMonitors
        );

        // PInvoke signature for GetPhysicalMonitorsFromHMONITOR
        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(
            IntPtr hMonitor,
            uint dwPhysicalMonitorArraySize,
            [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray
        );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        // PInvoke signature for DestroyPhysicalMonitor
        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

        // PInvoke signature for GetMonitorBrightness
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorBrightness(
            IntPtr hMonitor,
            out uint pdwMinimumBrightness,
            out uint pdwCurrentBrightness,
            out uint pdwMaximumBrightness
        );

        // PInvoke signature for SetMonitorBrightness
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetMonitorBrightness(
            IntPtr hMonitor,
            uint dwNewBrightness
        );


        public static uint? getBrightness(IntPtr hwnd)
        {
            IntPtr monitorHandle = MonitorFromWindow(hwnd, MONITOR_DEFAULTTOPRIMARY);
            if (monitorHandle != IntPtr.Zero && GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, out uint numberOfPhysicalMonitors))
            {
                PHYSICAL_MONITOR[] physicalMonitors = new PHYSICAL_MONITOR[numberOfPhysicalMonitors];
                if (GetPhysicalMonitorsFromHMONITOR(monitorHandle, numberOfPhysicalMonitors, physicalMonitors))
                {
                    foreach (var physicalMonitor in physicalMonitors)
                    {
                        if (GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, out uint minBrightness, out uint currentBrightness, out uint maxBrightness))
                        {
                            //Debug.WriteLine($"Monitor: {physicalMonitor.szPhysicalMonitorDescription}");
                            //Debug.WriteLine($"Min Brightness: {minBrightness}");
                            //Debug.WriteLine($"Current Brightness: {currentBrightness}");
                            //Debug.WriteLine($"Max Brightness: {maxBrightness}");
                            DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                            return currentBrightness;
                        }
                        else
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            Debug.WriteLine($"Failed to get monitor brightness. Error code: {errorCode}");
                        }
                        DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                    }
                }
                else
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to get physical monitors. Error code: {errorCode}");
                }
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                Debug.WriteLine($"Failed to get number of physical monitors or failed to get montior handle. Error code: {errorCode}");
            }
            return null;
        }

        public static void setBrightness(IntPtr hwnd, uint newBrightness)
        {
            IntPtr monitorHandle = MonitorFromWindow(hwnd, MONITOR_DEFAULTTOPRIMARY);
            if (monitorHandle != IntPtr.Zero && GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, out uint numberOfPhysicalMonitors))
            {
                PHYSICAL_MONITOR[] physicalMonitors = new PHYSICAL_MONITOR[numberOfPhysicalMonitors];
                if (GetPhysicalMonitorsFromHMONITOR(monitorHandle, numberOfPhysicalMonitors, physicalMonitors))
                {
                    foreach (var physicalMonitor in physicalMonitors)
                    {
                        if (SetMonitorBrightness(physicalMonitor.hPhysicalMonitor, newBrightness))
                        {
                            //Debug.WriteLine($"Successfully set brightness to {newBrightness} for monitor: {physicalMonitor.szPhysicalMonitorDescription}");
                        }
                        else
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            Debug.WriteLine($"Failed to set monitor brightness. Error code: {errorCode}");
                        }
                        DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                    }
                }
                else
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to get physical monitors. Error code: {errorCode}");
                }
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                Debug.WriteLine($"Failed to get number of physical monitors or failed to get montior handle. Error code: {errorCode}");
            }
        }
    }
}
