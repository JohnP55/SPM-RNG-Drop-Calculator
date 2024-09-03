using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPM_RNG_Drop_Calculator
{
    public static class SpmSystem
    {
        public static uint randomSeed = 1;

        public static uint _rand_advance()
        {
            randomSeed = randomSeed * 0x5d588b65 + 1;
            return randomSeed;
        }

        private static int _rand(int max)
        {
            uint divisor;
            uint res;

            divisor = 0xffffffff;
            divisor /= (uint)(max + 1);
            if (divisor < 1)
                divisor = 1;

            do
            {
                res = _rand_advance() / divisor;
            } while (res >= max + 1);

            return (int)res;
        }

        public static int irand(int max)
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

        private static float _frand()
        {
            return irand(0x7fff) / 32767.0f;
        }

        public static float frand(float limit)
        {
            return limit * _frand();
        }
    }
}
