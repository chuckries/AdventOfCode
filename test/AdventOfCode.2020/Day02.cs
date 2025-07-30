using System.Text.RegularExpressions;

namespace AdventOfCode._2020;

public class Day02
{
    [Fact]
    public void Part1()
    {
        int answer = CountValidPasswords(Validate1);

        Assert.Equal(640, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = CountValidPasswords(Validate2);

        Assert.Equal(472, answer);
    }

    private delegate bool Validate(int low, int high, char letter, string word);

    private int CountValidPasswords(Validate validate) =>
        File.ReadAllLines("Inputs/Day02.txt")
            .Count(s => IsValidPassword(s, validate));

    private bool IsValidPassword(string password, Validate validate)
    {
        GroupCollection groups = s_Regex.Match(password).Groups;
        int low = int.Parse(groups["low"].Value);
        int high = int.Parse(groups["high"].Value);
        char letter = groups["letter"].Value[0];
        string word = groups["word"].Value;

        return validate(low, high, letter, word);
    }

    private bool Validate1(int low, int high, char letter, string word)
    {
        int count = word.Count(letter.Equals);
        return count >= low && count <= high;
    }

    private bool Validate2(int low, int high, char letter, string word) =>
        word[low - 1] == letter ^ word[high - 1] == letter;

    Regex s_Regex = new Regex(@"^(?'low'\d+)-(?'high'\d+) (?'letter'[a-z]): (?'word'[a-z]+)$");
}
