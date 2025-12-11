
using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.Utilities;

namespace AdventOfCode.Y2025.Day11;

[ProblemName("Reactor")]
class Solution : Solver {

    public object PartOne(string input) {
        var graph = ParseInput(input);
        return CountPaths(graph, "you", "out");
    }

    public object PartTwo(string input) {
        var graph = ParseInput(input);

        var start = "svr";
        var end = "out";

        string[] requiredNodes = ["dac", "fft"];

        return requiredNodes.GetPermutations(2)
            .Select(per => CountPathsThroughNodes(graph, start, per.ToArray(), end))
            .Sum();
    }


    static Dictionary<string, List<string>> ParseInput(string input) {
        var lines = input.Split('\n');
        var graph = new Dictionary<string, List<string>>();
        foreach (var line in lines) {
            var parts = line.Split(":", StringSplitOptions.TrimEntries);
            var node = parts[0];
            var neighbors = parts[1].Split(" ").ToList();
            graph[node] = neighbors;
        }
        return graph;
    }

    static long CountPaths(
        Dictionary<string, List<string>> graph,
        string current,
        string target,
        HashSet<string> visited = null,
        Dictionary<string, long> memo = null
    ) {

        visited ??= [];
        memo ??= [];

        if (memo.TryGetValue(current, out var value)) {
            return value;
        }

        if (current == target)
            return 1;

        if (!graph.ContainsKey(current))
            return 0;

        visited.Add(current);

        long pathCount = 0;

        foreach (var next in graph[current]) {
            if (!visited.Contains(next)) {
                pathCount += CountPaths(graph, next, target, visited, memo);
            }
        }

        visited.Remove(current);
        memo[current] = pathCount;
        return pathCount;
    }

    static long CountPathsThroughNodes(
        Dictionary<string, List<string>> graph,
        string start,
        string[] intermediates,
        string end) {

        if (intermediates.Length == 0) {
            return CountPaths(graph, start, end);
        }

        long totalPaths = 1;
        var currentStart = start;

        foreach (var intermediate in intermediates) {

            var segmentPaths = CountPaths(graph, currentStart, intermediate);

            if (segmentPaths == 0) {
                return 0;
            }

            totalPaths *= segmentPaths;
            currentStart = intermediate;
        }

        totalPaths *= CountPaths(graph, currentStart, end);
        return totalPaths;
    }
}
