using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2015;

public class Day04
{
    const string Input = "bgvyzdsv";

    [Fact]
    public void Part1()
    {
        int answer = FindHash(bytes =>
            bytes[0] == 0 &&
            bytes[1] == 0 &&
            (bytes[2] & 0xF0) == 0);

        Assert.Equal(254575, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = FindHash(bytes =>
            bytes[0] == 0 &&
            bytes[1] == 0 &&
            bytes[2] == 0);

        Assert.Equal(1038736, answer);
    }

    private int FindHash(Predicate<byte[]> test)
    {
        MD5 hash = MD5.Create();
        int num = 1;

        while (true)
        {
            byte[] bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(Input + num.ToString()));

            if (test(bytes))
            {
                break;
            }

            num++;
        }

        return num;
    }
}
