using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPM_RNG_Drop_Calculator
{
    public static class SpmSystem
    {
        public static u32 randomSeed = 1;

        public static u32 _rand_advance()
        {
            randomSeed = randomSeed * 0x5d588b65 + 1;
            return randomSeed;
        }

        private static s32 _rand(s32 max)
        {
            u32 divisor;
            u32 res;

            divisor = 0xffffffff;
            divisor /= (u32)(max + 1);
            if (divisor < 1)
                divisor = 1;

            do
            {
                res = _rand_advance() / divisor;
            } while (res >= max + 1);

            return (s32)res;
        }

        public static s32 irand(s32 max)
        {
            max = Math.Abs(max);

            if (max == 0)
            {
                return 0;
            }
            else if (max == 1)
            {
                if (_rand(1000) <= 500)
                    return 0;
                else
                    return 1;
            }
            else if (max == 100)
            {
                return _rand(1009) / 10;
            }
            else
            {
                return _rand(max);
            }
        }

        private static f32 _frand()
        {
            return irand(0x7fff) / 32767.0f;
        }

        public static f32 frand(f32 limit)
        {
            return limit * _frand();
        }
    }
}
