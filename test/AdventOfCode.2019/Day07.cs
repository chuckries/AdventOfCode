using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day07
    {
        long[] _code = File.ReadAllText("Inputs/Day07.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            long answer = Enumerable.Range(0, 5)
                .FullPermutations()
                .Select(p => RunAmplifiers(_code, p))
                .Max();

            Assert.Equal(17440, answer);
        }

        [Fact]
        public void Part2()
        {
            long answer = Enumerable.Range(5, 5)
                .FullPermutations()
                .Select(p => RunAmplifiersWithFeedback(_code, p))
                .Max();

            Assert.Equal(27561242, answer);
        }

        private long RunAmplifiers(IEnumerable<long> code, IEnumerable<int> phaseSettings)
        {
            long answer = 0;

            Stack<long> inputs = new Stack<long>();
            inputs.Push(0);
            foreach (int phaseSetting in phaseSettings)
            {
                inputs.Push(phaseSetting);
                IntCode amp = new IntCode(
                    code,
                    inputs.Pop,
                    value => answer = value
                    );
                amp.Run();
                inputs.Push(answer);
            }

            return answer;
        }

        private long RunAmplifiersWithFeedback(IEnumerable<long> code, IEnumerable<int> phaseSettings)
        {
            IntCodeAsync[] amplifiers = new IntCodeAsync[5];
            AsyncQueue<long>[] ports = new AsyncQueue<long>[5];
            int i = 0;
            for (i = 0; i < 5; i++)
            {
                amplifiers[i] = new IntCodeAsync(code);
                ports[i] = new AsyncQueue<long>();
            }

            i = 0;
            foreach (int phaseSetting in phaseSettings)
                ports[i++].Enqueue(phaseSetting);

            ports[0].Enqueue(0);

            for (i = 0; i < amplifiers.Length; i++)
            {
                amplifiers[i].Reader = ports[i].Dequeue;
            }

            for (i = 0; i < amplifiers.Length - 1; i++)
            {
                amplifiers[i].Writer = ports[i + 1].Enqueue;
            }

            long answer = 0;
            amplifiers[^1].Writer = (value) =>
            {
                answer = value;
                ports[0].Enqueue(value);
            };


            Task[] tasks = amplifiers.Select(s => s.RunAsync()).ToArray();
            Task.WaitAll(tasks);

            return answer;
        }
    }
}
