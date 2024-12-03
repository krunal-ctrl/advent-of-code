namespace AdventOfCode.Y2024.Day03;

using System.Linq;
using System.Text.RegularExpressions;

[ProblemName("Mull It Over")]
class Solution : Solver {
    
    public object PartOne(string input) {
        var matchingCollection = Regex.Matches(input, @"mul\((\d+),(\d+)\)");
        var sum = matchingCollection.Sum(PerformMulOperation);
        return sum;
    }

    public object PartTwo(string input) {
        var matchingCollection = Regex.Matches(input, @"mul\((\d+),(\d+)\)|don't\(\)|do\(\)");
        return matchingCollection.Aggregate(
            (isEnabled: true, result: 0),
            (acc, m) => {
                return (m.Value) switch {
                    "don't()" => (false, acc.result),
                    "do()" => (true, acc.result),
                    _ => acc.isEnabled ? (acc.isEnabled, acc.result + PerformMulOperation(m)) : acc
                };
            }).result;
    }
    
    private int PerformMulOperation(Match match) {
        var num1 = int.Parse(match.Groups[1].Value);
        var num2 = int.Parse(match.Groups[2].Value);
        return num1 * num2;
    }
}
