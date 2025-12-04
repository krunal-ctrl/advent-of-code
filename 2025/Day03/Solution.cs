namespace AdventOfCode.Y2025.Day03;

using System.Linq;

[ProblemName("Lobby")]
class Solution : Solver {

    public object PartOne(string input) => MaxJoltSum(input, 2);
    public object PartTwo(string input) => MaxJoltSum(input, 12);

    private static long MaxJoltSum(string input, int batteryCount) =>
        input.Split("\n").Select(bank => MaxJolt(bank, batteryCount)).Sum();

    static long MaxJolt(string bank, int batteryCount) {
        long res = 0L;
        while (batteryCount > 0) {

            var slice = bank[..^(batteryCount - 1)];
            char jolt = slice.Max();

            var cut = bank.IndexOf(jolt) + 1;
            bank = bank[cut..];

            res = 10 * res + (jolt - '0');
            batteryCount--;
        }
        return res;
    }
}
