namespace AdventOfCode._2015;

public class Day21
{
    private class Item
    {
        public readonly int Cost;
        public readonly int Damage;
        public readonly int Armor;
        public readonly int Id;

        public Item(int cost, int damage, int armor, int id)
        {
            Cost = cost;
            Damage = damage;
            Armor = armor;
            Id = id;
        }
    }

    static Item[] s_Weapons = new Item[]
    {
        new Item(8, 4, 0, 1),
        new Item(10, 5, 0, 2),
        new Item(25, 6, 0, 3),
        new Item(40, 7, 0, 4),
        new Item(74, 8, 0, 5),
    };

    static Item[] s_Armors = new Item[]
    {
        new Item(13, 0, 1, 10),
        new Item(31, 0, 2, 20),
        new Item(53, 0, 3, 30),
        new Item(75, 0, 4, 40),
        new Item(102, 0, 5, 50)
    };

    static Item[] s_Rings = new Item[]
    {
        new Item(25, 1, 0, 100),
        new Item(50, 2, 0, 200),
        new Item(100, 3, 0, 300),
        new Item(20, 0, 1, 400),
        new Item(40, 0, 2, 500),
        new Item(80, 0, 3, 600)
    };

    private readonly struct Outfit
    {
        public readonly Item Weapon;
        public readonly Item? Armor;
        public readonly Item? Ring1;
        public readonly Item? Ring2;
        public readonly int Id;

        public readonly int Cost;
        public readonly int Damage;
        public readonly int ArmorCount;

        public Outfit(Item weapon)
            : this(weapon, null, null, null)
        {
        }

        public Outfit(Item weapon, Item? armor, Item? ring1, Item? ring2)
        {
            Weapon = weapon;
            Armor = armor;
            Ring1 = ring1;
            Ring2 = ring2;

            int costTotal, damageTotal, armorTotal;
            costTotal = damageTotal = armorTotal = 0;
            AddUp(weapon, ref costTotal, ref damageTotal, ref armorTotal);
            AddUp(armor, ref costTotal, ref damageTotal, ref armorTotal);
            AddUp(ring1, ref costTotal, ref damageTotal, ref armorTotal);
            AddUp(ring2, ref costTotal, ref damageTotal, ref armorTotal);

            Cost = costTotal;
            Damage = damageTotal;
            ArmorCount = armorTotal;

            int i = 0;
            i += weapon.Id;
            if (armor is object) i += armor.Id;
            if (ring1 is object) i += ring1.Id;
            if (ring2 is object) i += (ring2.Id * 10);
            Id = i;

            static void AddUp(Item? item, ref int costTotal, ref int damageTotal, ref int armorTtoal)
            {
                if (item != null)
                {
                    costTotal += item.Cost;
                    damageTotal += item.Damage;
                    armorTtoal += item.Armor;
                }
            }
        }

        public IEnumerable<Outfit> ExtendOutfit()
        {
            if (Armor is null)
            {
                foreach (Item armor in s_Armors)
                {
                    yield return new Outfit(Weapon, armor, Ring1, Ring2);
                }
            }

            if (Ring1 is null)
            {
                foreach (Item ring in s_Rings)
                {
                    yield return new Outfit(Weapon, Armor, ring, null);
                }
            }
            else if (Ring2 is null)
            {
                foreach (Item ring in s_Rings)
                {
                    if (ring == Ring1)
                        continue;

                    yield return new Outfit(Weapon, Armor, Ring1, ring);
                }
            }
        }

        public IEnumerable<Outfit> DiminishOutfit()
        {
            if (Ring2 is object)
            {
                foreach (Item ring in s_Rings)
                {
                    if (ring.Cost <= Ring2.Cost && ring != Ring2 && ring != Ring1)
                        yield return new Outfit(Weapon, Armor, Ring1, ring);
                }
                yield return new Outfit(Weapon, Armor, Ring1, null);
            }
            else if (Ring1 is object)
            {
                foreach (Item ring in s_Rings)
                {
                    if (ring.Cost <= Ring1.Cost && ring != Ring1)
                        yield return new Outfit(Weapon, Armor, ring, null);
                }
                yield return new Outfit(Weapon, Armor, null, null);
            }

            if (Armor is object)
            {
                foreach (Item armor in s_Armors)
                {
                    if (armor.Cost <= Armor.Cost && armor != Armor)
                        yield return new Outfit(Weapon, armor, Ring1, Ring2);
                }
                yield return new Outfit(Weapon, null, Ring1, Ring2);
            }

            foreach (Item weapon in s_Weapons)
            {
                if (weapon.Cost <= Weapon.Cost && weapon != Weapon)
                    yield return new Outfit(weapon, Armor, Ring1, Ring2);
            }
        }
    }

    private readonly struct Actor
    {
        public readonly int HitPoints;
        public readonly int Damage;
        public readonly int Armor;

        public Actor(int hitPoints, int damage, int armor)
        {
            HitPoints = hitPoints;
            Damage = damage;
            Armor = armor;
        }

        public bool Wins(in Actor opponent)
        {
            int myAttack = Math.Max(Damage - opponent.Armor, 1);
            int opponentAttack = Math.Max(opponent.Damage - Armor, 1);

            return (opponent.HitPoints / myAttack) <= (HitPoints / opponentAttack);
        }
    }

    const int MyHp = 100;
    static Actor s_Opponent = new Actor(100, 8, 2);

    [Fact]
    public void Part1()
    {
        int answer = Search(
            s_Weapons.Select(w => new Outfit(w)),
            outfit => outfit.ExtendOutfit(),
            actor => actor.Wins(s_Opponent),
            (lhs, rhs) => lhs.Cost - rhs.Cost
            );

        Assert.Equal(91, answer);
    }

    [Fact]
    public void Part2()
    {
        Outfit initial = new Outfit(s_Weapons[^1], s_Armors[^1], s_Rings[2], s_Rings[5]);
        int answer = Search(
            new[] { initial },
            outfit => outfit.DiminishOutfit(),
            actor => !actor.Wins(s_Opponent),
            (lhs, rhs) => rhs.Cost - lhs.Cost
            );

        Assert.Equal(158, answer);
    }

    private int Search(IEnumerable<Outfit> initial, Func<Outfit, IEnumerable<Outfit>> getAdjacent, Predicate<Actor> test, Comparison<Outfit> comparison)
    {
        HashSet<int> visited = new HashSet<int>();
        PriorityQueue<Outfit, Outfit> queue = new(Comparer<Outfit>.Create(comparison));
        foreach (Outfit outfit in initial)
            queue.Enqueue(outfit, outfit);

        while (queue.Count > 0)
        {
            Outfit current = queue.Dequeue();

            if (!visited.Contains(current.Id))
            {
                visited.Add(current.Id);

                if (test(new Actor(MyHp, current.Damage, current.ArmorCount)))
                    return current.Cost;

                foreach (var adjacent in getAdjacent(current))
                    if (!visited.Contains(adjacent.Id))
                        queue.Enqueue(adjacent, adjacent);
            }
        }

        throw new InvalidOperationException();
    }
}
