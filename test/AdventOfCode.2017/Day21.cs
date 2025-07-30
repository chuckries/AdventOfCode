using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode._2017;

public class Day21
{
    class Tile
    {
        public readonly int Size;

        private bool[,] _array;
        private IntVec2 _offset;

        public bool this[int x, int y]
        {
            get
            {
                return _array[x + _offset.X, y + _offset.Y];
            }
            set
            {
                _array[x + _offset.X, y + _offset.Y] = value;
            }
        }

        public Tile(int size, bool[,] array, IntVec2 offset)
        {
            Size = size;
            _array = array;
            _offset = offset;
        }

        public uint GetPrimaryId()
        {
            return GetAllIds().First();
        }

        public IEnumerable<uint> GetAllIds()
        {
            uint id = 0;
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                {
                    if (this[i, j])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;

            id = 0;
            for (int i = Size - 1; i >= 0; i--)
                for (int j = 0; j < Size; j++)
                {
                    if (this[i, j])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;

            id = 0;
            for (int i = 0; i < Size; i++)
                for (int j = Size - 1; j >= 0; j--)
                {
                    if (this[i, j])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;

            id = 0;
            for (int i = Size - 1; i >= 0; i--)
                for (int j = Size - 1; j >= 0; j--)
                {
                    if (this[i, j])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;

            id = 0;
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                {
                    if (this[j, i])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;

            id = 0;
            for (int i = Size - 1; i >= 0; i--)
                for (int j = 0; j < Size; j++)
                {
                    if (this[j, i])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;

            id = 0;
            for (int i = 0; i < Size; i++)
                for (int j = Size - 1; j >= 0; j--)
                {
                    if (this[j, i])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;

            id = 0;
            for (int i = Size - 1; i >= 0; i--)
                for (int j = Size - 1; j >= 0; j--)
                {
                    if (this[j, i])
                        id |= 1;
                    id <<= 1;
                }
            yield return id;
        }

        public void CopyTo(Tile other)
        {
            if (Size != other.Size)
                throw new InvalidOperationException();

            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    other[i, j] = this[i, j];
        }
    }

    class RuleMap
    {
        private readonly record struct RuleKey(int Size, uint Id);

        private Dictionary<RuleKey, Tile> _rules;

        public RuleMap(string[] rules)
        {
            _rules = new(rules.Length * 8);
            foreach (string rule in rules)
                AddRule(rule);
        }

        public void AddRule(string rule)
        {
            string[] parts = rule.Split("=>", StringSplitOptions.TrimEntries);
            string[] ruleParts = parts[0].Split('/');
            string[] matchParts = parts[1].Split('/');

            Tile ruleTile = ParseTile(ruleParts);
            Tile matchTile = ParseTile(matchParts);
            Debug.Assert(ruleTile.Size + 1 == matchTile.Size);

            HashSet<uint> uniqueRuleIds = new HashSet<uint>(ruleTile.GetAllIds());
            foreach (uint id in uniqueRuleIds)
                _rules.Add(new RuleKey(ruleTile.Size, id), matchTile);
        }

        public bool TryGetMatchedTile(int size, uint id, [NotNullWhen(true)] out Tile? matched) =>
            _rules.TryGetValue(new RuleKey(size, id), out matched);

        private Tile ParseTile(string[] parts)
        {
            int size = parts[0].Length;
            bool[,] array = new bool[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    array[i, j] = parts[j][i] == '#';
            Tile tile = new Tile(size, array, IntVec2.Zero);
            return tile;
        }
    }

    class Pattern
    {
        private int _size;
        private bool[,] _array;

        public static Pattern Start
        {
            get
            {
                Pattern p = new Pattern(3);
                p._array = new bool[,]
                {
                    { false, false, true },
                    { true, false, true },
                    { false, true, true }
                };
                return p;
            }
        }

        private Pattern(int size)
        {
            _size = size;
            _array = new bool[_size, _size];
        }

        public Pattern ApplyRules(RuleMap rules)
        {
            (int tileSize, int tileCount) = GetTileSizeAndCount();
            int nextTileSize = GetNextTileSize(tileSize);
            int nextSize = nextTileSize * tileCount;

            Pattern nextPattern = new Pattern(nextSize);

            foreach ((Tile currentTile, Tile nextTile) in GetTiles(tileSize).Zip(nextPattern.GetTiles(nextTileSize)))
            {
                if (!rules.TryGetMatchedTile(currentTile.Size, currentTile.GetPrimaryId(), out Tile? matchedTile))
                    throw new InvalidOperationException();

                matchedTile.CopyTo(nextTile);
            }

            return nextPattern;
        }

        public int CountOns()
        {
            int total = 0;
            for (int i = 0; i < _size; i++)
                for (int j = 0; j < _size; j++)
                    if (_array[i, j])
                        total++;

            return total;
        }

        private IEnumerable<Tile> GetTiles(int tileSize)
        {
            for (int i = 0; i < _size; i += tileSize)
                for (int j = 0; j < _size; j += tileSize)
                    yield return new Tile(tileSize, _array, (i, j));
        }

        private (int size, int count) GetTileSizeAndCount()
        {
            int remainder;
            int count = Math.DivRem(_size, 2, out remainder);
            if (remainder == 0)
            {
                return (2, count);
            }

            count = Math.DivRem(_size, 3, out remainder);
            if (remainder == 0)
            {
                return (3, count);
            }

            throw new InvalidOperationException();
        }

        private int GetNextTileSize(int currentTileSize) =>
            currentTileSize == 2 ? 3 : currentTileSize == 3 ? 4 : throw new InvalidOperationException();
    }

    private readonly RuleMap _ruleMap;

    public Day21()
    {
        string[] rules = File.ReadAllLines("Inputs/Day21.txt");
        _ruleMap = new RuleMap(rules);
    }

    [Fact]
    public void Part1()
    {
        int answer = DoIterations(5);
        Assert.Equal(125, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = DoIterations(18);
        Assert.Equal(1782917, answer);
    }

    private int DoIterations(int iterations)
    {
        Pattern p = Pattern.Start;
        for (int i = 0; i < iterations; i++)
        {
            p = p.ApplyRules(_ruleMap);
        }
        return p.CountOns();
    }
}
