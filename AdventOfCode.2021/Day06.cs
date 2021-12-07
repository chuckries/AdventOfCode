using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2021
{
    public class Day06
    {
        int[] _input;

        public Day06()
        {
            _input = File.ReadAllText("Inputs/Day06.txt").Split(',').Select(int.Parse).ToArray();
        }

        [Fact]
        public void Part1()
        {
            List<int> fish = new(_input);

            for (int i = 0; i < 80; i++)
            {
                int newFish = 0;
                for (int j = 0; j < fish.Count; j++)
                {
                    if (fish[j] == 0)
                    {
                        newFish++;
                        fish[j] = 6;
                    }
                    else
                    {
                        fish[j]--;
                    }
                }
                for (int j = 0; j < newFish; j++)
                    fish.Add(8);
            }

            int answer = fish.Count;

            Assert.Equal(386755, answer);
        }

        [Fact]
        public void Part2()
        {
            ulong[] fish = new ulong[9];
            ulong[] next = (ulong[])fish.Clone();

            foreach (int start in _input)
                fish[start]++;

            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 8; j++)
                    next[j] = fish[j + 1];

                next[6] += fish[0];
                next[8] = fish[0];

                var tmp = fish;
                fish = next;
                next = tmp;
            }

            ulong total = 0;
            for (int i = 0; i < fish.Length; i++)
                total += fish[i];

            Assert.Equal(1732731810807ul, total);
        }
    }
}
