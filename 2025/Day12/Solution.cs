namespace AdventOfCode.Y2025.Day12;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

[ProblemName("Christmas Tree Farm")]
class Solution : Solver {

    public object PartOne(string input) {
        string[] inp = input.Trim().Split(["\n\n", "\r\n\r\n"], StringSplitOptions.None);
        string[] regions = inp.Last().Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);


        int ret = 0;
        foreach (string line in regions) {
            List<int> l = ParseRegions(line);
            int h = l[0];
            int w = l[1];

            int area = h * w;
            int sum = l[2] * 7 +
                      l[3] * 7 +
                      l[4] * 6 +
                      l[5] * 5 +
                      l[6] * 7 +
                      l[7] * 7;

            if (area >= sum) {
                ret++;
            }
        }

        return ret;
    }

    public object PartTwo(string input) {
        return 0;
    }

    List<int> ParseRegions(string input, bool neg = true) {
        string pattern = neg ? @"-?\d+" : @"\d+";

        return Regex.Matches(input, pattern)
            .Cast<Match>()
            .Select(m => int.Parse(m.Value))
            .ToList();
    }
}
