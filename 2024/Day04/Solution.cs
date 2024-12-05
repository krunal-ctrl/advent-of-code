namespace AdventOfCode.Y2024.Day04;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

[ProblemName("Ceres Search")]
class Solution : Solver {
    
    private static readonly string Xmas = "XMAS";
    private static readonly string Mas = "MAS";
    private static readonly (int dx, int dy)[] XmasDirections = {
        ( 0, -1 ),  // Left
        ( -1, -1 ), // Up-Left
        ( -1, 0 ),  // Up
        ( -1, 1 ),  // Up-Right
        ( 0, 1 ),   // Right
        ( 1, 1 ),   // Down-Right
        ( 1, 0 ),   // Down
        ( 1, -1 ),  // Down-Left
    };

    private static readonly (int dx1, int dy1, int dx2, int dy2)[] MasDiagonals = {
        (-1, -1, 1, 1),  // Primary diagonal
        (-1, 1, 1, -1)   // Secondary diagonal
    };
    
    public object PartOne(string input) {
        var matrix = ParseInput(input);
        int count = 0;
        
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                foreach (var d in XmasDirections) {
                    var (dx, dy) = d;
                    if (IsFormXmas(matrix, i, j, dx, dy)) {
                        count++;
                    }
                }
            }
        }
        
        return count;
    }

    public object PartTwo(string input) {
        var count = 0;
        var matrix = ParseInput(input);
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                if (IsFormX_Mas(matrix, i, j)) {
                    count++;
                }
            }
        }
        return count;
    }

    private char[,] ParseInput(string input) {
        var lines = input.Trim().Split('\n');
        int rows = lines.Length;
        int cols = lines[0].Length;
        char[,] matrix = new char[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = lines[i][j];
            }
        }

        return matrix;
    }

    private bool IsFormXmas(char[,] matrix, int x, int y, int dx, int dy) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < Xmas.Length; i++)
        {
            int r = x + i * dx;
            int c = y + i * dy;
            
            if (r < 0 || r >= rows || c < 0 || c >= cols || matrix[r, c] != Xmas[i])
            {
                return false;
            }
        }
        return true;
    }

    private bool IsFormX_Mas(char[,] matrix, int x, int y) {
        if (matrix[x, y] != 'A') return false;
        
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        return MasDiagonals.All(d => {
            var (dx1, dy1, dx2, dy2) = d;

            int x1 = x + dx1, y1 = y + dy1;
            int x2 = x + dx2, y2 = y + dy2;

            if (x1 < 0 || x1 >= rows || y1 < 0 || y1 >= cols ||
                x2 < 0 || x2 >= rows || y2 < 0 || y2 >= cols) {
                return false;
            }

            char first = matrix[x1, y1];
            char second = matrix[x2, y2];
            return (first == 'M' && second == 'S') || (first == 'S' && second == 'M');
        });
    }
}
