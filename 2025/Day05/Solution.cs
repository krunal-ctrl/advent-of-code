
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2025.Day05;
[ProblemName("Cafeteria")]
class Solution : Solver {

    public object PartOne(string input) {
        var (ranges, ids) = ParseRangesAndIds(input);
        return ids.Where(id => ranges.Any(range => id >= range.Start && id <= range.End)).Count();
    }

    public object PartTwo(string input) {
        var (ranges, _) = ParseRangesAndIds(input);
        return GetUniqueIdsCountFromRanges(ranges);
    }

    public static (IEnumerable<(long Start, long End)> ranges, IEnumerable<long> ids) ParseRangesAndIds(string input) {
        var lines = input.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .ToList();

        int splitIndex = lines.FindIndex(line => !line.Contains('-'));

        var ranges = lines.Take(splitIndex)
            .Select(line => {
                var parts = line.Split('-');
                return (long.Parse(parts[0]), long.Parse(parts[1]));
            });

        var ids = lines.Skip(splitIndex)
            .Select(long.Parse);

        return (ranges, ids);
    }

    public static long GetUniqueIdsCountFromRanges(IEnumerable<(long Start, long End)> ranges) {
        if (ranges == null || !ranges.Any()) {
            return 0;
        }

        var sortedRanges = ranges.OrderBy(r => r.Start).ToList();

        long totalCount = 0;
        long currentStart = sortedRanges[0].Start;
        long currentEnd = sortedRanges[0].End;

        for (int i = 1; i < sortedRanges.Count; i++) {
            var range = sortedRanges[i];

            if (range.Start <= currentEnd + 1) {
                currentEnd = Math.Max(currentEnd, range.End);
            }

            else 
            {
                totalCount += (currentEnd - currentStart + 1);
                currentStart = range.Start;
                currentEnd = range.End;
            }
        }

        totalCount += (currentEnd - currentStart + 1);

        return totalCount;
    }
}
