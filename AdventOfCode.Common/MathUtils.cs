using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Common
{
    public static class MathUtils
    {
        public static long GreatestCommonFactor(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            for (; ;)
            {
                long remainder = a % b;
                if (remainder == 0) return b;
                a = b;
                b = remainder;
            }
        }

        public static long LeastCommonMultiple(long a, params long[] b)
        {
            static long LCM(long a, long b)
            {
                if (a % b == 0) return a;
                if (b % a == 0) return b;

                return (a * b) / GreatestCommonFactor(a, b);
            }

            return b.Length == 0 ? a : b.Aggregate(a, LCM);
        }
    }
}
