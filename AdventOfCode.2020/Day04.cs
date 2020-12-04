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
    public class Day04
    {
        private string _input;

        public Day04()
        {
            _input = File.ReadAllText("Inputs/Day04.txt");
        }

        [Fact]
        public void Part1()
        {
            MatchCollection matches = s_Regex.Matches(_input);
            int answer = matches.Count(m =>
            {
                Group group = m.Groups["key"];
                if (group.Captures.Count == 7 && m.Value.Contains("cid"))
                    return false;

                return true;
            });

            Assert.Equal(230, answer);
        }

        [Fact]
        public void Part2()
        {
            MatchCollection matches = s_Regex2.Matches(_input);

            int answer = matches.Count(m =>
            {
                CaptureCollection captures = m.Groups["x"].Captures;

                foreach (Capture capture in captures)
                {
                    string[] tokens = capture.Value.Split(':');
                    string key = tokens[0];
                    string value = tokens[1].TrimEnd();

                    if (key == "cid" && captures.Count == 7)
                        return false;

                    int num = 0;
                    if (int.TryParse(value, out num))
                    {
                        if ((key == "byr" && num is < 1920 or > 2002) ||
                            (key == "iyr" && num is < 2010 or > 2020) ||
                            (key == "eyr" && num is < 2020 or > 2030))
                            return false;
                    }
                    else if (key == "hgt")
                    {
                        num = int.Parse(value.Substring(0, value.Length - 2));
                        if (value.EndsWith("cm"))
                        {
                            if (num is < 150 or > 193)
                                return false;
                        }
                        else if (num is < 59 or > 76)
                            return false;
                    }
                }

                return true;
            });

            Assert.Equal(156, answer);
        }

        private static Regex s_Regex = new Regex(
            @"(?'value'(?'key'byr|iyr|eyr|hgt|hcl|ecl|pid|cid):[#\w]+( |\r\n)?){7,8}",
            RegexOptions.Compiled);

        private static Regex s_Regex2 = new Regex(
            @"(?'x'((byr|iyr|eyr):\d{4}|hgt:\d{2,3}(cm|in)|hcl:#[a-f0-9]{6}|ecl:(amb|blu|brn|gry|grn|hzl|oth)|pid:\d{9}|cid:\w+)( |\r\n)?){7,8}",
            RegexOptions.Compiled);
    }
}
