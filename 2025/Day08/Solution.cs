using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2025.Day08;
[ProblemName("Playground")]
class Solution : Solver {

    public object PartOne(string input) {
        var boxes = ParseInput(input);
        var componentMap = boxes.ToDictionary(b => b, b => new HashSet<JunctionBox>([b]));

        foreach(var (a, b) in GetUniquePairsByDistance(boxes).Take(1000)) {
            if (componentMap[a] != componentMap[b]) {
                MergeComponents(a, b, componentMap);
            }
        }

        return componentMap.Values.Distinct()
            .OrderByDescending(set => set.Count)
            .Take(3)
            .Aggregate(1, (a, b) => a * b.Count);
    }

    public object PartTwo(string input) {
        var points = ParseInput(input);
        var componentCount = points.Length;
        var componentMap = points.ToDictionary(p => p, p => new HashSet<JunctionBox>([p]));
        var res = 0m;

        foreach (var (left, right) in GetUniquePairsByDistance(points).TakeWhile(_ => componentCount > 1)) {
            if (componentMap[left] != componentMap[right]) {
                MergeComponents(left, right, componentMap);
                res = left.X * right.X;
                componentCount--;
            }
        }
        return res;
    }

    static JunctionBox[] ParseInput(string input) =>
        input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => {
                var parts = line.Split(',');
                return new JunctionBox(
                    long.Parse(parts[0]),
                    long.Parse(parts[1]),
                    long.Parse(parts[2])
                );
            })
            .ToArray();

    static IEnumerable<(JunctionBox left, JunctionBox right)> GetUniquePairsByDistance(JunctionBox[] boxes) =>
        boxes
            .SelectMany(right => boxes, (left, right) => (left, right))
            .Where(t => (t.left.X, t.left.Y, t.left.Z).CompareTo((t.right.X, t.right.Y, t.right.Z)) < 0)
            .OrderBy(t => t.left.DistanceTo(t.right));

    static void MergeComponents(JunctionBox left, JunctionBox right, Dictionary<JunctionBox, HashSet<JunctionBox>> componentMap) {
        componentMap[left].UnionWith(componentMap[right]);
        foreach (var jb in componentMap[right]) {
            componentMap[jb] = componentMap[left];
        }
    }

}


public class JunctionBox {
    public long X { get; set; }
    public long Y { get; set; }
    public long Z { get; set; }

    public JunctionBox(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double DistanceTo(JunctionBox other) {
        long dx = X - other.X;
        long dy = Y - other.Y;
        long dz = Z - other.Z;
        return dx * dx + dy * dy + dz * dz;
    }
}
