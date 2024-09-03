using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPM_RNG_Drop_Calculator
{
    static class Utils
    {
        public static int ReadInt16(byte[] buffer, int offset)
        {
            return buffer[offset] << 8 | buffer[offset+1];
        }
        public static int ReadInt32(byte[] buffer, int offset)
        {
            return buffer[offset] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8 | buffer[offset + 3];
        }

        public static string ReadASCII(byte[] buffer, int offset)
        {
            StringBuilder sb = new StringBuilder();

            char c;
            do
            {
                c = (char)buffer[offset];
                if (c == '\0') break;
                sb.Append(c);
                offset++;
            } while (true);
            return sb.ToString();
        }

        public static void Print<T>(this IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                Console.WriteLine(item);
        }
    }
}
