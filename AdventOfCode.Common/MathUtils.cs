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

        /// <summary>
        /// Generates bitmasks with a specific number of bits sets within an overall width of bits. 
        /// Based on Python's itertools pseudo implementation for Combinations
        /// https://docs.python.org/3/library/itertools.html#itertools.combinations
        /// </summary>
        /// <param name="width">The maximum bit width to use</param>
        /// <param name="count">The number of bits that should be set</param>
        /// <returns>The width choose count possible bitmasks</returns>
        public static IEnumerable<int> MaskCombinations(int width, int count)
        {
            if (width <= 0 || width > 32)
                throw new ArgumentException(nameof(width));
            if (count < 0 || count > width)
                throw new ArgumentException(nameof(count));

            if (count == 0)
            {
                yield return 0;
                yield break;
            }

            int[] indices = new int[count];
            for (int i = 0; i < count; i++)
                indices[i] = i;

            yield return MakeMask(indices);

            while (true)
            {
                int i;
                for (i = count - 1; i >= 0; i--)
                {
                    if (indices[i] != i + width - count)
                        break;
                }
                if (i < 0)
                    yield break;

                indices[i]++;
                for (int j = i + 1; j < count; j++)
                {
                    indices[j] = indices[j - 1] + 1;
                }
                yield return MakeMask(indices);
            }


            static int MakeMask(int[] indices)
            {
                int mask = 0;
                for (int i = 0; i < indices.Length; i++)
                    mask |= (1 << indices[i]);
                return mask;
            }
        }

        public static (IntPoint2 min, IntPoint2 max) MinMax(this IEnumerable<IntPoint2> collection)
        {
            return IntPoint2.MinMax(collection);
        }

        public static int BitCount(this int i)
        {
            //https://stackoverflow.com/questions/2709430/count-number-of-bits-in-a-64-bit-long-big-integer
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (int)((((i + (i >> 4)) & 0xF0F0F0F) * 0x1010101) >> 56);
        }

        public static int BitCount(this long i)
        {
            //https://stackoverflow.com/questions/2709430/count-number-of-bits-in-a-64-bit-long-big-integer
            i = i - ((i >> 1) & 0x5555555555555555);
            i = (i & 0x3333333333333333) + ((i >> 2) & 0x3333333333333333);
            return (int)((((i + (i >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
        }
    }
}
