using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPM_RNG_Drop_Calculator
{
    static class RNGIndex
    {
        static uint m = 0x5d588b65;
        static uint b = 0x1;
        static ulong q = 0x100000000;

        static uint NextRng(uint r)
        {
            unchecked
            {
                return m * r + b;
            }
        }

        static uint powmod(uint m, uint n, uint q)
        {
            if (m == 0)
            {
                return 1;
            }
            uint factor1 = powmod(m, Math.Floor((float)n / 2), q);
    }
}
