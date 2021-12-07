using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2016
{
    public class Day25
    {
        private void RunProgram(int seed)
        {
            int a = seed;
            int b = 0;
            int c = 0;
            int d = 0;

            int expected = 0;

#pragma warning disable CS0164 // This label has not been referenced
        _0: d = a;
        _1: c = 15;
        _2: b = 170;
        _3: d++;
        _4: b--;
        _5: if (b != 0) goto _3;
            _6: c--;
        _7: if (c != 0) goto _2;
            _8: a = d;
        _9:
        _10: b = a;
        _11: a = 0;
        _12: c = 2;
        _13: if (b != 0) goto _15;
            _14: goto _20;
        _15: b--;
        _16: c--;
        _17: if (c != 0) goto _13;
            _18: a++;
        _19: goto _12;
        _20: b = 2;
        _21: if (c != 0) goto _23;
            _22: goto _26;
        _23: b--;
        _24: c--;
        _25: goto _21;
        _26:
        _27: 
            if (b != expected) 
                return;
            expected = expected == 0 ? 1 : 0;
            
        _28: if (a != 0) goto _9;
            _29: goto _8;
#pragma warning restore CS0164 // This label has not been referenced

            throw new InvalidOperationException();

            //cpy a d
            //cpy 15 c
            //cpy 170 b
            //inc d
            //dec b
            //jnz b -2
            //dec c
            //jnz c -5
            //cpy d a
            //jnz 0 0
            //cpy a b
            //cpy 0 a
            //cpy 2 c
            //jnz b 2
            //jnz 1 6
            //dec b
            //dec c
            //jnz c -4
            //inc a
            //jnz 1 -7
            //cpy 2 b
            //jnz c 2
            //jnz 1 4
            //dec b
            //dec c
            //jnz 1 -4
            //jnz 0 0
            //out b
            //jnz a -19
            //jnz 1 -21
        }
    }
}
