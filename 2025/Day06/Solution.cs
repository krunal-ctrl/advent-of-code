
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2025.Day06;
[ProblemName("Trash Compactor")]
class Solution : Solver {

    public object PartOne(string input) {
        var problems = ParseBlocks(input).Select(block => {
            var operation = block[^1].Trim()[0];
            var nums = block[..^1].Select(long.Parse);
            return (operation, nums);
        });

        return Solve(problems);
    }

    public object PartTwo(string input) {

        var problems = ParseBlocks(input).Select(Transpose)
            .Select(block => {
                var operation = block[0].Trim()[^1];
                var nums = block.Select(b => long.Parse(b[..^1]));
                return (operation, nums);
            });

        return Solve(problems);
    }

    private static long Solve(IEnumerable<(char operation, IEnumerable<long> nums)> problems) {
        var total = 0L;
        foreach (var (operation, nums) in problems) {
            total += operation switch {
                '+' => nums.Sum(),
                '*' => nums.Aggregate(1L, (a, b) => a * b),
                _ => throw new InvalidOperationException($"Unknown operation: {operation}"),
            };
        }
        return total;
    }

    private static IEnumerable<string[]> ParseBlocks(string input) {
        var lines = input.Split("\n");
        int col = lines[0].Length;
        var blockStart = 0;
        for (int i = 0; i < col; i++) {
            if (GetColumn(lines, i).Trim() == "") { // if it not empty then there is no. in the block so continue
                yield return GetBlock(lines, blockStart, i);
                blockStart = i + 1; // move to next block
            }
        }
        yield return GetBlock(lines, blockStart, col);
    }

    private static string GetColumn(string[] lines, int col) => string.Join("", lines.Select(line => line[col]));
    private static string[] GetBlock(string[] lines, int startCol, int endCol) => [.. lines.Select(line => line[startCol..endCol])];
    private static string[] Transpose(string[] lines) => [.. Enumerable.Range(0, lines[0].Length).Select(col => GetColumn(lines, col))];
}
