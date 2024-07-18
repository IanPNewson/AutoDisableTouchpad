using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using Touchpad;

class Program
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    static void Main(string[] args)
    {
        while (true)
        {
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'x': Environment.Exit(0); break;
                case 'r':
                    Console.WriteLine($"Touchpad enabled: {TouchpadSettings.Enabled}");
                    break;
                case 'y':
                    TouchpadSettings.Enable();
                    break;
                case 'n':
                    TouchpadSettings.Disable();
                    break;
            }
        }
    }

    static void ToggleTouchpad(bool enable)
    {
        try
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PointingDevice");
            foreach (ManagementObject device in searcher.Get())
            {
                // Check if the device is a touchpad
                if (device["Description"].ToString().ToLower().Contains("touchpad"))
                {
                    device.InvokeMethod("Enable", null); // Enable method
                    if (!enable)
                    {
                        device.InvokeMethod("Disable", null); // Disable method
                    }
                }
            }
            Console.WriteLine($"Touchpad {(enable ? "enabled" : "disabled")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void T()
    {
        Thread.Sleep(5000);
        try
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
            {
                Console.WriteLine("No active window found.");
                return;
            }

            uint processId;
            GetWindowThreadProcessId(hwnd, out processId);

            Process process = Process.GetProcessById((int)processId);
            Console.WriteLine("Active Window Process Name: " + process.ProcessName);
            Console.WriteLine("Active Window Process ID: " + process.Id);

            var modules = process.Modules;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}
