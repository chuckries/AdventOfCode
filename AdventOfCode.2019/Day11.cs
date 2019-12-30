using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day11
    {
        long[] _program = File.ReadAllText("Inputs/Day11.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            Dictionary<IntPoint2, bool> canvas = new Dictionary<IntPoint2, bool>();
            Run(_program, canvas);
            int answer = canvas.Count;
            Assert.Equal(2018, answer);
        }

        [Fact]
        public void Part2()
        {
            Dictionary<IntPoint2, bool> canvas = new Dictionary<IntPoint2, bool>
            { { IntPoint2.Zero, true } };
            Run(_program, canvas);

            (IntPoint2 min, IntPoint2 max) = IntPoint2.MinMax(canvas.Keys);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();

            for (int y = max.Y; y >= min.Y; y--)
            {
                for (int x = min.X; x <= max.X; x++)
                {
                    if (canvas.TryGetValue((x, y), out bool val))
                        sb.Append(val ? '█' : ' ');
                    else
                        sb.Append(' ');
                }
                sb.AppendLine();
            }

            string answer = sb.ToString();

            const string expected = @"
  ██  ███  ████ █  █ ███  █  █ ███  ███    
 █  █ █  █ █    █ █  █  █ █ █  █  █ █  █   
 █  █ █  █ ███  ██   █  █ ██   ███  █  █   
 ████ ███  █    █ █  ███  █ █  █  █ ███    
 █  █ █    █    █ █  █ █  █ █  █  █ █ █    
 █  █ █    █    █  █ █  █ █  █ ███  █  █   
";

            Assert.Equal(expected, answer);
        }

        private void Run(long[] program, Dictionary<IntPoint2, bool> canvas)
        {
            IntPoint2 position = IntPoint2.Zero;
            IntPoint2 heading = IntPoint2.UnitY;
            bool writeMode = false;

            IntCodeAsync.InputReaderAsync reader = () =>
            {
                if (!canvas.TryGetValue(position, out bool value))
                    value = false;

                return Task.FromResult<long>(value ? 1 : 0);
            };

            IntCodeBase.OutputWriter writer = value =>
            {
                if (!writeMode)
                {
                    canvas[position] = value != 0;
                }
                else
                {
                    if (value == 0)
                        heading = heading.TurnLeft();
                    else if (value == 1)
                        heading = heading.TurnRight();
                    else
                        throw new InvalidOperationException();

                    position += heading;
                }
                writeMode = !writeMode;
            };

            IntCodeAsync intCode = new IntCodeAsync(program, reader, writer);
            intCode.RunAsync().Wait();
        }
    }
}
