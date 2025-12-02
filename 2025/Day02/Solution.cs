using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2025.Day02;
[ProblemName("Gift Shop")]
class Solution : Solver {

    public object PartOne(string input) {
        return GetIds(input)
            .Where(id => IsPeriodic(id.ToString(), 2))
            .Sum();
    }

    public object PartTwo(string input) {
        return GetIds(input)
            .Where(id => Enumerable.Range(2, id.ToString().Length).Any(k => IsPeriodic(id.ToString(), k)))
            .Sum();
    }

    private static IEnumerable<long> GetIds(string input) {
        foreach (var range in input.Split(",")) {
            var parts = range.Split("-").Select(long.Parse).ToArray();
            for (var i = parts[0]; i <= parts[1]; i++) {
                yield return i;
            }
        }
    }

    private static bool IsPeriodic(string st, int repetitionCount) {
        if (st.Length % repetitionCount != 0) {
            return false;
        }

        int period = st.Length / repetitionCount;
        for (int i = period; i < st.Length; i+=period) {
            if (st[..period] != st[i..(i+period)]) {
                return false;
            }
        }

        return true;
    }
}
