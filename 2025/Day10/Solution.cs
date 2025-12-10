using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.Utilities;

namespace AdventOfCode.Y2025.Day10;
[ProblemName("Factory")]

record ProblemPart1(int target, int[] buttons);
record ProblemPart2(List<int> pattern, List<List<int>> indexLists, List<int> b);
class Solution : Solver {

    // --- Unified Parsing ---
    private static IEnumerable<(List<int> pattern, List<List<int>> indexLists, List<int> b, int targetBitmask, int[] buttonsBitmasks)> ParseInput(string input) {
        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
            var parts = line.Split(" ").ToArray();

            // Part 1 Target (Bitmask from [....])
            var targetPattern = parts.First()
                                     .Replace("[", "").Replace("]", "")
                                     .Select(c => c == '#' ? 1 : 0);


            int targetBitmask = Convert.ToInt32(string.Join("", targetPattern.Reverse()), 2);

            // Part 1 Buttons (Bitmasks from (N), (M,O), etc.)
            var buttonsBitmasks = parts[1..^1]
                                     .Select(part => Regex.Matches(part, @"\d")
                                                           .Select(m => 1 << int.Parse(m.Value))
                                                           .Sum())
                                     .ToArray();

            // Part 2 pattern (List<int> from [....])
            List<int> pattern = targetPattern.ToList();

            // Part 2 indexLists (List<List<int>> from (..), (..), ...)
            var indexLists = new List<List<int>>();
            var groups = Regex.Matches(line, @"\((.*?)\)");
            foreach (Match match in groups) {
                string g = match.Groups[1].Value.Trim();
                indexLists.Add(string.IsNullOrEmpty(g)
                    ? new List<int>()
                    : g.Split(',').Select(x => int.Parse(x.Trim())).ToList());
            }

            // Part 2 b vector (List<int> from {..})
            List<int> b = null;
            var bMatch = Regex.Match(line, @"\{(.*?)\}");
            if (bMatch.Success) {
                string raw = bMatch.Groups[1].Value;
                b = raw.Split(',')
                       .Where(x => !string.IsNullOrWhiteSpace(x))
                       .Select(x => int.Parse(x.Trim()))
                       .ToList();
            }

            yield return (pattern, indexLists, b, targetBitmask, buttonsBitmasks);
        }
    }
    // --- End Unified Parsing ---

    public object PartOne(string input) {
        var problems = ParseInput(input).Select(p => new ProblemPart1(p.targetBitmask, p.buttonsBitmasks));
        int totalMinButtons = 0;

        foreach (var p in problems) {
            // Optimization: Iterate over masks ordered by BitCount (Hamming weight) to find the minimum mask first.
            int limit = 1 << p.buttons.Length;
            var tries = Enumerable.Range(0, limit)
                                  .OrderBy(Util.BitCount)
                                  .ToArray();

            int minMask = -1;
            foreach (var mask in tries) {
                if ((Util.Xor(p.buttons, mask) ^ p.target) == 0) {
                    minMask = mask;
                    break;
                }
            }

            if (minMask == -1) {
                throw new Exception("No solution found for Part One problem.");
            }
            totalMinButtons += Util.BitCount(minMask);
        }
        return totalMinButtons;
    }

    public object PartTwo(string input) {
        var problems = ParseInput(input).Select(p => new ProblemPart2(p.pattern, p.indexLists, p.b));
        return problems.Select(p => SolvePartTwo(p)).Sum();
    }

    double SolvePartTwo(ProblemPart2 p) {
        if (p.pattern == null || p.b == null) {
            Console.WriteLine("HIBA: Missing pattern or b vector");
            return 0;
        }

        int dim = p.pattern.Count;
        var M = IndicesToBinaryColumnMatrix(p.indexLists, dim);
        var b = p.b.Select(x => (double)x).ToArray();

        // M*x = b. We want to find the non-negative integer vector x with min(sum(x)).
        // This is a variation of Integer Programming.

        // 1. Decompose M into A (full rank, square) and Y (remaining columns).
        // M = [A | Y]. System becomes A*x_A + Y*x_Y = b.
        // x_A = A^-1 * (b - Y*x_Y)
        var (A, bSquare, Y_cols, M_indepCols) = MakeSquareAAndY(M, b);
        var indepColsCount = M_indepCols.Count;
        var freeColsCount = p.indexLists.Count - indepColsCount;

        double bestSum = double.MaxValue;

        // 2. Iterate over non-negative integer vectors 'i' (representing x_Y) in order of increasing sum (k = sum(i)).
        // This is the core optimization: trying smallest sums first.
        foreach (var i_vector in Combinations(freeColsCount)) {

            // Check early exit based on current sum of 'i'
            // The logic (i == null || i.Sum() >= bestSum) from the original code
            // is essentially a break condition for the outer loop if the current sum 
            // of the 'i' vector is already greater than or equal to the best found sum.
            // i_vector == null implies sum is 0 (first iteration).
            if (i_vector != null && i_vector.Sum() >= bestSum) {
                break;
            }

            // Calculate q = b - Y * i_vector
            double[] q;
            if (i_vector == null) {
                q = bSquare.Clone() as double[];
            } else {
                q = (double[])bSquare.Clone();
                // Compute Y*i_vector and subtract from bSquare (q)
                for (int row = 0; row < bSquare.Length; row++) {
                    double sum = 0;
                    for (int col = 0; col < freeColsCount; col++) {
                        sum += Y_cols[col][row] * i_vector[col];
                    }
                    q[row] -= sum;
                }
            }

            double[] x_A;
            try {
                // 3. Solve the resulting square system: A * x_A = q
                x_A = Matrix.SolveLinearSystem(A, q);
            } catch (Exception) {
                // System is singular (shouldn't happen if A is full rank), skip this 'i'
                continue;
            }

            // 4. Check non-negativity and integrality of x_A
            bool nonNegative = x_A.All(val => val >= -Constants.Epsilon);
            bool integers = x_A.All(val => Math.Abs(val - Math.Round(val)) < Constants.Epsilon);

            if (nonNegative && integers) {
                double currentSum = x_A.Sum();
                if (i_vector != null) {
                    currentSum += i_vector.Sum();
                }

                if (currentSum < bestSum) {
                    bestSum = currentSum;
                }
            }
        }

        // The original code uses a small constant for the initial best sum, 
        // using double.MaxValue is safer for general case.
        if (bestSum == double.MaxValue) {
            throw new Exception("Part Two: No non-negative integer solution found.");
        }

        return bestSum;
    }

    /// <summary>Transforms a list of index lists into a binary column matrix.</summary>
    public static double[,] IndicesToBinaryColumnMatrix(List<List<int>> indexLists, int dim) {
        int nCols = indexLists.Count;
        var M = new double[dim, nCols];

        for (int col = 0; col < nCols; col++) {
            foreach (int row in indexLists[col]) {
                if (row >= 0 && row < dim) { // Added bounds check
                    M[row, col] = 1.0;
                }
            }
        }
        return M;
    }

    /// <summary>
    /// Decomposes the system M*x = b into A*x_A + Y*x_Y = b, where A is a square, full-rank matrix
    /// derived from M, and Y contains the remaining columns.
    /// </summary>
    /// <returns>A (square matrix), bNew (corresponding b vector), Y (list of dependent columns vectors), indepCols (indices of columns in A)</returns>
    public static (double[,] A, double[] bNew, List<double[]> Y, List<int> indepCols) MakeSquareAAndY(
        double[,] M, double[] b) {
        int rows = M.GetLength(0);
        int cols = M.GetLength(1);

        // 1. Select a maximal set of linearly independent columns (Basis for M's column space)
        var indepCols = Matrix.SelectIndependentColumns(M);
        int rank = indepCols.Count;

        if (rank == 0) {
            throw new Exception("Matrix rank is 0, no independent columns.");
        }

        // 2. Select a maximal set of linearly independent rows from M's column space 
        //    (This ensures A is square and invertible for the sub-system).
        var MCol = Matrix.ExtractSubMatrix(M, Enumerable.Range(0, rows), indepCols);
        var indepRows = Matrix.SelectIndependentRows(MCol);

        // If the number of independent rows found is less than the number of independent columns found,
        // it implies that the rank is actually smaller than 'rank' and we need to trim the column set.
        // This is crucial for numerical stability and correctness.
        if (indepRows.Count < rank) {
            rank = indepRows.Count;
            indepCols = indepCols.Take(rank).ToList();
            // Re-select rows based on the smaller set of independent columns to ensure they match
            MCol = Matrix.ExtractSubMatrix(M, Enumerable.Range(0, rows), indepCols);
            indepRows = Matrix.SelectIndependentRows(MCol); // Should now have size 'rank'
        }


        // 3. Create the square, invertible matrix A and the corresponding reduced b vector
        var A = Matrix.ExtractSubMatrix(M, indepRows, indepCols);
        var bNew = indepRows.Select(i => b[i]).ToArray();

        // 4. Create Y: Extract the columns of M that are NOT in indepCols (the free variables)
        var Y_cols = new List<double[]>();
        var indepSet = new HashSet<int>(indepCols);
        int reducedRows = indepRows.Count;

        for (int j = 0; j < cols; j++) {
            if (!indepSet.Contains(j)) {
                var col = new double[reducedRows];
                for (int i = 0; i < reducedRows; i++) {
                    col[i] = M[indepRows[i], j]; // Only take rows corresponding to indepRows
                }
                Y_cols.Add(col);
            }
        }

        return (A, bNew, Y_cols, indepCols);
    }

    /// <summary>Generates non-negative integer vectors of length n in increasing order of sum.</summary>
    public static IEnumerable<int[]> Combinations(int n) {
        // Yield the zero vector first (sum=0)
        yield return n == 0 ? null : new int[n]; // Adjusted logic: null for n=0 means empty set of free variables
        if (n == 0) yield break;

        int k = 1; // Start with sum 1
        while (true) {
            foreach (var q in NonNegativeIntegerVectorsWithSum(n, k)) {
                yield return q;
            }
            k++;
            // Note: This continues indefinitely unless the caller adds a break condition (as in the original PartTwo logic).
        }
    }

    /// <summary>Generates all non-negative integer vectors of length n that sum up to k.</summary>
    /// <remarks>This is an internal recursive implementation.</remarks>
    public static IEnumerable<int[]> NonNegativeIntegerVectorsWithSum(int n, int k) {
        if (n == 1) {
            yield return new int[] { k };
            yield break;
        }

        for (int i = 0; i <= k; i++) {
            foreach (var rest in NonNegativeIntegerVectorsWithSum(n - 1, k - i)) {
                var result = new int[n];
                result[0] = i;
                Array.Copy(rest, 0, result, 1, rest.Length);
                yield return result;
            }
        }
    }
}
