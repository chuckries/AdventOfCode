using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2016;

public class Day05
{
    const string Input = "abbhdwsy";

    [Fact(Skip = "MD5 so slow")]
    public void Part1()
    {
        string answer = new string(EnumPasswords()
            .Take(8)
            .Select(i => i.X.ToString("x")[0])
            .ToArray());

        Assert.Equal("801b56a7", answer);
    }

    [Fact(Skip = "MD5 so slow")]
    public void Part2()
    {
        int count = 0;
        char[] password = new char[8];

        foreach (IntVec2 p in EnumPasswords())
        {
            if (p.X < 8 && password[p.X] != '\0')
            {
                password[p.X] = p.Y.ToString("x")[0];
                count++;
            }

            if (count == 8)
                break;
        }

        string answer = new string(password.Select(c => c).ToArray());
        Assert.Equal("424a0197", answer);
    }

    private IEnumerable<IntVec2> EnumPasswords()
    {
        int i = 0;
        while (true)
        {
            MD5 md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(Input + i.ToString()));
            if (hash[0] == 0 &&
                hash[1] == 0 &&
                (hash[2] & 0xF0) == 0)
            {
                yield return (unchecked((byte)(hash[2] & 0x0F)), unchecked((byte)(hash[3] >> 4)));
            }

            i++;
        }
    }
}
