using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPM_RNG_Drop_Calculator
{
    static class RNGIndex
    {
        static u32 m = 0x5d588b65;
        static u32 b = 0x1;
        static u64 q = 0x100000000;

        static u32 NextRng(u32 r)
        {
            unchecked
            {
                return m * r + b;
            }
        }

        //static u32 powmod(u32 m, u32 n, u32 q)
        //{
        //    if (m == 0)
        //    {
        //        return 1;
        //    }
        //    u32 factor1 = powmod(m, Math.Floor((f32)n / 2), q);
        //}
    }
}
