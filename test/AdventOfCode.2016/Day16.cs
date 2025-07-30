using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day16
    {
        const string Input = "01110110101001000";

        private bool[] _sequence;

        public Day16()
        {
            _sequence = ParseSequence(Input);
        }

        [Fact]
        public void Part1()
        {
            string answer = GetSequenceString(Checksum(_sequence, 272));

            Assert.Equal("11100111011101111", answer);
        }

        [Fact]
        public void Part2()
        {
            string answer = GetSequenceString(Checksum(_sequence, 35_651_584));

            Assert.Equal("10001110010000110", answer);
        }

        private static bool[] Checksum(bool[] sequence, int diskLength)
        {
            int reductions = 0;
            int length = diskLength / 2;
            while (true)
            {
                int halfLength = Math.DivRem(length, 2, out int remainder);
                if (remainder == 1)
                    break;
                reductions++;
                length = halfLength;
            }
            int take = (int)Math.Pow(2, reductions + 1);

            bool[] portion = new bool[take];
            bool[] checksum = new bool[length];

            int iPortion = 0;
            int iChecksum = 0;

            Enumerate(sequence, b =>
            {
                portion[iPortion++] = b;
                if (iPortion == take)
                {
                    iPortion = 0;
                    checksum[iChecksum++] = ReduceChecksumPortion(portion);
                    if (iChecksum == length)
                        return false;
                }

                return true;
            });

            return checksum;
        }

        private static bool ReduceChecksumPortion(bool[] portion)
        {
            int length = portion.Length;
            while ((length /= 2) > 0)
            {
                for (int i = 0; i < length; i++)
                    portion[i] = portion[2 * i] == portion[2 * i + 1];
            }
            return portion[0];
        }

        private static void Enumerate(bool[] sequence, Func<bool, bool> report)
        {
            for (int i = 0; i < sequence.Length; i++)
                if (!report(sequence[i]))
                    return;

            int level = 0;
            while (true)
            {
                if (!report(false))
                    return;

                if (!Sequence(sequence, level++, true, report))
                    return;
            }
        }

        private static bool Sequence(bool[] sequence, int level, bool reverse, Func<bool, bool> report)
        {
            if (level == 0)
            {
                if (!reverse)
                {
                    for (int i = 0; i < sequence.Length; i++)
                        if (!report(sequence[i]))
                            return false;
                }
                else
                {
                    for (int i = sequence.Length - 1; i >= 0; i--)
                        if (!report(!sequence[i]))
                            return false;
                }
            }
            else
            {
                if (!Sequence(sequence, level - 1, false, report))
                    return false;

                if (!report(reverse))
                    return false;

                if (!Sequence(sequence, level - 1, true, report))
                    return false;
            }

            return true;
        }

        private static bool[] ParseSequence(string s) =>
            s.Select(c => c switch
            {
                '0' => false,
                '1' => true,
                _ => throw new InvalidOperationException()
            }).ToArray();

        private static string GetSequenceString(bool[] sequence) =>
            new string(sequence.Select(b => b ? '1' : '0').ToArray());
    }
}
