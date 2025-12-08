namespace AdventOfCode.Y2025.Day07;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

[ProblemName("Laboratories")]
class Solution : Solver {

    public object PartOne(string input) {
        return RunManifold(input).splits;
    }

    public object PartTwo(string input) {
        return RunManifold(input).timelines;
    }

    public (int splits, long timelines) RunManifold(string input) {
        var lines = input.Split("\n").Select(line => line.ToCharArray()).ToArray();
        var crow = lines.Length;
        var ccol = lines[0].Length;
        var splits = 0;
        var timelines = new long[ccol];

        for (int irow = 0; irow < crow; irow++) {
            var nextTimelines = new long[ccol];
            for (var icol = 0; icol < ccol; icol++) {
                if (lines[irow][icol] == 'S') {
                    nextTimelines[icol] = 1;
                } else if (lines[irow][icol] == '^') {
                    splits += timelines[icol] > 0 ? 1 : 0;
                    nextTimelines[icol - 1] += timelines[icol];
                    nextTimelines[icol + 1] += timelines[icol];
                } else {
                    nextTimelines[icol] += timelines[icol];
                }
            }
            timelines = nextTimelines;
        }
        return (splits, timelines.Sum());
    }
}
