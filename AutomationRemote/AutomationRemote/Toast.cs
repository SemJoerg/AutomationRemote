using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRemote
{
    public interface Toast
    {
        void ShowShort(string message);
        void ShowLong(string message);
    }
}
