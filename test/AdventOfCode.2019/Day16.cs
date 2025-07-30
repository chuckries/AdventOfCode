using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day16
    {
        string _input = File.ReadAllText("Inputs/Day16.txt");

        [Fact]
        public void Part1()
        {
            int[] data = _input.Select(c => c - '0').ToArray();

            for (int generation = 0; generation < 100; generation++)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    int sequence = i + 1;
                    int index = i;
                    int total = 0;
                    int counter;
                    while (index < data.Length)
                    {
                        counter = sequence;
                        while (counter-- > 0 && index < data.Length)
                            total += data[index++];
                        index += sequence;
                        counter = sequence;
                        while (counter-- > 0 && index < data.Length)
                            total -= data[index++];
                        index += sequence;
                    }
                    data[i] = Math.Abs(total) % 10;
                }
            }

            string answer = new string(data.Take(8).Select(i => (char)(i + '0')).ToArray());
            Assert.Equal("84487724", answer);
        }

        [Fact]
        public void Part2()
        {
            int offset = int.Parse(_input.Substring(0, 7));
            int[] originalData = _input.Select(c => c - '0').ToArray();

            int[] data = new int[originalData.Length * 10000 - offset];
            int index = 0;
            for (int i = offset; i < originalData.Length * 10000; i++)
                data[index++] = originalData[i % originalData.Length];

            for (int generations = 0; generations < 100; generations++)
            {
                for (int i = data.Length - 2; i >= 0; i--)
                {
                    data[i] = (data[i] + data[i + 1]) % 10;
                }
            }

            string answer = new string(data.Take(8).Select(i => (char)(i + '0')).ToArray());
            Assert.Equal("84692524", answer);
        }
    }
}
