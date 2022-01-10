using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationEngine
{
    public static class Extension
    {
        public static T[] GetSegment<T>(this T[] array, int startIndex, int lastIndex)
        {
            T[] output = new T[lastIndex - startIndex + 1];

            for (int i = startIndex; i <= lastIndex; i++)
            {
                output[i-startIndex] = array[i];
            }

            return output;
        }
    }
}
