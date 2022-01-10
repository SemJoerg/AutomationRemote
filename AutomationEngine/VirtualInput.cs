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
        [DllImport("user32.dll")]
        public extern static bool GetCursorPos(ref Point pot);

        [DllImport("user32.dll")]
        public extern static void SetCursorPos(int x, int y);

        
    }
}
