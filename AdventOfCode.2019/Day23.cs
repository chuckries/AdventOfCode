using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day23
    {
        public class Computer
        {
            public Computer(int address, long[] program, Action<long, long, long> send)
            {
                _address = address;
                _receivedPackets = new Queue<long>();
                _receivedPackets.Enqueue(address);
                _outputs = new List<long>();

                _intCode = new IntCode(program,
                    () =>
                    {
                        lock (_receivedPackets)
                        {
                            if (_receivedPackets.Count == 0)
                                return -1L;
                            else
                                return _receivedPackets.Dequeue();
                        }
                    },
                    value =>
                    {
                        _outputs.Add(value);
                        if (_outputs.Count == 3)
                        {
                            send(_outputs[0], _outputs[1], _outputs[2]);
                            _outputs.Clear();
                        }
                    });
            }

            public void Receive(long x, long y)
            {
                lock(_receivedPackets)
                {
                    _receivedPackets.Enqueue(x);
                    _receivedPackets.Enqueue(y);
                }
            }

            public void Run()
            {
                _intCode.Run();
            }

            IntCode _intCode;
            readonly int _address;
            Queue<long> _receivedPackets;
            List<long> _outputs;
        }

        long[] _program;

        public Day23()
        {
            _program = File.ReadAllText("Inputs/Day23.txt").Split(',').Select(long.Parse).ToArray();
        }

        [Fact]
        public void Part1()
        {
            TaskCompletionSource<long> answerTcs = new TaskCompletionSource<long>();
            Computer[] network = new Computer[50];
            for (int i = 0; i < 50; i++)
            {
                network[i] = new Computer(i, _program, (address, x, y) =>
                {
                    if (address == 255)
                        answerTcs.SetResult(y);
                    else
                        network[address].Receive(x, y);
                });
            }

            foreach(Computer c in network)
            {
                Task.Run(c.Run);
            }

            long answer = answerTcs.Task.Result;
            Assert.Equal(0, answer);
        }
    }
}
