namespace AdventOfCode.Y2024.Day06;

using System.Collections.Generic;
using System.Linq;
using CharMap = System.Collections.Generic.Dictionary<(int x, int y), char>;

[ProblemName("Guard Gallivant")]
class Solution : Solver {

    (int x, int y)[] deltas = { ( -1, 0 ), ( 0, 1 ), ( 1, 0 ), ( 0, -1 ) };

    public object PartOne(string input) {
        var (map, start) = ParseInput(input);
        return Path(map, start).positions.Count();
    }

    public object PartTwo(string input) {
        var (map, start) = ParseInput(input);
        var positions = Path(map, start).positions;
        var loops = 0;
        // simply try a blocker in each locations visited by the guard and count the loops
        foreach (var block in positions.Where(pos => map[pos] == '.')) {
            map[block] = '#';
            if (Path(map, start).isLoop) {
                loops++;
            }
            map[block] = '.';
        }
        return loops;
    }

    (IEnumerable<(int x, int y)> positions, bool isLoop) Path(CharMap map, (int x, int y) pos) {
        var visited = new HashSet<((int x, int y) pos, (int x, int y) direction)> ();
        var dir = 0;
        var currentDirection = deltas[dir];
        
        while (map.ContainsKey(pos) && !visited.Contains((pos, currentDirection))) {
            
            visited.Add((pos, currentDirection));

            int dx = pos.x + currentDirection.x;
            int dy = pos.y + currentDirection.y;
            (int x, int y) nextPos = (dx, dy);
            
            if (map.GetValueOrDefault(nextPos) == '#') {
                dir = (dir + 1) % 4;
                currentDirection = deltas[dir];
            } else {
                pos = nextPos;
            }
        }
        

        return (
            positions: visited.Select(s => s.pos).Distinct(),
            isLoop: visited.Contains((pos, currentDirection))
        );
    }
    
    private (CharMap map, (int x, int y) pos) ParseInput(string input) {
        
        var lines = input.Split("\n");
        var map = lines
            .SelectMany((line, x) => line
                .Select((ch, y) => new KeyValuePair<(int x, int y), char>((x, y), ch)))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var start = map.First(x => x.Value == '^').Key;

        return (map, start);
    }
}
