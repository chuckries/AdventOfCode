using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2016;

public class Day14
{
    const string Input = "ngcjuoqr";

    private List<string> _hashes = new List<string>(10_000);
    private Dictionary<int, char[]> _quints = new Dictionary<int, char[]>(10_000);
    private static MD5 s_md5 = MD5.Create();
    private static StringBuilder s_sb = new StringBuilder(32);
    private static List<char> s_quintList = new List<char>();

    [Fact]
    public void Part1()
    {
        int answer = Search(GetHash);
        Assert.Equal(18626, answer);
    }

    [Fact(Skip = "MD5 so slow")]
    public void Part2()
    {
        int answer = Search(GetStretchedHash);
        Assert.Equal(20092, answer);
    }

    private int Search(Func<int, string> hashFactory)
    {
        int index = 0;
        int keysFound = 0;
        while (true)
        {
            if (TryFindTriple(index, hashFactory, out char c))
            {
                for (int j = 1; j <= 1000; j++)
                {
                    if (IsQuint(index + j, c, hashFactory))
                    {
                        keysFound++;
                        if (keysFound == 64)
                            return index;
                        break;
                    }
                }
            }
            index++;
        }
    }

    private bool TryFindTriple(int index, Func<int, string> hashProvider, out char c)
    {
        string hash = hashProvider(index);
        c = '?';
        int i = 0;
        while (i < hash.Length - 2)
        {
            if (hash[i] == hash[i + 1])
            {
                if (hash[i] == hash[i + 2])
                {
                    c = hash[i];
                    return true;
                }
                else
                {
                    i += 2;
                }
            }
            else
            {
                i += 1;
            }
        }

        return false;
    }

    private bool IsQuint(int index, char c, Func<int, string> hashProvider)
    {
        if (!_quints.TryGetValue(index, out char[] quintC))
        {
            s_quintList.Clear();
            quintC = null;
            string hash = hashProvider(index);
            int i = 0;
            while (i < hash.Length - 4)
            {
                bool valid = true;
                int j = 1;
                for (; j < 5; j++)
                {
                    if (hash[i] != hash[i + j])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    s_quintList.Add(hash[i]);
                }
                i += j;
            }
            quintC = s_quintList.Count > 0 ? s_quintList.ToArray() : null;
            _quints.Add(index, quintC);
        }

        if (quintC is object)
        {
            for (int i = 0; i < quintC.Length; i++)
                if (quintC[i] == c)
                    return true;
        }

        return false;
    }

    private string GetHash(int index)
    {
        while (_hashes.Count <= index)
            _hashes.Add(null);

        if (_hashes[index] is null)
        {
            _hashes[index] = GetHashString(Input + index.ToString());
        }

        return _hashes[index];
    }

    private string GetStretchedHash(int index)
    {
        while (_hashes.Count <= index)
            _hashes.Add(null);

        string hash = _hashes[index];
        if (hash is null)
        {
            hash = GetHashString(Input + index.ToString());
            for (int i = 0; i < 2016; i++)
                hash = GetHashString(hash);
            _hashes[index] = hash;
        }

        return hash;
    }

    private static string GetHashString(string s)
    {
        byte[] bytes = s_md5.ComputeHash(Encoding.ASCII.GetBytes(s));
        s_sb.Clear();
        for (int i = 0; i < bytes.Length; i++)
            s_sb.Append(bytes[i].ToString("x2"));
        return s_sb.ToString();

    }
}
