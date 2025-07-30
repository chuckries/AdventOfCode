using System.Text.RegularExpressions;

namespace AdventOfCode._2015;

public class Day15
{
    private readonly struct Ingredient
    {
        public readonly string Name;
        public readonly int Capacity;
        public readonly int Durability;
        public readonly int Flavor;
        public readonly int Texture;
        public readonly int Calories;

        public Ingredient(string input)
        {
            Match match = s_Regex.Match(input);
            Name = match.Groups["name"].Value;
            Capacity = int.Parse(match.Groups["cap"].Value);
            Durability = int.Parse(match.Groups["dur"].Value);
            Flavor = int.Parse(match.Groups["flav"].Value);
            Texture = int.Parse(match.Groups["tex"].Value);
            Calories = int.Parse(match.Groups["cal"].Value);
        }

        private static Regex s_Regex = new Regex(
            @"^(?'name'[a-zA-Z]+): capacity (?'cap'-?\d+), durability (?'dur'-?\d+), flavor (?'flav'-?\d+), texture (?'tex'-?\d+), calories (?'cal'-?\d+)$",
            RegexOptions.Compiled);
    }

    private Ingredient[] _ingredients;

    public Day15()
    {
        _ingredients = File.ReadAllLines("Inputs/Day15.txt").Select(s => new Ingredient(s)).ToArray();
    }

    [Fact]
    public void Part1()
    {
        int answer = EnumRecipes().Max(r => r.score);
        Assert.Equal(13882464, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = EnumRecipes().Where(r => r.calories == 500).Max(r => r.score);
        Assert.Equal(11171160, answer);
    }

    private IEnumerable<(int score, int calories)> EnumRecipes()
    {
        for (int a = 0; a <= 100; a++)
        {
            for (int b = 0; b <= 100 - a; b++)
            {
                for (int c = 0; c <= 100 - (a + b); c++)
                {
                    int d = 100 - (a + b + c);

                    int capacity = a * _ingredients[0].Capacity +
                                   b * _ingredients[1].Capacity +
                                   c * _ingredients[2].Capacity +
                                   d * _ingredients[3].Capacity;
                    if (capacity < 0)
                        continue;

                    int durability = a * _ingredients[0].Durability +
                                     b * _ingredients[1].Durability +
                                     c * _ingredients[2].Durability +
                                     d * _ingredients[3].Durability;
                    if (durability < 0)
                        continue;

                    int flavor = a * _ingredients[0].Flavor +
                                 b * _ingredients[1].Flavor +
                                 c * _ingredients[2].Flavor +
                                 d * _ingredients[3].Flavor;
                    if (flavor < 0)
                        continue;

                    int texture = a * _ingredients[0].Texture +
                                  b * _ingredients[1].Texture +
                                  c * _ingredients[2].Texture +
                                  d * _ingredients[3].Texture;
                    if (texture < 0)
                        continue;

                    int calories = a * _ingredients[0].Calories +
                                   b * _ingredients[1].Calories +
                                   c * _ingredients[2].Calories +
                                   d * _ingredients[3].Calories;

                    int score = capacity * durability * flavor * texture;
                    yield return (score, calories);
                }
            }
        }
    }
}
