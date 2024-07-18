using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Touchpad
{
    public class TouchpadSettings
    {

        private static string KeyPath
        {
            get
            {
                const string userRoot = "HKEY_CURRENT_USER";
                const string subkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PrecisionTouchPad\\Status";
                return userRoot + "\\" + subkey;
            }
        }

        private static string KeyName { get => "Enabled"; }

        private static int? KeyValue
        {
            get
            {
                return (int?)Registry.GetValue(KeyPath, KeyName, null);
            }
            set
            {
                Registry.SetValue(KeyPath, KeyName, value);
            }
        }

        public static bool Enabled
        {
            get => KeyValue == 1;
            private set
            {
                KeyValue = value ? 1 : 0;
            }
        }

        public static void Enable()
        {
            Enabled = true;
            BroadcastChange();
        }

        public static void Disable()
        {
            Enabled = false;
            BroadcastChange();
        }

        // Constants
        private const int HWND_BROADCAST = 0xFFFF;
        private const uint WM_SETTINGCHANGE = 0x1A;
        private const int MSG_TIMEOUT = 15000;

        // Enum for SendMessageTimeout flags
        [Flags]
        private enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008,
            SMTO_ERRORONEXIT = 0x0020
        }

        // PInvoke declaration
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);

        private static void BroadcastChange()
        {
            IntPtr hwndBroadcast = new IntPtr(HWND_BROADCAST);
            string environment = "Environment";

            // Allocate memory for the string parameter
            IntPtr lParam = Marshal.StringToHGlobalAuto(environment);

            try
            {
                UIntPtr result;
                var returnCode = SendMessageTimeout(
                    hwndBroadcast,
                    WM_SETTINGCHANGE,
                    UIntPtr.Zero,
                    lParam,
                    SendMessageTimeoutFlags.SMTO_ABORTIFHUNG,
                    MSG_TIMEOUT,
                    out result);

                Console.WriteLine($"Broadcast message sent successfully: {result}, {returnCode}");
            }
            finally
            {
                // Free the allocated memory
                Marshal.FreeHGlobal(lParam);
            }
        }

    }
}
