using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode2019
{
    public class Day7
    {
        int[] _code = File.ReadAllText("Inputs/Day7.txt")
            .Split(',')
            .Select(int.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            int answer = Enumerable.Range(0, 5)
                .FullPermutations()
                .Select(p => RunAmplifiers(_code, p))
                .Max();

            Assert.Equal(17440, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Enumerable.Range(5, 5)
                .FullPermutations()
                .Select(p => RunAmplifiersWithFeedback(_code, p))
                .Max();

            Assert.Equal(27561242, answer);
        }

        [Theory]
        [InlineData(new int[] { 3, 15, 3, 16, 1002, 16, 10, 16, 1, 16, 15, 15, 4, 15, 99, 0, 0 }, new int[] { 4, 3, 2, 1, 0 }, 43210)]
        [InlineData(new int[] { 3, 23, 3, 24, 1002, 24, 10, 24, 1002, 23, -1, 23, 101, 5, 23, 23, 1, 24, 23, 23, 4, 23, 99, 0, 0 }, new int[] { 0, 1, 2, 3, 4 }, 54321)]
        [InlineData(new int[] { 3, 31, 3, 32, 1002, 32, 10, 32, 1001, 31, -2, 31, 1007, 31, 0, 33, 1002, 33, 7, 33, 1, 33, 31, 31, 1, 32, 31, 31, 4, 31, 99, 0, 0, 0 }, new int[] { 1, 0, 4, 3, 2 }, 65210)]
        public void Part1ExamplesWithPhaseGiven(int[] code, int[] phases, int expected)
        {
            Assert.Equal(expected, RunAmplifiers(code, phases));
        }

        [Theory]
        [InlineData(new int[] { 3, 15, 3, 16, 1002, 16, 10, 16, 1, 16, 15, 15, 4, 15, 99, 0, 0 }, 43210)]
        [InlineData(new int[] { 3, 23, 3, 24, 1002, 24, 10, 24, 1002, 23, -1, 23, 101, 5, 23, 23, 1, 24, 23, 23, 4, 23, 99, 0, 0 }, 54321)]
        [InlineData(new int[] { 3, 31, 3, 32, 1002, 32, 10, 32, 1001, 31, -2, 31, 1007, 31, 0, 33, 1002, 33, 7, 33, 1, 33, 31, 31, 1, 32, 31, 31, 4, 31, 99, 0, 0, 0 }, 65210)]
        public void Part1Examples(int[] code, int expected)
        {
            int answer = Enumerable.Range(0, 5)
                .FullPermutations()
                .Select(p => RunAmplifiers(code, p))
                .Max();

            Assert.Equal(expected, answer);
        }

        [Theory]
        [InlineData(new int[] { 3, 26, 1001, 26, -4, 26, 3, 27, 1002, 27, 2, 27, 1, 27, 26, 27, 4, 27, 1001, 28, -1, 28, 1005, 28, 6, 99, 0, 0, 5 }, new int[] { 9, 8, 7, 6, 5 }, 139629729)]
        [InlineData(new int[] { 3, 52, 1001, 52, -5, 52, 3, 53, 1, 52, 56, 54, 1007, 54, 5, 55, 1005, 55, 26, 1001, 54, -5, 54, 1105, 1, 12, 1, 53, 54, 53, 1008, 54, 0, 55, 1001, 55, 1, 55, 2, 53, 55, 53, 4, 53, 1001, 56, -1, 56, 1005, 56, 6, 99, 0, 0, 0, 0, 10 }, new int[] { 9, 7, 8, 5, 6 }, 18216)]
        public void Part2ExamplesWithPhaseGiven(int[] code, int[] phases, int expected)
        {
            Assert.Equal(expected, RunAmplifiersWithFeedback(code, phases));
        }

        private int RunAmplifiers(IEnumerable<int> code, IEnumerable<int> phaseSettings)
        {
            int answer = 0;
            IntCode.WriteOutput writer = (value) => { answer = value; };

            Stack<int> inputs = new Stack<int>();
            inputs.Push(0);
            foreach (int phaseSetting in phaseSettings)
            {
                inputs.Push(phaseSetting);
                IntCode amp = new IntCode(
                    code,
                    () => Task.FromResult(inputs.Pop()),
                    writer
                    );
                amp.Run().Wait();
                inputs.Push(answer);
            }

            return answer;
        }

        private int RunAmplifiersWithFeedback(IEnumerable<int> code, IEnumerable<int> phaseSettings)
        {
            IntCode[] amplifiers = new IntCode[5];
            AsyncQueue<int>[] ports = new AsyncQueue<int>[5];
            int i = 0;
            for (i = 0; i < 5; i++)
            {
                amplifiers[i] = new IntCode(code);
                ports[i] = new AsyncQueue<int>();
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

            int answer = 0;
            amplifiers[^1].Writer = (value) =>
            {
                answer = value;
                ports[0].Enqueue(value);
            };

            Task[] tasks = new Task[amplifiers.Length];
            for (i = 0; i < amplifiers.Length; i++)
            {
                tasks[i] = Task.Run(amplifiers[i].Run);
            }

            Task.WaitAll(tasks);

            return answer;
        }
    }
}
