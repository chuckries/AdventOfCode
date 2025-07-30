using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day04
    {
        private class Room
        {
            public readonly string EncryptedName;
            public readonly int SectorId;
            public readonly string Checksum;

            public Room(string s)
            {
                Match m = s_Regex.Match(s);
                EncryptedName = m.Groups["name"].Value;
                SectorId = int.Parse(m.Groups["id"].Value);
                Checksum = m.Groups["checksum"].Value;
            }

            public string Decrypt()
            {
                StringBuilder sb = new StringBuilder(EncryptedName.Length);
                foreach(int c in EncryptedName)
                {
                    if (c == '-')
                        sb.Append(' ');
                    else
                    {
                        int i = c - 'a';
                        i += SectorId;
                        i %= 26;
                        i += 'a';
                        sb.Append((char)i);
                    }
                }
                return sb.ToString();
            }

            public bool IsValid() => CalculateChecksum() == Checksum;

            private string CalculateChecksum()
            {
                Dictionary<char, int> counts = new Dictionary<char, int>(26);
                foreach (char c in EncryptedName)
                {
                    if (c == '-')
                        continue;

                    if (!counts.ContainsKey(c))
                        counts[c] = 1;
                    else
                        counts[c]++;
                }

                var list = counts.ToList();
                list.Sort((lhs, rhs) =>
                {
                    int val = rhs.Value - lhs.Value;
                    if (val == 0)
                        val = lhs.Key - rhs.Key;
                    return val;
                });

                return new string(list.Take(5).Select(kvp => kvp.Key).ToArray());
            }

            static Regex s_Regex = new Regex(
                @"^(?'name'.*)-(?'id'\d+)\[(?'checksum'[a-z]+)\]$",
                RegexOptions.Compiled);
        }

        private Room[] _rooms;

        public Day04()
        {
            _rooms = File.ReadAllLines("Inputs/Day04.txt").Select(s => new Room(s)).ToArray();
        }

        [Fact]
        public void Part1()
        {
            int answer = _rooms.Where(r => r.IsValid()).Sum(r => r.SectorId);
            Assert.Equal(185371, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = _rooms.First(r => r.Decrypt() == "northpole object storage").SectorId;
            Assert.Equal(984, answer);
        }

    }
}
