using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AdventOfCode.Y2024.Day07;

using System.Collections.Generic;
using System.Linq;

[ProblemName("Bridge Repair")]
class Solution : Solver {

    public object PartOne(string input) {
        var map = ParseInput(input);
        var results = new ConcurrentBag<long>();
        Parallel.ForEach(map, kvp => {
            long testValue = kvp.Key;
            Queue<long> numbers = kvp.Value;
            List<long> possibles = new List<long>() { numbers.Dequeue() };

            while (numbers.Count > 0) {
                long curr = numbers.Dequeue();
                List<long> temp = [];

                foreach (var p in possibles) {
                    temp.Add(curr + p);
                    temp.Add(curr * p);
                }

                possibles = temp;
            }

            if (possibles.Contains(testValue)) {
                results.Add(testValue);
            }

        });
        return results.Sum();
    }

    public object PartTwo(string input) {
        var map = ParseInput(input);
        var results = new ConcurrentBag<long>();
        Parallel.ForEach(map, kvp => {
            long testValue = kvp.Key;
            Queue<long> numbers = kvp.Value;
            List<long> possibles = [numbers.Dequeue()];

            while (numbers.Count > 0) {
                long curr = numbers.Dequeue();
                List<long> temp = [];

                foreach (var p in possibles) {
                    var nextValues = new List<long>
                    {
                        p + curr,
                        p * curr,
                        long.Parse(p.ToString() + curr.ToString())
                    };

                    temp.AddRange(nextValues.Where(v => v <= testValue));
                }

                possibles = temp;
            }

            if (possibles.Contains(testValue)) {
                results.Add(testValue);
            }

        });
        return results.Sum();
    }

    private IEnumerable<KeyValuePair<long, Queue<long>>> ParseInput(string input) {
        return input.Split("\n")
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var parts = line.Split(":");
                var testValue = long.Parse(parts[0].Trim());
                var numbers = new Queue<long>(parts[1].Trim().Split(' ').Select(long.Parse));
                return new KeyValuePair<long, Queue<long>>(testValue, numbers);
            });

    }
}
