using System.Text;

namespace AdventOfCode._2017;

public class KnotHash
{
    private int[] _list;
    private int _current;
    private int _skip;

    public static string HashString(string input, int size = 256)
    {
        int[] lengths = input.Select(c => (int)c).Concat(new[] { 17, 31, 73, 47, 23 }).ToArray();
        KnotHash hash = new KnotHash(size);

        for (int i = 0; i < 64; i++)
            foreach (int length in lengths)
                hash.Apply(length);

        return hash.Enumerate()
            .Chunk(16)
            .Select(chunk => chunk.Aggregate((x, y) => x ^ y))
            .Select(n => string.Format("{0:x2}", n))
            .Aggregate(new StringBuilder(), (sb, s) => sb.Append(s), sb => sb.ToString());
    }

    public KnotHash(int size)
    {
        _list = Enumerable.Range(0, size).ToArray();
        _current = 0;
        _skip = 0;
    }

    public void Apply(int length)
    {
        int begin = _current;
        int end = begin + length - 1;

        for (int i = 0; i < length / 2; i++)
        {
            int b = Index(begin);
            int e = Index(end);

            (_list[b], _list[e]) = (_list[e], _list[b]);

            begin++;
            end--;
        }

        Advance(length);
    }

    public IEnumerable<int> Enumerate() => _list;

    private void Advance(int length)
    {
        _current = Index(_current + length + _skip++);
    }

    private int Index(int i) => i % _list.Length;
}
