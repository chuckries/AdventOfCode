using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day17
    {
        const string Input = "bwnlcvfs";

        static MD5 _md5 = MD5.Create();

        [Fact]
        public void Part1()
        {
            string answer = Search().First();
            Assert.Equal("DDURRLRRDD", answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Search().Last().Length;
            Assert.Equal(436, answer);
        }

        private IEnumerable<string> Search()
        {
            (string Path, IntVec2 pos) current = (string.Empty, IntVec2.Zero);
            Queue<(string path, IntVec2 pos)> queue = new Queue<(string path, IntVec2)>();
            queue.Enqueue(current);

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                if (current.pos.Equals((3, 3)))
                {
                    yield return current.Path;
                }
                else
                {
                    foreach (var adj in Adjacent(current.Path))
                    {
                        IntVec2 next = current.pos + adj.delta;
                        if (next.X >=0 && next.X < 4 && next.Y >= 0 && next.Y < 4)
                        {
                            queue.Enqueue((current.Path + adj.dir, next));
                        }
                    }
                }
            }
        }

        private IEnumerable<(char dir, IntVec2 delta)> Adjacent(string path)
        {
            byte[] hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(Input + path));

            byte nibble = (byte)(hash[0] >> 4);
            if (IsValid(nibble))
                yield return ('U', -IntVec2.UnitY);

            nibble = (byte)(hash[0] & 0x0F);
            if (IsValid(nibble))
                yield return ('D', IntVec2.UnitY);

            nibble = (byte)(hash[1] >> 4);
            if (IsValid(nibble))
                yield return ('L', -IntVec2.UnitX);

            nibble = (byte)(hash[1] & 0x0F);
            if (IsValid(nibble))
                yield return ('R', IntVec2.UnitX);


            static bool IsValid(byte b) => b >= 11 && b <= 15;
        }
    }
}
