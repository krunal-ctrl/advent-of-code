namespace AdventOfCode.Y2024.Day01;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

[ProblemName("Historian Hysteria")]
class Solution : Solver {

    public object PartOne(string input) {
        // go over the sorted columns pairwise and sum the difference of the pairs
        var (leftList, rightList) = PrepareInput(input);
        return leftList.Zip(rightList)
            .Select(p => Math.Abs(p.First - p.Second))
            .Sum();
    }

    public object PartTwo(string input) {
        var (leftList, rightList) = PrepareInput(input);
        
        var weights = rightList.CountBy(x=>x).ToDictionary();
        return leftList.Select(num => weights.GetValueOrDefault(num) * num).Sum();
    }

    private (IEnumerable<int>, IEnumerable<int>) PrepareInput(string input) {
        var inputList = input.Split("\n")
            .Select(line => line.Split("   ").Select(int.Parse).ToArray())
            .ToArray();

        var leftList = inputList
            .OrderBy(nums => nums[0])
            .Select(nums => nums[0]);
        
        var rightList = inputList
            .OrderBy(nums => nums[1])
            .Select(nums => nums[1]);
        
        return (leftList, rightList);
    }
}
