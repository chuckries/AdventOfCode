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

        public static long LeastCommonMultiple(long a, long b)
        {
            if (a % b == 0) return a;
            if (b % a == 0) return b;

            return (a * b) / GreatestCommonFactor(a, b);
        }

        public static long LeastCommonMultiple(params long[] numbers) => numbers.Length switch
        {
            0 => 0,
            1 => numbers[0],
            2 => LeastCommonMultiple(numbers[0], numbers[1]),
            _ => numbers.Skip(1).Aggregate(numbers[0], LeastCommonMultiple)
        };
    }
}
