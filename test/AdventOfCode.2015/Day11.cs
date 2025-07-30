using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day11
    {
        const string Input = "hepxcrrq";

        [Fact]
        public void Part1()
        {
            char[] password = Input.ToCharArray();
            NexPassword(password);

            Assert.Equal("hepxxyzz", new string(password));
        }

        [Fact]
        public void Part2()
        {
            char[] password = Input.ToCharArray();
            NexPassword(password);
            NexPassword(password);

            Assert.Equal("heqaabcc", new string(password));
        }

        private static void NexPassword(char[] password)
        {
            while (true)
            {
                IncrementPassword(password);
                if (ValidatePassword(password))
                    break;
            }
        }

        private static bool ValidatePassword(char[] password)
        {
            bool hasStraight = false;
            int numPairs = 0;
            int lastPairEndIndex = -1;

            for (int i = 0; i < password.Length - 1; i++)
            {
                char c = password[i];

                if (!hasStraight &&
                    c <= 'x' &&
                    password[i + 1] == (c + 1) &&
                    (i + 2) < password.Length &&
                    password[i + 2] == (c + 2))
                {
                    hasStraight = true;
                }

                if (numPairs < 2 &&
                    i > lastPairEndIndex &&
                    password[i + 1] == c)
                {
                    numPairs++;
                    lastPairEndIndex = i + 1;
                }
            }

            return hasStraight && numPairs >= 2;
        }

        private static void IncrementPassword(char[] password)
        {
            int index = password.Length - 1;

            while (index >= 0)
            {
                if (password[index] == 'z')
                {
                    password[index] = 'a';
                    index--;
                }
                else
                {
                    char c = ++password[index];
                    if (c == 'i' || c == 'o' || c == 'l')
                        password[index]++;
                    break;
                }
            }
        }
    }
}
