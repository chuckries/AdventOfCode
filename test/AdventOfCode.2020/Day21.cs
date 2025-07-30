using System.Text.RegularExpressions;

namespace AdventOfCode._2020;

public class Day21
{
    private HashSet<string>[] _recipes;
    private HashSet<string> _ingredients;
    private Dictionary<string, HashSet<string>> _allergens;

    public Day21()
    {
        string[] input = File.ReadAllLines("Inputs/Day21.txt");

        List<HashSet<string>> recipes = new(input.Length);
        _ingredients = new();
        _allergens = new();

        foreach (string s in input)
        {
            Match m = s_Regex.Match(s);
            List<string> ingredients = new(m.Groups["left"].Captures.Count);
            List<string> allergens = new(m.Groups["right"].Captures.Count);

            foreach (Capture c in m.Groups["left"].Captures)
            {
                _ingredients.Add(c.Value);
                ingredients.Add(c.Value);
            }

            foreach (Capture c in m.Groups["right"].Captures)
                allergens.Add(c.Value);

            foreach (string allergen in allergens)
            {
                if (!_allergens.TryGetValue(allergen, out HashSet<string>? possibleIngredients))
                    _allergens.Add(allergen, new(ingredients));
                else
                    possibleIngredients.IntersectWith(ingredients);
            }

            recipes.Add(new HashSet<string>(ingredients));
        }

        _recipes = recipes.ToArray();
    }

    [Fact]
    public void Part1()
    {
        HashSet<string> inertIngredients = new HashSet<string>(_ingredients);
        foreach (HashSet<string> allergenIngredients in _allergens.Values)
            inertIngredients.ExceptWith(allergenIngredients);

        int answer = inertIngredients.Sum(i => _recipes.Count(r => r.Contains(i)));
        Assert.Equal(2573, answer);
    }

    [Fact]
    public void Part2()
    {
        Dictionary<string, HashSet<string>> possibleAllergens = new(_allergens);
        List<(string allergen, string ingredient)> assignments = new(_allergens.Count);

        while (possibleAllergens.Count > 0)
        {
            (string allergen, HashSet<string> ingredients) = possibleAllergens.First(kvp => kvp.Value.Count == 1);
            string ingredient = ingredients.Single();
            assignments.Add((allergen, ingredient));
            possibleAllergens.Remove(allergen);
            foreach (HashSet<string> remaining in possibleAllergens.Values)
                remaining.Remove(ingredient);
        }

        assignments.Sort((lhs, rhs) => string.Compare(lhs.allergen, rhs.allergen));
        string answer = string.Join(',', assignments.Select(a => a.ingredient));

        Assert.Equal(
            "bjpkhx,nsnqf,snhph,zmfqpn,qrbnjtj,dbhfd,thn,sthnsg",
            answer);
    }

    private static Regex s_Regex = new Regex(
        @"^((?'left'\w+)( (?'left'\w+))*) \(contains (?'right'\w+)(, (?'right'\w+))*\)$",
        RegexOptions.Compiled);
}
