namespace AdventOfCode._2021
{
    public class Day16
    {
        private class Packet
        {
            public readonly int Version;
            public readonly int Type;
            public readonly long Literal;

            private readonly List<Packet> _subPackets;

            private class PacketReader
            {
                readonly byte[] _data;
                int _dataIdx;
                int _bitIdx;
                int _position;

                public PacketReader(string input)
                {
                    _data = new byte[input.Length / 2];
                    for (int i = 0; i < input.Length; i += 2)
                    {
                        int a = ParseChar(input[i]);
                        int b = ParseChar(input[i + 1]);
                        _data[i >> 1] = (byte)(a << 4 | b);
                    }
                    _dataIdx = 0;
                    _bitIdx = 7;
                    _position = 0;
                }

                public Packet ReadPackets()
                {
                    int version = ReadBits(3);
                    int type = ReadBits(3);

                    Packet packet;
                    if (type is 4)
                    {
                        long literal = ReadLiteral();
                        packet = new Packet(version, type, literal);
                    }
                    else
                    {
                        packet = new Packet(version, type, 0);
                        ReadSubPackets(packet);
                    }

                    return packet;
                }

                private void ReadSubPackets(Packet packets)
                {
                    int lengthTypeId = ReadBits(1);
                    if (lengthTypeId is 0)
                    {
                        int bitCount = ReadBits(15);
                        int offset = _position + bitCount;
                        while (_position < offset)
                            packets._subPackets.Add(ReadPackets());
                    }
                    else
                    {
                        int subPackets = ReadBits(11);
                        for (int i = 0; i < subPackets; i++)
                            packets._subPackets.Add(ReadPackets());
                    }
                }

                private long ReadLiteral()
                {
                    long literal = 0;

                    while (true)
                    {
                        int next = ReadBits(5);
                        literal = checked(literal << 4) | (long)(next & 0x0F);
                        if ((next & 0x10) == 0)
                            break;
                    }

                    return literal;
                }

                private int ReadBits(int bits)
                {
                    int result = 0;
                    for (int i = 0; i < bits; i++)
                    {
                        result = checked(result << 1) | (_data[_dataIdx] & (1 << _bitIdx)) >> (_bitIdx);
                        _bitIdx--;
                        if (_bitIdx < 0)
                        {
                            _dataIdx++;
                            _bitIdx = 7;
                        }
                    }

                    _position += bits;
                    return result;
                }

                private static int ParseChar(char c) => c <= '9' ? c - '0' : c - 'A' + 10;

            }

            private Packet(int version, int type, long literal)
            {
                Version = version;
                Type = type;
                Literal = literal;
                _subPackets = new();
            }

            public static Packet ParsePacketTree(string input)
            {
                PacketReader reader = new PacketReader(input);
                return reader.ReadPackets();
            }

            public int SumVersions() =>
                Version + _subPackets.Sum(p => p.SumVersions());

            public long Evaluate() => Type switch
            {
                0 => _subPackets.Sum(p => p.Evaluate()),
                1 => _subPackets.Aggregate(1L, (a, b) => a * b.Evaluate()),
                2 => _subPackets.Min(p => p.Evaluate()),
                3 => _subPackets.Max(p => p.Evaluate()),
                4 => Literal,
                5 => _subPackets[0].Evaluate() > _subPackets[1].Evaluate() ? 1 : 0,
                6 => _subPackets[0].Evaluate() < _subPackets[1].Evaluate() ? 1 : 0,
                7 => _subPackets[0].Evaluate() == _subPackets[1].Evaluate() ? 1 : 0,
                _ => throw new InvalidOperationException()
            };
        }

        Packet _root;

        public Day16()
        {
            _root = Packet.ParsePacketTree(File.ReadAllText("Inputs/Day16.txt"));
        }

        [Fact]
        public void Part1()
        {
            int answer = _root.SumVersions();
            Assert.Equal(893, answer);
        }

        [Fact]
        public void Part2()
        {
            long answer = _root.Evaluate();
            Assert.Equal(4358595186090, answer);
        }
    }
}
