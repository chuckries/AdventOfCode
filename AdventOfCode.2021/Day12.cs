using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day12
    {
        private Dictionary<string, List<string>> _graph;

        public Day12()
        {
            string[] lines = File.ReadAllLines("Inputs/Day12.txt");

            _graph = new(lines.Length * 2);
            foreach (string line in lines)
            {
                string[] tok = line.Split('-');
                if (!_graph.TryGetValue(tok[0], out var list))
                    list = _graph[tok[0]] = new();
                list.Add(tok[1]);
                if (!_graph.TryGetValue(tok[1], out list))
                    list = _graph[tok[1]] = new();
                list.Add(tok[0]);
            }
        }

        [Fact]
        public void Part1()
        {
            int answer = 0;
            Backtrack("start", new() { "start" }, ref answer);
            Assert.Equal(3292, answer);

            void Backtrack(string current, HashSet<string> visited, ref int count)
            {
                if (current == "end")
                {
                    count++;
                }
                else
                {
                    foreach (string next in _graph[current])
                    {
                        if (char.IsLower(next[0]))
                        {
                            if (!visited.Contains(next))
                            {
                                visited.Add(next);
                                Backtrack(next, visited, ref count);
                                visited.Remove(next);
                            }
                        }
                        else
                        {
                            Backtrack(next, visited, ref count);
                        }
                    }
                }
            }
        }

        [Fact]
        public void Part2()
        {
            int answer = 0;
            Backtrack("start", new() { { "start", 2 } }, ref answer);
            Assert.Equal(89592, answer);

            void Backtrack(string current, Dictionary<string, int> visited, ref int count)
            {
                if (current == "end")
                {
                    count++;
                }
                else
                {
                    bool hasDouble = visited.Any(kvp => kvp.Key != "start" && kvp.Value == 2);

                    foreach (string cand in _graph[current])
                    {
                        if (cand == "start")
                            continue;

                        if (char.IsLower(cand[0]))
                        {
                            if (hasDouble)
                            {
                                if (!visited.ContainsKey(cand))
                                {
                                    visited[cand] = 1;
                                    Backtrack(cand, visited, ref count);
                                    visited.Remove(cand);
                                }
                            }
                            else
                            {
                                if (visited.ContainsKey(cand))
                                {
                                    visited[cand]++;
                                    Backtrack(cand, visited, ref count);
                                    visited[cand]--;
                                }
                                else
                                {
                                    visited[cand] = 1;
                                    Backtrack(cand, visited, ref count);
                                    visited.Remove(cand);
                                }
                            }
                        }
                        else
                        {
                            Backtrack(cand, visited, ref count);
                        }
                    }
                }
            }
        }
    }
}
