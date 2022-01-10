using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutomationEngine
{
    static public class VirtualInput
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;  //The left button was pressed

        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;  //The left button was released.

        private const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x08;   //The right button was pressed

        private const UInt32 MOUSEEVENTF_RIGHTUP = 0x10;   //The left button was released.

        private const UInt32 MOUSEEVENTF_MIDDLEDOWN = 0x0020;  //The middle button was pressed

        private const UInt32 MOUSEEVENTF_MIDDLEUP = 0x0040;  //The middle button was released.

        [DllImport("user32.dll")]
        static private extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        
        static public void LeftDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, new System.IntPtr());
        }

        static public void LeftUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, new System.IntPtr());
        }

        static public void RightDown()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, new System.IntPtr());
        }

        static public void RightUp()
        {
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, new System.IntPtr());
        }

        static public void MiddleDown()
        {
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, new System.IntPtr());
        }

        static public void MiddleUp()
        {
            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, new System.IntPtr());
        }

        static public void LeftClick()
        {
            LeftDown();
            LeftUp();
        }

        static public void RightClick()
        {
            RightDown();
            RightUp();
        }

        static public void MiddleClick()
        {
            MiddleDown();
            MiddleUp();
        }

        [DllImport("user32.dll")]
        static public extern bool GetCursorPos(ref Point pot);

        [DllImport("user32.dll")]
        static public extern void SetCursorPos(int x, int y);

        
    }
}
