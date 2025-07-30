namespace AdventOfCode._2015;

public class Day22
{
    private class Spell
    {
        public readonly int Id;
        public readonly int ManaCost;
        private readonly Action<GameState> _action;

        public Spell(int id, int cost, Action<GameState> action)
        {
            Id = id;
            ManaCost = cost;
            _action = action;
        }

        public static Spell Effect(int id, int cost, int durtion, Action<GameState> effectAction)
        {
            return new Spell(id, cost, state => state.ActiveEffects.Add(new Effect(id, durtion, effectAction)));
        }

        public void Apply(GameState state)
        {
            _action(state);
        }
    }

    private class Effect
    {
        public readonly int SpellId;
        private readonly Action<GameState> _action;

        public int Timer { get; private set; }

        public Effect(int spellId, int duration, Action<GameState> action)
        {
            SpellId = spellId;
            Timer = duration;
            _action = action;
        }

        public void Apply(GameState state)
        {
            if (Timer <= 0)
                throw new InvalidOperationException();

            _action(state);
            Timer--;
        }

        public Effect Clone()
        {
            return new Effect(SpellId, Timer, _action);
        }
    }

    static void MagicMissile(GameState state)
    {
        state.OpponentHp -= 4;
    }

    static void Drain(GameState state)
    {
        state.OpponentHp -= 2;
        state.MyHp += 2;
    }

    static void ShieldEffect(GameState state)
    {
        state.MyArmor += 7;
    }

    static void PoisonEffect(GameState state)
    {
        state.OpponentHp -= 3;
    }

    static void RechargeEffect(GameState state)
    {
        state.MyMana += 101;
    }

    static Spell[] s_Spells = new Spell[]
    {
        new Spell(0, 53, MagicMissile),
        new Spell(1, 73, Drain),
        Spell.Effect(2, 113, 6, ShieldEffect),
        Spell.Effect(3, 173, 6, PoisonEffect),
        Spell.Effect(4, 229, 5, RechargeEffect)
    };

    private class GameState
    {
        const int OpponentAttack = 9;

        public int MyHp { get; set; }
        public int MyArmor { get; set; }
        public int MyMana { get; set; }
        public int ManaSpent { get; set; }
        public int OpponentHp { get; set; }
        public List<Effect> ActiveEffects { get; set; }
        public int HpLostOnPlayerTurn { get; set; }

        public bool IsLoss => MyHp <= 0 && OpponentHp > 0;
        public bool IsWin => OpponentHp <= 0;

        private void ApplyEffects()
        {
            MyArmor = 0;
            foreach (Effect effect in ActiveEffects)
                effect.Apply(this);

            ActiveEffects.RemoveAll(e => e.Timer == 0);
        }

        private void ApplySpell(Spell spell)
        {
            spell.Apply(this);
        }

        private void DoOpponentAttack()
        {
            MyHp -= Math.Max(OpponentAttack - MyArmor, 1);
        }

        public IEnumerable<GameState> EnumNextGameStates()
        {
            MyHp -= HpLostOnPlayerTurn;
            if (MyHp == 0)
                yield break;

            ApplyEffects();
            if (IsWin)
            {
                yield return this;
                yield break;
            }

            foreach (Spell s in s_Spells.Where(s => s.ManaCost <= MyMana && !ActiveEffects.Any(e => e.SpellId == s.Id)))
            {
                GameState nextState = new GameState
                {
                    MyHp = MyHp,
                    MyArmor = 0,
                    MyMana = MyMana - s.ManaCost,
                    ManaSpent = ManaSpent + s.ManaCost,
                    OpponentHp = OpponentHp,
                    ActiveEffects = ActiveEffects.Select(e => e.Clone()).ToList(),
                    HpLostOnPlayerTurn = HpLostOnPlayerTurn
                };

                nextState.ApplySpell(s);
                if (!nextState.IsWin)
                {
                    nextState.ApplyEffects();
                    if (!nextState.IsWin)
                    {
                        nextState.DoOpponentAttack();
                    }
                }

                if (!nextState.IsLoss)
                    yield return nextState;
            }
        }
    }

    [Fact]
    public void Part1()
    {
        GameState initial = new GameState
        {
            MyHp = 50,
            MyArmor = 0,
            MyMana = 500,
            ManaSpent = 0,
            OpponentHp = 51,
            ActiveEffects = new List<Effect>(),
            HpLostOnPlayerTurn = 0
        };

        int answer = Search(initial);

        Assert.Equal(900, answer);
    }

    [Fact]
    public void Part2()
    {
        GameState initial = new GameState
        {
            MyHp = 50,
            MyArmor = 0,
            MyMana = 500,
            ManaSpent = 0,
            OpponentHp = 51,
            ActiveEffects = new List<Effect>(),
            HpLostOnPlayerTurn = 1
        };

        int answer = Search(initial);

        Assert.Equal(1216, answer);
    }

    private int Search(GameState initial)
    {
        PriorityQueue<GameState, int> queue = new();
        queue.Enqueue(initial, initial.ManaSpent);

        while (queue.Count > 0)
        {
            GameState current = queue.Dequeue();

            if (current.IsWin)
                return current.ManaSpent;

            foreach (GameState next in current.EnumNextGameStates())
                queue.Enqueue(next, next.ManaSpent);
        }

        throw new InvalidOperationException();
    }
}
