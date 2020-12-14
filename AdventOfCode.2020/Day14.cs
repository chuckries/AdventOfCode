using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;

namespace AdventOfCode._2020
{
    public class Day14
    {
        private class ValueMask
        {
            public string Mask
            {
                set
                {
                    _andMask = ~0;
                    _orMask = 0;

                    for (int i = 0; i < 36; i++)
                    {
                        long mask = 1L << 36 - i - 1;

                        if (value[i] == '0')
                            _andMask &= ~mask;
                        else if (value[i] == '1')
                            _orMask |= mask;
                    }
                }
            }

            public ValueMask()
            {
                _andMask = ~0;
                _orMask = 0;
            }

            public long Apply(long input)
            {
                input |= _orMask;
                input &= _andMask;
                return input;
            }

            private long _andMask;
            private long _orMask;
        }

        private class FloatingMemoryDictionary
        {
            private Dictionary<string, long> _dict = new();

            public string Mask { get; set; }

            public void Add(long address, long value)
            {
                string effective = ApplyMask(address);
                foreach (string resolved in EnumerateAddresses(effective))
                {
                    _dict[resolved] = value;
                }
            }

            private IEnumerable<string> EnumerateAddresses(string address)
            {
                List<string> addresses = new((int)Math.Pow(2, address.Count('X'.Equals)));
                Recurse(0, new char[address.Length], address, addresses);
                return addresses;

                static void Recurse(int index, char[] current, string template, List<string> addresses)
                {
                    if (index == template.Length)
                        addresses.Add(new string(current));
                    else if (template[index] == 'X')
                    {
                        current[index] = '0';
                        Recurse(index + 1, current, template, addresses);
                        current[index] = '1';
                        Recurse(index + 1, current, template, addresses);
                        current[index] = 'X';
                    }
                    else
                    {
                        current[index] = template[index];
                        Recurse(index + 1, current, template, addresses);
                    }
                }
            }

            public long Sum()
            {
                return _dict.Values.Sum();
            }

            private string ApplyMask(long address)
            {
                string binary = Convert.ToString(address, 2).PadLeft(36, '0');

                char[] result = new char[binary.Length];
                for (int i = 0; i < binary.Length; i++)
                {
                    result[i] = Mask[i] switch
                    {
                        '0' => binary[i],
                        '1' => '1',
                        'X' => 'X',
                        _ => throw new InvalidOperationException()
                    };
                }
                return new string(result);
            }
        }

        [Fact]
        public void Part1()
        {
            Dictionary<long, long> mem = new Dictionary<long, long>();
            ValueMask mask = new ValueMask();

            foreach(var input in Parse())
            {
                if (input.mask != null)
                    mask.Mask = input.mask;
                else
                {
                    mem[input.address] = mask.Apply(input.value);
                }
            }

            long answer = mem.Values.Sum();
            Assert.Equal(5875750429995, answer);
        }

        [Fact]
        public void Part2()
        {
            FloatingMemoryDictionary mem = new();

            foreach (var input in Parse())
            {
                if (input.mask != null)
                    mem.Mask = input.mask;
                else
                {
                    mem.Add(input.address, input.value);
                }
            }

            long answer = mem.Sum();
            Assert.Equal(5272149590143, answer);
        }

        private IEnumerable<(long address, long value, string mask)> Parse()
        {
            foreach (string line in File.ReadAllLines("Inputs/Day14.txt"))
            {
                if (line.StartsWith("mask"))
                    yield return (0, 0, line.Substring(line.Length - 36));
                else
                {
                    int index = line.IndexOf('[');
                    ReadOnlySpan<char> span = line.AsSpan(index + 1);
                    index = span.IndexOf(']');

                    long address = long.Parse(span.Slice(0, index));

                    index = span.IndexOf('=');

                    long value = long.Parse(span.Slice(index + 2));
                    yield return (address, value, null);
                }
            }
        }
    }
}
