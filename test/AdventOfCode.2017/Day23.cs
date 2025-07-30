using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2017
{
    public class Day23
    {
        [Fact]
        public void Part1()
        {
            RunProgram(0, out int countMuls);
            Assert.Equal(3025, countMuls);
        }

        [Fact]
        public void Part2()
        {
            int answer = OptimizedProgram();
            Assert.Equal(915, answer);
        }

        private int RunProgram(int seed, out int countMuls)
        {
            int a = seed;
            int b = 0;
            int c = 0;
            int d = 0;
            int e = 0;
            int f = 0;
            int g = 0;
            int h = 0;
            countMuls = 0;

            b = 57;
            c = b;
            if (a != 0) goto _5;
            goto _9;
        _5: 
            b *= 100; countMuls++;
            b += 100000;
            c = b;
            c += 17000;
        _9: 
            f = 1; // set flag
            d = 2; // start d counter at 2
        _11:
            e = 2; // start e counter at 2
        _12: 
            g = d; // start candidate at d
            g *= e; countMuls++;  // multiplay candidate by e
            g -= b; // check if candidate is equal to b
            if (g != 0) goto _17; // if d * e == b, reset flag
            f = 0;
        _17: 
            e += 1; // incrememnt e counter
            g = e;  // set candidate equal to e counter
            g -= b; // check if candidate is equal to b
            if (g != 0) goto _12; // if candidate not equal to b, restart with incremented e
            d += 1; // increment d counter
            g = d; // set candidate equal to d counter
            g -= b; // check if candidate equal to b
            if (g != 0) goto _11; // if candidate not equal to b, restart
            if (f != 0) goto _27; // if flag was not reset, increment h
            h += 1;
        _27: 
            g = b; // check if b == c, if not, add 17 to b and restart (will execute 1000 times)
            g -= c;
            if (g != 0) goto _31;
            goto _33;
        _31: 
            b += 17;
            goto _9;
        _33: return h;
        }

        private int OptimizedProgram()
        {
            int total = 0;
            int start = 57 * 100 + 100000;
            int end = start + 17000;
            for (int i = start; i <= end; i += 17)
                if (!isPrime(i))
                    total++;

            return total;

            static bool isPrime(int num)
            {
                for (int i = 2; i <= Math.Ceiling(Math.Sqrt(num)); i++)
                {
                    if (num % i == 0)
                        return false;
                }
                return true;
            }
        }
    }
}
