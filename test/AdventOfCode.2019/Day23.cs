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
            public bool Idle { get; private set; } = false;

            public Computer(int address, long[] program, Action<long, long, long> send)
            {
                _address = address;
                _receivedPackets = new Queue<long>();
                _receivedPackets.Enqueue(address);
                _receivedPackets.Enqueue(-1);
                _outputs = new List<long>();

                _intCode = new IntCode(program,
                    () =>
                    {
                        lock (_receivedPackets)
                        {
                            if (_receivedPackets.Count == 0)
                            {
                                Idle = true;
                                return -1;
                            }
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
                    _receivedPackets.Enqueue(-1);
                    Idle = false;
                }
            }

            public void Run()
            {
                _intCode.Run();
            }

            public void Step()
            {
                _intCode.Step();
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
        public async Task Part1()
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

            _ = Task.Run(() =>
            {
                while (true)
                {
                    foreach (Computer c in network)
                    {
                        c.Step();
                    }
                }
            });

            long answer = await answerTcs.Task;
            Assert.Equal(22134, answer);
        }

        [Fact]
        public async Task Part2()
        {
            TaskCompletionSource<long> answerTcs = new TaskCompletionSource<long>();
            Computer[] network = new Computer[50];
            (long x, long y) natPacket = (-1, -1);
            long lastYSent = -1;
            for (int i = 0; i < 50; i++)
            {
                network[i] = new Computer(i, _program, (address, x, y) =>
                {
                    if (address == 255)
                        natPacket = (x, y);
                    else
                        network[address].Receive(x, y);
                });
            }

            _ = Task.Run(() =>
            {
                while (true)
                {
                    foreach (Computer c in network)
                    {
                        c.Step();
                    }

                    if (network.All(c => c.Idle))
                    {
                        network[0].Receive(natPacket.x, natPacket.y);
                        if (natPacket.y == lastYSent)
                            answerTcs.SetResult(lastYSent);

                        lastYSent = natPacket.y;
                    }
                }
            });

            long answer = await answerTcs.Task;
            Assert.Equal(16084, answer);
        }
    }
}
