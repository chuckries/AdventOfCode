using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> FullPermutations<T>(this IEnumerable<T> source)
        {
            Stack<(List<T> solution, List<T> candidates)> stack = new Stack<(List<T> solution, List<T> candidates)>();
            stack.Push((new List<T>(), new List<T>(source)));

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.candidates.Count == 0)
                    yield return current.solution;
                else
                {
                    foreach (T candidate in current.candidates)
                    {
                        List<T> nextSolution = new List<T>(current.solution);
                        List<T> nextCandidates = new List<T>(current.candidates);
                        nextSolution.Add(candidate);
                        nextCandidates.Remove(candidate);
                        stack.Push((nextSolution, nextCandidates));
                    }
                }
            }
        }

        public static IEnumerable<(T, T)> UniquePairs<T>(this IEnumerable<T> source)
        {
            T[] items = source.ToArray();
            for (int i = 0; i < items.Length - 1; i++)
                for (int j = i + 1; j < items.Length; j++)
                    yield return (items[i], items[j]);
        }
    }
}
