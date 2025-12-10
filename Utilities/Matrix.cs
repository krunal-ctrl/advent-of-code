using System;
using System.Collections.Generic;
using System.Linq;

namespace adventofcode.Utilities;

public static class Matrix {

    /// <summary>Extracts a submatrix defined by row and column indices.</summary>
    public static T[,] ExtractSubMatrix<T>(T[,] M, IEnumerable<int> rowIndices, IEnumerable<int> colIndices) {
        var rows = rowIndices.ToList();
        var cols = colIndices.ToList();
        var result = new T[rows.Count, cols.Count];

        for (int i = 0; i < rows.Count; i++) {
            for (int j = 0; j < cols.Count; j++) {
                result[i, j] = M[rows[i], cols[j]];
            }
        }
        return result;
    }

    /// <summary>Computes the rank of a matrix using Gaussian elimination.</summary>
    public static int MatrixRank(double[,] M, double tol = Constants.Epsilon) {
        int rows = M.GetLength(0);
        int cols = M.GetLength(1);
        var A = (double[,])M.Clone();
        int rank = 0;

        for (int col = 0; col < cols && rank < rows; col++) {
            // Find pivot
            int pivotRow = -1;
            for (int row = rank; row < rows; row++) {
                if (Math.Abs(A[row, col]) > tol) {
                    pivotRow = row;
                    break;
                }
            }

            if (pivotRow == -1) continue;

            // Swap rows (if necessary)
            if (pivotRow != rank) {
                for (int c = 0; c < cols; c++) {
                    (A[rank, c], A[pivotRow, c]) = (A[pivotRow, c], A[rank, c]);
                }
            }

            // Eliminate
            for (int row = rank + 1; row < rows; row++) {
                double factor = A[row, col] / A[rank, col];
                for (int c = col; c < cols; c++) {
                    A[row, c] -= factor * A[rank, c];
                }
            }

            rank++;
        }
        return rank;
    }

    /// <summary>Selects indices of linearly independent columns.</summary>
    public static List<int> SelectIndependentColumns(double[,] M, double tol = Constants.Epsilon) {
        int rows = M.GetLength(0);
        int cols = M.GetLength(1);
        var independent = new List<int>();
        int currentRank = 0;

        for (int j = 0; j < cols; j++) {
            // Create submatrix with current independent columns + candidate j
            var tempCols = new List<int>(independent) { j };
            var sub = ExtractSubMatrix(M, Enumerable.Range(0, rows), tempCols);
            int r = MatrixRank(sub, tol);

            if (r > currentRank) {
                independent.Add(j);
                currentRank = r;
                if (currentRank == rows) break;
            }
        }
        return independent;
    }

    /// <summary>Selects indices of linearly independent rows.</summary>
    public static List<int> SelectIndependentRows(double[,] M, double tol = Constants.Epsilon) {
        // Transpose M to use SelectIndependentColumns logic for rows
        int rows = M.GetLength(0);
        int cols = M.GetLength(1);
        var M_T = new double[cols, rows];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                M_T[j, i] = M[i, j];
            }
        }
        return SelectIndependentColumns(M_T, tol);
    }

    /// <summary>Solves a square linear system Ax = b using Gaussian elimination with partial pivoting.</summary>
    public static double[] SolveLinearSystem(double[,] A, double[] b) {
        int n = A.GetLength(0);
        if (n != b.Length) throw new ArgumentException("Matrix A must be square and match vector b length.");

        var augmented = new double[n, n + 1];
        // Create augmented matrix
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) augmented[i, j] = A[i, j];
            augmented[i, n] = b[i];
        }

        // Forward elimination (Gaussian elimination with partial pivoting)
        for (int col = 0; col < n; col++) {
            // Find pivot
            int maxRow = col;
            for (int row = col + 1; row < n; row++) {
                if (Math.Abs(augmented[row, col]) > Math.Abs(augmented[maxRow, col])) {
                    maxRow = row;
                }
            }

            // Swap rows
            for (int c = 0; c <= n; c++) {
                (augmented[col, c], augmented[maxRow, c]) = (augmented[maxRow, c], augmented[col, c]);
            }

            // Check for singularity
            if (Math.Abs(augmented[col, col]) < Constants.Epsilon) {
                throw new Exception("Singular matrix: System has no unique solution.");
            }

            // Eliminate
            for (int row = col + 1; row < n; row++) {
                double factor = augmented[row, col] / augmented[col, col];
                for (int c = col; c <= n; c++) {
                    augmented[row, c] -= factor * augmented[col, c];
                }
            }
        }

        // Back substitution
        var x = new double[n];
        for (int i = n - 1; i >= 0; i--) {
            double sum = 0;
            for (int j = i + 1; j < n; j++) {
                sum += augmented[i, j] * x[j];
            }
            x[i] = (augmented[i, n] - sum) / augmented[i, i];
        }
        return x;
    }
}
