namespace AdventOfCode._2020;

public class Day18
{
    private string[] _input;

    public Day18()
    {
        _input = File.ReadAllLines("Inputs/Day18.txt");
    }

    [Fact]
    public void Part1()
    {
        long answer = _input.Select(s => Evaluate(s, c => 0)).Sum();
        Assert.Equal(701339185745, answer);
    }
    [Fact]
    public void Part2()
    {
        long answer = _input.Select(s => Evaluate(s, c => c switch
        {
            '+' => 0,
            '*' => 1,
            _ => throw new InvalidOperationException()
        })).Sum();
        Assert.Equal(4208490449905, answer);
    }

    private long Evaluate(string str, Func<char, int> precedence)
    {
        Stack<char> opStack = new();
        Stack<long> valStack = new();

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (c is ' ')
                continue;
            else if (char.IsNumber(c))
            {
                int nextIndex = str.IndexOfAny(new char[] { ' ', ')' }, i);
                if (nextIndex == -1)
                    nextIndex = str.Length;
                long num = long.Parse(str.AsSpan(i, nextIndex - i));
                i = nextIndex - 1;

                valStack.Push(num);
            }
            else if (c is '(')
                opStack.Push(c);
            else if (c is ')')
            {
                while (opStack.Peek() != '(')
                    Eval(opStack, valStack);
                opStack.Pop();
            }
            else if (c is '*' or '+')
            {
                while (opStack.Count > 0)
                {
                    char op = opStack.Peek();
                    if (op is '(' or ')' || precedence(c) < precedence(op))
                        break;

                    Eval(opStack, valStack);
                }
                opStack.Push(c);
            }
            else throw new InvalidOperationException();
        }

        while (opStack.Count > 0)
            Eval(opStack, valStack);

        return valStack.Single();

        static void Eval(Stack<char> opStack, Stack<long> valStack)
        {
            char op = opStack.Pop();
            long a = valStack.Pop();
            long b = valStack.Pop();

            long result;
            if (op == '*')
                result = a * b;
            else if (op == '+')
                result = a + b;
            else
                throw new InvalidOperationException();

            valStack.Push(result);
        }
    }
}
