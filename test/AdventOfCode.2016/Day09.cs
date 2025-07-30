namespace AdventOfCode._2016;

public class Day09
{
    string _input;

    public Day09()
    {
        _input = File.ReadAllText("Inputs/Day09.txt");
    }

    [Fact]
    public void Part1()
    {
        long answer = DecompressedLength(_input.AsSpan(), SimpleTotal);
        Assert.Equal(183269, answer);
    }

    [Fact]
    public void Part2()
    {
        long answer = DecompressedLength(_input.AsSpan(), RecursiveTotal);
        Assert.Equal(11317278863, answer);
    }

    private delegate long DecompressPortion(ReadOnlySpan<char> portion);

    private static long SimpleTotal(ReadOnlySpan<char> portion)
    {
        return portion.Length;
    }

    private static long RecursiveTotal(ReadOnlySpan<char> portion)
    {
        return DecompressedLength(portion, RecursiveTotal);
    }

    private static long DecompressedLength(ReadOnlySpan<char> input, DecompressPortion decompressPortion)
    {
        long total = 0;
        int index = 0;

        while (true)
        {
            int relIndex = input.Slice(index).IndexOf('(');
            if (relIndex == -1)
            {
                total += input.Slice(index).Length;
                return total;
            }
            else
            {
                total += relIndex;
                index += relIndex;

                int start = index + 1;
                index = start + input.Slice(start).IndexOf('x');
                int width = int.Parse(input.Slice(start, index - start));

                start = index + 1;
                index = start + input.Slice(start).IndexOf(')');
                int count = int.Parse(input.Slice(start, index - start));
                index++;

                total += count * decompressPortion(input.Slice(index, width));

                index += width;
            }
        }
    }
}
