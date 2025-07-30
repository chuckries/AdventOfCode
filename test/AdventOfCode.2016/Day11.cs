using System.Diagnostics;
using System.Text;

namespace AdventOfCode._2016;

public class Day11
{
    [DebuggerDisplay("{DisplayForDebugger(),nq}")]
    private readonly struct ChipGenState
    {
        public const int MaxItems = 7;

        public readonly HashSet<int> Chips;
        public readonly HashSet<int> Gens;

        private readonly Lazy<long> _id;
        private readonly Lazy<bool> _isValid;
        private readonly Lazy<bool> _isEmpty;

        public long Id => _id.Value;

        public bool IsValid => _isValid.Value;

        public bool IsEmpty => _isEmpty.Value;

        public ChipGenState(HashSet<int> chips, HashSet<int> gens)
        {
            Chips = chips;
            Gens = gens;
            _id = new Lazy<long>(() =>
            {
                long id = 0;
                for (int i = 0; i < chips.Count; i++)
                    id |= 1L << i;
                for (int i = 0; i < gens.Count; i++)
                    id |= 1L << (i + MaxItems);
                return id;
            });
            _isValid = new Lazy<bool>(() =>
            {
                if (chips.Count == 0 || gens.Count == 0)
                    return true;

                return chips.IsProperSubsetOf(gens) || chips.SetEquals(gens);
            });
            _isEmpty = new Lazy<bool>(() =>
            {
                return chips.Count == 0 && gens.Count == 0;
            });
        }

        public ChipGenState Add(in ChipGenState add)
        {
            HashSet<int> chips = new HashSet<int>(Chips);
            HashSet<int> gens = new HashSet<int>(Gens);
            chips.UnionWith(add.Chips);
            gens.UnionWith(add.Gens);
            return new ChipGenState(chips, gens);
        }

        public ChipGenState Sub(in ChipGenState sub)
        {
            HashSet<int> chips = new HashSet<int>(Chips);
            HashSet<int> gens = new HashSet<int>(Gens);
            chips.ExceptWith(sub.Chips);
            gens.ExceptWith(sub.Gens);
            return new ChipGenState(chips, gens);
        }

        public IEnumerable<ChipGenState> EnumMoveCombinations()
        {
            int[] chips = Chips.ToArray();
            int[] gens = Gens.ToArray();

            if (chips.Length == 0 && gens.Length == 0)
            {
                throw new InvalidOperationException();
            }
            else
            {
                for (int i = 1; i <= chips.Length && i <= 2; i++)
                {
                    foreach (int mask in MathUtils.MaskCombinations(chips.Length, i))
                    {
                        HashSet<int> combo = ApplyMask(mask, chips);
                        yield return new ChipGenState(combo, s_EmptySet);
                    }
                }

                for (int i = 1; i <= gens.Length && i <= 2; i++)
                {
                    foreach (int mask in MathUtils.MaskCombinations(gens.Length, i))
                    {
                        HashSet<int> combo = ApplyMask(mask, gens);
                        yield return new ChipGenState(s_EmptySet, combo);
                    }
                }

                HashSet<int> pairs = new HashSet<int>(Chips);
                pairs.IntersectWith(gens);
                foreach (int pair in pairs)
                {
                    yield return new ChipGenState(new HashSet<int> { pair }, new HashSet<int> { pair });
                }
            }
        }

        private string DisplayForDebugger()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("(");
            sb.AppendLine(string.Join(',', Chips));
            sb.AppendLine(") (");
            sb.AppendLine(string.Join(',', Gens));
            sb.AppendLine(")");
            return sb.ToString();
        }

        private static HashSet<int> ApplyMask(int mask, int[] set)
        {
            int bit = 1;
            HashSet<int> masked = new HashSet<int>(set.Length);
            for (int i = 0; i < set.Length; i++)
            {
                if ((mask & bit) != 0)
                    masked.Add(set[i]);
                bit <<= 1;
            }
            return masked;
        }

        static HashSet<int> s_EmptySet = new HashSet<int>();
    }

    private readonly struct State
    {
        const int NumLevels = 4;

        public readonly int Level;
        public readonly int Steps;
        public readonly ChipGenState[] Floors;
        private readonly Lazy<long> _id;

        public long Id => _id.Value;

        public State(int level, int steps, ChipGenState[] floors)
        {
            Level = level;
            Steps = steps;
            Floors = floors;
            _id = new Lazy<long>(() =>
            {
                const int levelShift = ChipGenState.MaxItems * 2;

                long id = 0;
                int i = 0;
                for (; i < floors.Length; i++)
                {
                    id |= floors[i].Id << (i * levelShift);
                }
                id |= 1L << (i * levelShift + level);
                return id;
            });
        }

        public bool IsEmptyBelow(int level)
        {
            bool hasItems = false;
            for (int i = 0; i < level && !hasItems; i++)
            {
                hasItems = !Floors[i].IsEmpty;
            }

            return !hasItems;
        }

        public IEnumerable<State> EnumStates()
        {
            ChipGenState floor = Floors[Level];
            foreach (ChipGenState candidate in floor.EnumMoveCombinations())
            {
                ChipGenState floorResult = floor.Sub(candidate);
                if (!floorResult.IsValid)
                    continue;

                if (Level > 0 && !IsEmptyBelow(Level))
                {
                    ChipGenState nextFloor = Floors[Level - 1];
                    ChipGenState nextFloorResult = nextFloor.Add(candidate);
                    if (nextFloorResult.IsValid)
                    {
                        ChipGenState[] newFloors = ApplyResults(floorResult, nextFloorResult, -1);
                        yield return new State(Level - 1, Steps + 1, newFloors);
                    }
                }

                if (Level < NumLevels - 1)
                {
                    ChipGenState nextFloor = Floors[Level + 1];
                    ChipGenState nextFloorResult = nextFloor.Add(candidate);
                    if (nextFloorResult.IsValid)
                    {
                        ChipGenState[] newFloors = ApplyResults(floorResult, nextFloorResult, 1);
                        yield return new State(Level + 1, Steps + 1, newFloors);
                    }
                }
            }
        }

        private ChipGenState[] ApplyResults(in ChipGenState floorResult, in ChipGenState nextFloorResult, int levelDetla)
        {
            ChipGenState[] newFloors = new ChipGenState[Floors.Length];
            for (int i = 0; i < newFloors.Length; i++)
            {
                if (i == Level)
                    newFloors[i] = floorResult;
                else if (i == Level + levelDetla)
                    newFloors[i] = nextFloorResult;
                else
                    newFloors[i] = Floors[i];
            }
            return newFloors;
        }
    }

    [Fact]
    public void Part1()
    {
        ChipGenState[] floors = new[]
        {
            new ChipGenState(new HashSet<int>{ 0 }, new HashSet<int>{ 0, 1, 2 }),
            new ChipGenState(new HashSet<int>{ 1, 2 }, new HashSet<int>()),
            new ChipGenState(new HashSet<int>{ 3, 4 }, new HashSet<int>{ 3, 4 }),
            new ChipGenState(new HashSet<int>(), new HashSet<int>())
        };

        State initial = new State(0, 0, floors);
        State final = Search(initial);

        Assert.Equal(31, final.Steps);
    }

    [Fact]
    public void Part2()
    {
        ChipGenState[] floors = new[]
        {
            new ChipGenState(new HashSet<int>{ 0, 5, 6}, new HashSet<int>{ 0, 1, 2, 5, 6 }),
            new ChipGenState(new HashSet<int>{ 1, 2 }, new HashSet<int>()),
            new ChipGenState(new HashSet<int>{ 3, 4 }, new HashSet<int>{ 3, 4 }),
            new ChipGenState(new HashSet<int>(), new HashSet<int>())
        };

        State initial = new State(0, 0, floors);
        State final = Search(initial);

        Assert.Equal(55, final.Steps);
    }

    private State Search(in State initial)
    {
        State current = initial;
        Queue<State> queue = new Queue<State>();
        HashSet<long> visisted = new HashSet<long>();
        queue.Enqueue(current);

        while (queue.Count > 0)
        {
            current = queue.Dequeue();

            if (visisted.Contains(current.Id))
                continue;
            visisted.Add(current.Id);

            if (current.IsEmptyBelow(3))
                return current;
            else
            {
                foreach (State next in current.EnumStates())
                    if (!visisted.Contains(next.Id))
                        queue.Enqueue(next);
            }
        }

        throw new InvalidOperationException();
    }
}
