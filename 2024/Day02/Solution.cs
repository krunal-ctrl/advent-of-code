namespace AdventOfCode.Y2024.Day02;

using System.Collections.Generic;
using System.Linq;

[ProblemName("Red-Nosed Reports")]
class Solution : Solver {

    public object PartOne(string input) {
        return 
            ParsedSample(input)
                .Count(IsValid);
    }

    public object PartTwo(string input) {
        return 
            ParsedSample(input)
                .Count(samples => GeneratePairs(samples).Any(IsValid));
    }

    IEnumerable<int[]> GeneratePairs(int[] samples) {
        var result = new List<int[]>();

        for (int i = 0; i < samples.Length; i++)
        {
            var newArray = new List<int>(samples);
            newArray.RemoveAt(i);
            result.Add(newArray.ToArray());
        }

        return result;
    }

    bool IsValid(int[] samples) {
        var pairs = samples.Zip(samples.Skip(1)).ToArray();
        return pairs.All(p => 1 <= p.Second - p.First && p.Second - p.First <= 3) ||
               pairs.All(p => 1 <= p.First - p.Second && p.First - p.Second <= 3);
    }

    IEnumerable<int[]> ParsedSample(string input) {
        return input.Split("\n")
            .Select(line => line.Split(" ").Select(int.Parse).ToArray());
    }
}
