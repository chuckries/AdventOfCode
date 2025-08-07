using System.Net;

namespace AdventOfCode._2022;

public class Day02
{
    List<(char, char)> input;

    public Day02()
    {
        input = File.ReadAllLines("Inputs/Day02.txt")
            .Select(line => line.Split(' '))
            .Select(parts => (parts[0][0], parts[1][0]))
            .ToList();
    }

    [Fact]
    public void Part1()
    {
        static int Result(char a, char b) => (a, b) switch
        {
            ('A', 'X') => 4, // Rock vs Rock
            ('A', 'Y') => 8, // Rock vs Paper
            ('A', 'Z') => 3, // Rock vs Scissors
            ('B', 'X') => 1, // Paper vs Rock
            ('B', 'Y') => 5, // Paper vs Paper
            ('B', 'Z') => 9, // Paper vs Scissors
            ('C', 'X') => 7, // Scissors vs Rock
            ('C', 'Y') => 2, // Scissors vs Paper
            ('C', 'Z') => 6, // Scissors vs Scissors
            _ => throw new ArgumentException("Invalid input")
        };

        int answer = input.Sum(pair => Result(pair.Item1, pair.Item2));
        Assert.Equal(14297, answer);
    }

    [Fact]
    public void Part2()
    {
        static int Result(char a, char b) => (a, b) switch
        {
            ('A', 'X') => 3, // Rock vs Scissors
            ('A', 'Y') => 4, // Rock vs Rock
            ('A', 'Z') => 8, // Rock vs Paper
            ('B', 'X') => 1, // Paper vs Rock
            ('B', 'Y') => 5, // Paper vs Paper
            ('B', 'Z') => 9, // Paper vs Scissors
            ('C', 'X') => 2, // Scissors vs Paper
            ('C', 'Y') => 6, // Scissors vs Scissors
            ('C', 'Z') => 7, // Scissors vs Rock
            _ => throw new ArgumentException("Invalid input")
        };
        int answer = input.Sum(pair => Result(pair.Item1, pair.Item2));
        Assert.Equal(10498, answer);
    }
}
