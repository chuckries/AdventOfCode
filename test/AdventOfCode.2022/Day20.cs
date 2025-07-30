namespace AdventOfCode._2022;

static class LinkedListExtensions
{
    public static LinkedListNode<T> NextCircular<T>(this LinkedListNode<T> node)
    {
        return node?.Next ?? node?.List?.First ?? throw new InvalidOperationException();
    }

    public static LinkedListNode<T> PreviousCircular<T>(this LinkedListNode<T> node)
    {
        return node?.Previous ?? node?.List?.Last ?? throw new InvalidOperationException();
    }
}

public class Day20
{
    (LinkedList<long> list, LinkedListNode<long>[] order) Input(long multiplier)
    {
        long[] ints = File.ReadAllLines("Inputs/day20.txt").Select(long.Parse).Select(i => i * multiplier).ToArray();
        LinkedListNode<long>[] nodes = ints.Select(n => new LinkedListNode<long>(n)).ToArray();

        LinkedList<long> list = new();
        foreach (var node in nodes)
        {
            list.AddLast(node);
        }

        return (list, nodes);
    }

    void Mix(LinkedList<long> list, LinkedListNode<long>[] order)
    {
        foreach (var node in order)
        {
            long value = node.Value;
            if (value > 0)
            {
                var after = node.NextCircular();
                list.Remove(node);

                value = value % list.Count;

                while (value > 0)
                {
                    value -= 1;
                    after = after.NextCircular();
                }
                list.AddBefore(after, node);
            }
            else if (node.Value < 0)
            {
                var before = node.PreviousCircular();
                list.Remove(node);

                value = value % list.Count;

                while (value < 0)
                {
                    value += 1;
                    before = before.PreviousCircular();
                }
                list.AddAfter(before, node);
            }
        }
    }

    long Sum(LinkedListNode<long>[] order)
    {
        var current = order.First(n => n.Value == 0);
        long sum = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 1000; j++)
            {
                current = current.NextCircular();
            }
            sum += current.Value;
        }

        return sum;
    }

    [Fact]
    public void Part1()
    {
        (var list, var order) = Input(1);
        Mix(list, order);
        var sum = Sum(order);
        Assert.Equal(988, sum);
    }

    [Fact]
    public void Part2()
    {
        (var list, var order) = Input(811589153);

        for (int i = 0; i < 10; i++)
        {
            Mix(list, order);
        }

        var sum = Sum(order);
        Assert.Equal(7768531372516, sum);
    }
}