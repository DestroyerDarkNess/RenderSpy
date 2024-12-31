using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace RenderSpy.Inputs
{
    public class GlobalWndProcHook : IDisposable
    {
        #region P/Invoke y Constantes

        private const int WH_CALLWNDPROC = 4;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        [StructLayout(LayoutKind.Sequential)]
        private struct CWPSTRUCT
        {
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        }

        #endregion

        private IntPtr _hookID = IntPtr.Zero;

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private HookProc _hookCallback;

        public event Globals.WindowProcDelegate WindowProc;

        public bool BlockInput = false;

        public IntPtr GameWindowHandle { get; set; }

        public void Install()
        {
            if (_hookID == IntPtr.Zero)
            {
                _hookCallback = HookCallback;

                uint processId;
                uint threadId = GetWindowThreadProcessId(GameWindowHandle, out processId);

                if (threadId == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not get the game thread ID.");
                }

                _hookID = SetWindowsHookEx(WH_CALLWNDPROC, _hookCallback, GetModuleHandle(IntPtr.Zero), threadId);

                if (_hookID == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to install global hook.");
                }
            }
        }

        public void Uninstall()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                CWPSTRUCT message = Marshal.PtrToStructure<CWPSTRUCT>(lParam);

                WindowProc?.Invoke(message.hwnd, message.message, message.wParam, message.lParam);

                if (BlockInput)
                {
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Uninstall();
        }
    }
}
