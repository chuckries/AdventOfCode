using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> FullPermutations<T>(this IEnumerable<T> inputEnumerable)
        {
            Stack<(List<T> solution, List<T> candidates)> stack = new Stack<(List<T> solution, List<T> candidates)>();
            stack.Push((new List<T>(), new List<T>(inputEnumerable)));

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
    }
}
