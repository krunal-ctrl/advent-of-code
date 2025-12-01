namespace AdventOfCode.Y2025.Day01;

using System;
using System.Collections.Generic;
using System.Linq;

[ProblemName("Secret Entrance")]
class Solution : Solver {

    public object PartOne(string input) {
        var rotations = ParseInstructions(input);
        return SimulateDialPositions(rotations).Count(x => x == 0);
    }

    public object PartTwo(string input) {
        var rotations = ParseWithExpandedSteps(input);
        return SimulateDialPositions(rotations).Count(x => x == 0);
    }

    // ParseInstructions
    // - Expects input with one instruction per line, e.g. "R5" or "L42".
    // - Returns a sequence of signed rotations: 'R' produces +n, 'L' produces -n.
    // Example: "R5" ->  +5, "L42" -> -42
    static IEnumerable<int> ParseInstructions(string input) =>
        input.Split("\n")
            .Select(line => {
                var d = line[0] == 'R' ? 1 : -1;
                var n = int.Parse(line[1..]);
                return d * n;
            });

    // ParseWithExpandedSteps
    // - Same input format as ParseInstructions.
    // - Expands each instruction into `n` single-step directions so each yielded
    //   value is either 1 (step right) or -1 (step left).
    // Example: "R5" -> [1,1,1,1,1], "L3" -> [-1,-1,-1]
    static IEnumerable<int> ParseWithExpandedSteps(string input) =>
        input.Split("\n")
         .SelectMany(line => {
             var d = line[0] == 'R' ? 1 : -1;
             var n = int.Parse(line[1..]);
             return Enumerable.Range(0, n).Select(_ => d);
         });

    // SimulateDialPositions
    // - Simulates a circular dial with positions 0..99.
    // - Starts at 50, applies each rotation in order and yields the new position.
    // - Deferred execution + `yield return` preserve streaming behavior.
    // - The modular arithmetic wraps positions into [0,99].
    //
    // For the sample input lines:
    // R5, R49, L42, R1, R30, L41, L19, R5, R36, L22, L14, L10
    // the successive yielded positions are:
    // 55, 4, 62, 63, 93, 52, 33, 38, 74, 52, 38, 28
    static IEnumerable<int> SimulateDialPositions(IEnumerable<int> rotations) {
        var pos = 50;
        foreach (var r in rotations) {
            pos = (pos + r + 100) % 100;
            yield return pos;
        }
    }
}
