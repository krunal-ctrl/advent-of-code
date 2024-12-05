namespace AdventOfCode.Y2024.Day05;

using System.Collections.Generic;
using System.Linq;

[ProblemName("Print Queue")]
class Solution : Solver {

    public object PartOne(string input) {
        var (updates, ordering) = Parse(input);
        return updates
            .Where(pages => IsSorted(pages, ordering))
            .Sum(GetMiddlePage);
    }

    public object PartTwo(string input) {
        var (updates, ordering) = Parse(input);
        return updates
            .Where(pages => !IsSorted(pages, ordering))
            .Select(pages => SortPages(pages, ordering))
            .Sum(GetMiddlePage);
    }

    int GetMiddlePage(string[] nums) => int.Parse(nums[nums.Length / 2]);

    bool IsSorted(string[] pages, HashSet<string> ordering) {
        return pages
            .Zip(pages.Skip(1), (first, second) => first + "|" + second)
            .All(ordering.Contains);
    }

    private string[] SortPages(string[] pages, HashSet<string> ordering) {
        var sortedPages = pages.ToArray();
        for (int i = 0; i < sortedPages.Length - 1; i++) {
            for (int j = 0; j < sortedPages.Length - 1 - i; j++) {
                if (ordering.Contains(sortedPages[j] + "|" + sortedPages[j + 1])) {
                    (sortedPages[j], sortedPages[j + 1]) = (sortedPages[j + 1], sortedPages[j]);
                }
            }
        }
        
        return sortedPages;
        // return pages
        //     .OrderBy(p => p, Comparer<string>.Create((p1, p2) => ordering.Contains(p1 + "|" + p2) ? -1 : 1))
        //     .ToArray();
    }
    
    (string[][] updates, HashSet<string> ordering) Parse(string input) {
        var parts = input.Split("\n\n");

        var ordering = new HashSet<string>(parts[0].Split("\n"));;

        var updates = parts[1].Split("\n").Select(line => line.Split(",")).ToArray();
        return (updates, ordering);
    }
}
