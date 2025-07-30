namespace AdventOfCode._2015;

public class Day16
{
    // givens
    const int Children = 3;
    const int Cats = 7;
    const int Samoyeds = 2;
    const int Pomeranians = 3;
    const int Akitas = 0;
    const int Vizslas = 0;
    const int Goldfish = 5;
    const int Trees = 3;
    const int Cars = 2;
    const int Perfumes = 1;

    [Fact]
    public void Part1()
    {
        int answer = TestInputs((ReadOnlySpan<char> key, int value) =>
            (key.Equals("children", StringComparison.Ordinal) && value == Children) ||
            (key.Equals("cats", StringComparison.Ordinal) && value == Cats) ||
            (key.Equals("samoyeds", StringComparison.Ordinal) && value == Samoyeds) ||
            (key.Equals("pomeranians", StringComparison.Ordinal) && value == Pomeranians) ||
            (key.Equals("akitas", StringComparison.Ordinal) && value == Akitas) ||
            (key.Equals("vizslas", StringComparison.Ordinal) && value == Vizslas) ||
            (key.Equals("goldfish", StringComparison.Ordinal) && value == Goldfish) ||
            (key.Equals("trees", StringComparison.Ordinal) && value == Trees) ||
            (key.Equals("cars", StringComparison.Ordinal) && value == Cars) ||
            (key.Equals("perfumes", StringComparison.Ordinal) && value == Perfumes));

        Assert.Equal(373, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = TestInputs((ReadOnlySpan<char> key, int value) =>
            (key.Equals("children", StringComparison.Ordinal) && value == Children) ||
            (key.Equals("cats", StringComparison.Ordinal) && value > Cats) ||
            (key.Equals("samoyeds", StringComparison.Ordinal) && value == Samoyeds) ||
            (key.Equals("pomeranians", StringComparison.Ordinal) && value < Pomeranians) ||
            (key.Equals("akitas", StringComparison.Ordinal) && value == Akitas) ||
            (key.Equals("vizslas", StringComparison.Ordinal) && value == Vizslas) ||
            (key.Equals("goldfish", StringComparison.Ordinal) && value < Goldfish) ||
            (key.Equals("trees", StringComparison.Ordinal) && value > Trees) ||
            (key.Equals("cars", StringComparison.Ordinal) && value == Cars) ||
            (key.Equals("perfumes", StringComparison.Ordinal) && value == Perfumes));

        Assert.Equal(260, answer);
    }

    private delegate bool Test(ReadOnlySpan<char> key, int value);

    private int TestInputs(Test test)
    {
        string[] inputs = File.ReadAllLines("Inputs/Day16.txt");
        int i = 0;
        for (; i < inputs.Length; i++)
            if (ParseAndTest(inputs[i], test))
                break;

        return i + 1;
    }

    private bool ParseAndTest(string input, Test test)
    {
        // chop off sue
        int index = input.IndexOf(':');

        while (index != -1)
        {
            index += 2;

            int wordStart = index;
            index = input.IndexOf(':', wordStart);
            ReadOnlySpan<char> word = input.AsSpan(wordStart, index - wordStart);

            int numStart = index + 2;
            index = input.IndexOf(',', numStart);
            ReadOnlySpan<char> numSpan = input.AsSpan(numStart, (index == -1 ? input.Length : index) - numStart);
            int number = int.Parse(numSpan);

            if (!test(word, number))
                return false;
        }

        return true;
    }
}
