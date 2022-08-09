using System;
using System.Runtime.InteropServices;

namespace MapExanima
{



    static class WinAPI
    {
        public static int GWL_STYLE = -16;
        public static int WS_BORDER = 0x00800000; //window with border
        public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar
        public static int WS_SYSMENU = 0x00080000;      //window with no borders etc.
        public static int WS_MAXIMIZEBOX = 0x00010000;
        public static int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox
        public static uint WS_SIZEBOX = 0x00040000;
        public static int WM_EXITSIZEMOVE = 0x0232;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
          IntPtr hProcess,
          uint lpBaseAddress,
          byte[] lpBuffer,
          int nSize,
          IntPtr lpNumberOfBytesRead);
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);
    }
}
