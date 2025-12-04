
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2025.Day04;
[ProblemName("Printing Department")]
class Solution : Solver {

    private static readonly (int dx, int dy)[] directions = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];

    public object PartOne(string input) {
        var grid = ParseInput(input);
        return GetAccessiblePositions(grid).Count;
    }

    public object PartTwo(string input) {
        var grid = ParseInput(input);
        int totalRemoved = 0;
        int removedThisRound;

        do {
            var accessible = GetAccessiblePositions(grid);
            removedThisRound = accessible.Count;
            totalRemoved += removedThisRound;

            // Remove all accessible rolls
            accessible.ForEach(pos => grid[pos.row][pos.col] = '.');
        } while (removedThisRound > 0);


        return totalRemoved;
    }

    static List<(int row, int col)> GetAccessiblePositions(char[][] grid) {
        int rows = grid.Length;
        int cols = grid[0].Length;

        return Enumerable.Range(0, rows)
            .SelectMany(i => Enumerable.Range(0, cols).Select(j => (row: i, col: j)))
            .Where(pos => grid[pos.row][pos.col] == '@')
            .Where(pos => CountNeighbors(grid, pos.row, pos.col) < 4)
            .ToList();
    }

    static int CountNeighbors(char[][] grid, int row, int col) {
        int rows = grid.Length;
        int cols = grid[0].Length;

        return directions
            .Select(d => (nr: row + d.dx, nc: col + d.dy))
            .Count(n => n.nr >= 0 && n.nr < rows && n.nc >= 0 && n.nc < cols && grid[n.nr][n.nc] == '@');
    }

    public static char[][] ParseInput(string input) {
        return input
            .Split("\n")
            .Select(line => line.ToCharArray())
            .ToArray();
    }
}
