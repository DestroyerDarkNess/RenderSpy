using MinHook;
using RenderSpy.Interfaces;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Inputs
{
    public class DirectInputHook : IHook
    {

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int GetDeviceStateDelegate(IntPtr hDevice, int cbData, IntPtr lpvData);

        [StructLayout(LayoutKind.Sequential)]
        public struct LPDIMOUSESTATE
        {
            public int lX;
            public int lY;
            public int lZ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] rgbButtons;
        }


        IntPtr OrigAddr = IntPtr.Zero;
        HookEngine Engine;
        public GetDeviceStateDelegate Hook_orig;

        public event GetDeviceStateDelegate GetDeviceState;

        public bool BlockInput = false;

        private IntPtr Handle;

        public IntPtr WindowHandle   // property
        {
            get { return Handle; }   // get Handle
            set { Handle = value; }  // set Handle
        }

        public void Install()
        {
            if (Engine != null) { return; }
            Engine = new HookEngine();
            IntPtr GetDeviceStatePtr = GetVTableAdress(9);
            Hook_orig = Engine.CreateHook(GetDeviceStatePtr, new GetDeviceStateDelegate(GetDeviceStateHooked));
            Engine.EnableHooks();
        }


        private int GetDeviceStateHooked(IntPtr hDevice, int cbData, IntPtr lpvData)
        {
            if (GetDeviceState != null)
            {
                return GetDeviceState.Invoke(hDevice, cbData, lpvData);
            }
            else { return Hook_orig(hDevice, cbData, lpvData); }
        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }

        public IntPtr GetVTableAdress(int Funtion_Index)
        {
            IntPtr Result = IntPtr.Zero;
            var adapter = new DirectInput();
            var devices = adapter.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AttachedOnly);

            if (devices.Count <= 0)
                return IntPtr.Zero;

            using (var joystick = new Joystick(adapter, devices[0].InstanceGuid))
            {
                IntPtr vTable = joystick.NativePointer;
                IntPtr entry = RenderSpy.Globals.Helpers.GetVTblAddresses(vTable, Funtion_Index + 1)[Funtion_Index];

                Result = entry;
            }
            return Result;
        }

        public LPDIMOUSESTATE GetMouseData(IntPtr lpvData)
        {
            return Marshal.PtrToStructure<LPDIMOUSESTATE>(lpvData);
        }

    }
}
